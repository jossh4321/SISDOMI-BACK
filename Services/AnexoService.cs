using Microsoft.AspNetCore.Mvc;
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
    public class AnexoService
    {
        private readonly IMongoCollection<Anexo> _anexos;

        public AnexoService(ISysdomiDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _anexos = database.GetCollection<Anexo>("anexos");
        }

        public async Task<List<AnexoDTO>> GetAll(String fromDate, String toDate)
        {
            List<AnexoDTO> listAnexos;

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
            var projectAnexoNormal = new BsonDocument("$project", new BsonDocument
            {
                { "_id", 1},
                { "titulo", 1 },
                { "descripcion", 1 },
                { "enlaces", 1 },
                { "idresidente", 1 },
                { "idcreador", 1 },
                { "fechacreacion", 1 },
                { "area", 1 }

            });

            //Para obtener los datos del residente usando el lookup
            var lookupResidente = new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "residentes" },
                {"let", new BsonDocument("idResidente", "$idresidente") },
                { "pipeline", new BsonArray
                             {
                                new BsonDocument("$match",
                                new BsonDocument("$expr",
                                new BsonDocument("$eq",
                                new BsonArray {
                                    "$_id",
                                    new BsonDocument("$toObjectId", "$$idResidente")
                                })))
                             }
                },
                { "as", "residente" }
            });

            //Para obtener los datos del residente usando el lookup
            var lookupCreador = new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "usuarios" },
                {"let", new BsonDocument("idCreador", "$idcreador") },
                { "pipeline", new BsonArray
                             {
                                new BsonDocument("$match",
                                new BsonDocument("$expr",
                                new BsonDocument("$eq",
                                new BsonArray {
                                    "$_id",
                                    new BsonDocument("$toObjectId", "$$idCreador")
                                })))
                             }
                },
                { "as", "creador" }
            });

            //Para cambiar el arrays de residentes por un objeto utilizando unwind
            var unwindResidente = new BsonDocument("$unwind", new BsonDocument("path", "$residente"));

            //Para cambiar el arrays de creador por un objeto utilizando unwind
            var unwindCreador = new BsonDocument("$unwind", new BsonDocument("path", "$creador"));

            //Para solo enviar los datos que se necesitan utilizando project
            var projectAnexo = new BsonDocument("$project", new BsonDocument
            {
                { "titulo", 1 },
                { "descripcion", 1 },
                { "idresidente", 1 },
                { "idcreador", 1 },
                { "creador", new BsonDocument("$concat",
                             new BsonArray{
                                   "$creador.datos.nombre",
                                   " ",
                                   "$creador.datos.apellido"
                               })
                },
                { "fechacreacion", 1 },
                { "area", 1 },
                { "enlaces", 1 },
                { "residente", new BsonDocument("$concat",
                               new BsonArray{
                                   "$residente.nombre",
                                   " ",
                                   "$residente.apellido"
                               })
                }
            });

            listAnexos = await _anexos.Aggregate()
                                                    .AppendStage<dynamic>(addFieldDayYearMonth)
                                                    .AppendStage<dynamic>(addFieldDate)
                                                    .AppendStage<dynamic>(matchPlanesBetweenDate)
                                                    .AppendStage<dynamic>(projectAnexoNormal)
                                                    .AppendStage<dynamic>(lookupResidente)
                                                    .AppendStage<dynamic>(unwindResidente)
                                                    .AppendStage<dynamic>(lookupCreador)
                                                    .AppendStage<dynamic>(unwindCreador)
                                                    .AppendStage<AnexoDTO>(projectAnexo)
                                                    .ToListAsync();

            return listAnexos;
        }

        public async Task<ActionResult<AnexoDTO>> GetAnexoById(string id)
        {
            AnexoDTO anexoDTO;

            var matchId = new BsonDocument("$match",
                                           new BsonDocument("$expr",
                                                    new BsonDocument("$eq", new BsonArray
                                                    {
                                                        "$_id",
                                                        new BsonDocument("$toObjectId", id)
                                                    })));


            var projectGeneralDataAnexo = new BsonDocument("$project", new BsonDocument
            {
                { "titulo", 1 },
                { "descripcion", 1 },
                { "idresidente", 1 },
                { "idcreador", 1 },
                { "fechacreacion", 1 },
                { "area", 1 },
                { "enlaces", 1 }
            });


            //Para obtener los datos del residente usando el lookup
            var lookupResidente = new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "residentes" },
                {"let", new BsonDocument("idResidente", "$idresidente") },
                { "pipeline", new BsonArray
                             {
                                new BsonDocument("$match",
                                new BsonDocument("$expr",
                                new BsonDocument("$eq",
                                new BsonArray {
                                    "$_id",
                                    new BsonDocument("$toObjectId", "$$idResidente")
                                })))
                             }
                },
                { "as", "residente" }
            });

            //Para obtener los datos del residente usando el lookup
            var lookupCreador = new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "usuarios" },
                {"let", new BsonDocument("idCreador", "$idcreador") },
                { "pipeline", new BsonArray
                             {
                                new BsonDocument("$match",
                                new BsonDocument("$expr",
                                new BsonDocument("$eq",
                                new BsonArray {
                                    "$_id",
                                    new BsonDocument("$toObjectId", "$$idCreador")
                                })))
                             }
                },
                { "as", "creador" }
            });

            //Para cambiar el arrays de residentes por un objeto utilizando unwind
            var unwindResidente = new BsonDocument("$unwind", new BsonDocument("path", "$residente"));

            //Para cambiar el arrays de creador por un objeto utilizando unwind
            var unwindCreador = new BsonDocument("$unwind", new BsonDocument("path", "$creador"));

            //Para solo enviar los datos que se necesitan utilizando project
            var projectAnexo = new BsonDocument("$project", new BsonDocument
            {
                { "titulo", 1 },
                { "descripcion", 1 },
                { "idresidente", 1 },
                { "idcreador", 1 },
                { "creador", new BsonDocument("$concat",
                             new BsonArray{
                                   "$creador.datos.nombre",
                                   " ",
                                   "$creador.datos.apellido"
                               })
                },
                { "fechacreacion", 1 },
                { "area", 1 },
                { "enlaces", 1 },
                { "residente", new BsonDocument("$concat",
                               new BsonArray{
                                   "$residente.nombre",
                                   " ",
                                   "$residente.apellido"
                               })
                }
            });

            anexoDTO = await _anexos.Aggregate()
                                                .AppendStage<dynamic>(matchId)
                                                .AppendStage<dynamic>(projectGeneralDataAnexo)
                                                .AppendStage<dynamic>(lookupResidente)
                                                .AppendStage<dynamic>(unwindResidente)
                                                .AppendStage<dynamic>(lookupCreador)
                                                .AppendStage<dynamic>(unwindCreador)
                                                .AppendStage<AnexoDTO>(projectAnexo)
                                                .FirstOrDefaultAsync();
            return anexoDTO;
        }

        public async Task<ActionResult<Anexo>> CreateAnexo(Anexo anexo)
        {            
            anexo.fechacreacion = DateTime.UtcNow.AddHours(-5);
            await _anexos.InsertOneAsync(anexo);
            return anexo;
        }

        public async Task<Anexo> ModifyAnexo(Anexo anexo)
        {
            var filter = Builders<Anexo>.Filter.Eq("id", anexo.id);
            var update = Builders<Anexo>.Update
                .Set("titulo", anexo.titulo)
                .Set("descripcion", anexo.descripcion)
                .Set("enlaces", anexo.enlaces)
                .Set("area", anexo.area)
                .Set("idresidente", anexo.idresidente);

            var resultado = await _anexos.FindOneAndUpdateAsync<Anexo>(filter, update, new FindOneAndUpdateOptions<Anexo>
            {
                ReturnDocument = ReturnDocument.After
            });

            return resultado;
        }
    }
}
