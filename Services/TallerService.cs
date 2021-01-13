using MongoDB.Bson;
using MongoDB.Driver;
using SISDOMI.DTOs;
using SISDOMI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Services
{
    public class TallerService
    {
        private readonly IMongoCollection<TallerDTO> _talleres;

        public TallerService(ISysdomiDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _talleres = database.GetCollection<TallerDTO>("talleres");
        }

        public async Task<List<Taller>> GetAll(String fromDate, String toDate)
        {
            List<Taller> listTaller = new List<Taller>();

            // Para realizar el filtro de los planes por fechas
            var addFieldDayYearMonth = new BsonDocument("$addFields",
                                       new BsonDocument
                                       {
                                           { "mes", new BsonDocument("$month", "$fechacreacion") },
                                           { "ano", new BsonDocument("$year", "$fechacreacion") },
                                           { "dia", new BsonDocument("$dayOfMonth", "$fechacreacion") }
                                       });

            //Se obtiene solamente la fecha sin los minutos ni los milisegundos
            var addFieldDate = new BsonDocument("$addFields",
                               new BsonDocument("fecha",
                               new BsonDocument("$toDate",
                               new BsonDocument("$concat",
                               new BsonArray
                               {
                                   new BsonDocument("$toString", "$ano"),
                                   "-",
                                   new BsonDocument("$toString", "$mes"),
                                   "-",
                                   new BsonDocument("$toString", "$dia")
                               }))));

            BsonValue fromDateTransform;
            BsonValue toDateTransform;

            if (fromDate != null)
            {
                fromDateTransform = fromDate;
            }
            else
            {
                fromDateTransform = BsonNull.Value;
            }

            if (toDate != null)
            {
                toDateTransform = toDate;
            }
            else
            {
                toDateTransform = BsonNull.Value;
            }

            //Obtener los planes donde solamente este entre las fechas consultadas
            var matchPlanesBetweenDate = new BsonDocument("$match",
                                         new BsonDocument("$expr",
                                         new BsonDocument("$and",
                                         new BsonArray
                                         {
                                             new BsonDocument("$or",
                                             new BsonArray
                                             {
                                                 new BsonDocument("$gte",
                                                 new BsonArray
                                                 {
                                                     "$fecha",
                                                     new BsonDocument("$toDate", fromDateTransform)
                                                 }),
                                                 new BsonDocument("$eq",
                                                 new BsonArray
                                                 {
                                                     fromDateTransform,
                                                     BsonNull.Value
                                                 })
                                             }),
                                             new BsonDocument("$or",
                                             new BsonArray
                                             {
                                                 new BsonDocument("$lte",
                                                 new BsonArray
                                                 {
                                                     "$fecha",
                                                     new BsonDocument("$toDate", toDateTransform)
                                                 }),
                                                 new BsonDocument("$eq",
                                                 new BsonArray
                                                 {
                                                     toDateTransform,
                                                     BsonNull.Value
                                                 })
                                             })
                                         }
                                         )));

            // Para eliminar las variables creadas para la consulta entre fechas
            var projectPlanNormal = new BsonDocument("$project", new BsonDocument
            {
                { "_id", 1},
                {"_t", 1 },
                { "tipo", 1 },
                { "historialcontenido", 1 },
                { "creadordocumento", 1 },
                { "fechacreacion", 1 },
                { "area", 1 },
                { "fase", 1 },
                { "idresidente", 1 },
                { "estado", 1 },
                { "contenido", 1 }

            });

            var proyect = new BsonDocument("$project",
    new BsonDocument
        {
            { "_id", 1 },
            { "titulo", 1 },
            { "tipo",
    new BsonDocument("$cond",
    new BsonDocument
                {
                    { "if",
    new BsonDocument("$eq",
    new BsonArray
                        {
                            "$tipo",
                            "TallerEscuelaPadres"
                        }) },
                    { "then", "Taller de escuela para padres" },
                    { "else",
    new BsonDocument("$cond",
    new BsonDocument
                        {
                            { "if",
    new BsonDocument("$eq",
    new BsonArray
                                {
                                    "$tipo",
                                    "TallerEducativo"
                                }) },
                            { "then", "Taller Educativo" },
                            { "else", "Taller Formativo de Egreso" }
                        }) }
                }) },
            { "area", 1 },
            { "fase", 1 },
            { "fechacreacion", 1 }
        });

            listTaller = await _talleres.Aggregate()
                                .AppendStage<dynamic>(addFieldDayYearMonth)
                                .AppendStage<dynamic>(addFieldDate)
                                .AppendStage<dynamic>(matchPlanesBetweenDate)
                                .AppendStage<dynamic>(projectPlanNormal)
                                .AppendStage<Taller>(proyect)
                                .ToListAsync();

            return listTaller;
        }

        public async Task<List<TallerEscuelaPadres>> GetAllTEP()
        {
            List<TallerEscuelaPadres> listTaller = new List<TallerEscuelaPadres>();

            var match = new BsonDocument("$match",
                    new BsonDocument("tipo", "TallerEscuelaPadres"));

            listTaller = await _talleres.Aggregate()
                                .AppendStage<TallerEscuelaPadres>(match)
                                .ToListAsync();

            return listTaller;
        }

        public async Task<List<TallerEducativo>> GetAllTE()
        {
            List<TallerEducativo> listTaller = new List<TallerEducativo>();

            var match = new BsonDocument("$match",
                    new BsonDocument("tipo", "TallerEducativo"));

            listTaller = await _talleres.Aggregate()
                                .AppendStage<TallerEducativo>(match)
                                .ToListAsync();

            return listTaller;
        }

        public async Task<List<TallerFormativoEgreso>> GetAllTFE()
        {
            List<TallerFormativoEgreso> listTaller = new List<TallerFormativoEgreso>();

            var match = new BsonDocument("$match",
                    new BsonDocument("tipo", "TallerFormativoEgreso"));

            listTaller = await _talleres.Aggregate()
                                .AppendStage<TallerFormativoEgreso>(match)
                                .ToListAsync();

            return listTaller;
        }

        //Creacion de talleres
        public TallerEscuelaPadres CreateTEP(TallerEscuelaPadres mitaller)
        {
            mitaller.fechaCreacion = DateTime.UtcNow.AddHours(-5);
            _talleres.InsertOne(mitaller);
            return mitaller;
        }

        public TallerEducativo CreateTE(TallerEducativo mitaller)
        {
            mitaller.fechaCreacion = DateTime.UtcNow.AddHours(-5);
            _talleres.InsertOne(mitaller);
            return mitaller;
        }

        public TallerFormativoEgreso CreateTFE(TallerFormativoEgreso mitaller)
        {
            mitaller.fechaCreacion = DateTime.UtcNow.AddHours(-5);
            _talleres.InsertOne(mitaller);
            return mitaller;
        }

        //cONSULTA DE cualquier TALLER POR ID
        public async Task<Taller> GetById(string id)
        {
            var match = new BsonDocument("$match",
                        new BsonDocument("_id",
                        new ObjectId(id)));

            Taller documento = new Taller();
            documento = await _talleres.Aggregate()
                            .AppendStage<Taller>(match)
                            .FirstAsync();
            return documento;
        }

        //MODIFICACIONES
        
        public async Task<Taller> PutTallerEP(TallerEscuelaPadres taller)
        {
            var filter = Builders<TallerDTO>.Filter.Eq("id", taller.id);
            var update = Builders<TallerDTO>.Update
                .Set("titulo", taller.titulo)
                .Set("descripcion", taller.descripcion)
                .Set("firma", taller.firma)
                .Set("contenido", taller.contenido);

            await _talleres.UpdateOneAsync(filter, update);

            Taller mitaller = await GetById(taller.id);
            return mitaller;

        }
        public async Task<Taller> PutTallerE(TallerEducativo taller)
        {
            var filter = Builders<TallerDTO>.Filter.Eq("id", taller.id);
            var update = Builders<TallerDTO>.Update
                .Set("titulo", taller.titulo)
                .Set("descripcion", taller.descripcion)
                .Set("firma", taller.firma)
                .Set("contenido", taller.contenido);

            await _talleres.UpdateOneAsync(filter, update);

            Taller mitaller = await GetById(taller.id);
            return mitaller;

        }
        public async Task<Taller> PutTallerFE(TallerFormativoEgreso taller)
        {
            var filter = Builders<TallerDTO>.Filter.Eq("id", taller.id);
            var update = Builders<TallerDTO>.Update
                .Set("titulo", taller.titulo)
                .Set("descripcion", taller.descripcion)
                .Set("firma", taller.firma)
                .Set("contenido", taller.contenido);

            await _talleres.UpdateOneAsync(filter, update);

            Taller mitaller = await GetById(taller.id);
            return mitaller;
        }
    }
}
