using MongoDB.Bson;
using MongoDB.Driver;
using SISDOMI.DTOs;
using SISDOMI.Entities;
using SISDOMI.Helpers;
using SISDOMI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace SISDOMI.Services
{
    public class SesionesEducativasService
    {
        private readonly IMongoCollection<SesionEducativa> _sesioneducativa;

        public SesionesEducativasService(ISysdomiDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _sesioneducativa = database.GetCollection<SesionEducativa>("sesioneseducativas");

        }

        //Trae la lista de sesiones educativas de la bd
        public async Task<List<SesionEducativaDTOInicial>> GetAll()
        {
            List<SesionEducativaDTOInicial> sesionEducativa = new List<SesionEducativaDTOInicial>();

            var match = new BsonDocument("$match",
            new BsonDocument("tipo",
            new BsonDocument("$eq", "Sesion Educativa")));

            var addFields =
            new BsonDocument("$addFields",
            new BsonDocument("idcreadorpk",
            new BsonDocument("$toObjectId", "$idcreador")));

            var lookup =
            new BsonDocument("$lookup",
            new BsonDocument
                {
                    { "from", "usuarios" },
                    { "localField", "idcreadorpk" },
                    { "foreignField", "_id" },
                    { "as", "datoscreador" }
                });

            var unwind =
            new BsonDocument("$unwind",
            new BsonDocument("path", "$datoscreador"));

            var project =
            new BsonDocument("$project",
            new BsonDocument
                {
                    { "idcreadorpk", 0 },
                    { "datoscreador",
            new BsonDocument
                    {
                        { "_id", 0 },
                        { "clave", 0 },
                        { "datos",
            new BsonDocument
                        {
                            { "fechanacimiento", 0 },
                            { "numerodocumento", 0 },
                            { "tipodocumento", 0 },
                            { "direccion", 0 }
                        } }
                    } }
            });

            sesionEducativa = await _sesioneducativa.Aggregate()
                                .AppendStage<dynamic>(match)
                                .AppendStage<dynamic>(addFields)
                                .AppendStage<dynamic>(lookup)
                                .AppendStage<dynamic>(unwind)
                                .AppendStage<SesionEducativaDTOInicial>(project)
                                .ToListAsync();

            return sesionEducativa;
        }
        //Trae una sesion educativa de la bd segun id con datos de residente (DTO)
        public async Task<SesionEducativaDTO> GetSesionEducativaDTO(string id)
        {
            //Observem, a la Gran Teresa
            SesionEducativaDTO sesionEducativaDTO = new SesionEducativaDTO();

            var match = new BsonDocument("$match",
                        new BsonDocument("_id",
                        new ObjectId(id)));

            var unwind = new BsonDocument("$unwind",
                        new BsonDocument("path", "$contenido.participantes"));

            var addfields = new BsonDocument("$addFields",
                        new BsonDocument("idparticipantepro",
                        new BsonDocument("$toObjectId", "$contenido.participantes.idparticipante")));

            var lookup = new BsonDocument("$lookup",
                        new BsonDocument
                            {
                                { "from", "residentes" },
                                { "localField", "idparticipantepro" },
                                { "foreignField", "_id" },
                                { "as", "contenido.participantes.datosresidente" }
                            });

            var unwind2 = new BsonDocument("$unwind",
                        new BsonDocument
                            {
                                { "path", "$contenido.participantes.datosresidente" },
                                { "preserveNullAndEmptyArrays", true }
                            });

            var addfields2 = new BsonDocument("$addFields",
                        new BsonDocument("contenido.participantes.datosresidente.progresoactual",
                        new BsonDocument("$arrayElemAt",
                        new BsonArray
                                    {
                                        "$contenido.participantes.datosresidente.progreso",
                                        -1
                                    })));

            var project = new BsonDocument("$project",
                        new BsonDocument
                            {
                                { "_id", 1 },
                                { "titulo", 1 },
                                { "idcreador", 1 },
                                { "fechacreacion", 1 },
                                { "area", 1 },
                                { "contenido",
                        new BsonDocument("participantes",
                        new BsonDocument
                                    {
                                        { "idparticipante", 1 },
                                        { "grado", 1 },
                                        { "fecha", 1 },
                                        { "firma", 1 },
                                        { "observaciones", 1 },
                                        { "datosresidente",
                        new BsonDocument
                                        {
                                            { "nombre", 1 },
                                            { "apellido", 1 },
                                            { "tipodocumento", 1 },
                                            { "numerodocumento", 1 },
                                            { "progresoactual",
                                                new BsonDocument
                                                {
                                                    { "fase", 1 },
                                                    { "nombre", 1 }
                                                } 
                                            }
                                        } }
                                    }) },
                                { "tipo", 1 }
                            });

            var group = new BsonDocument("$group",
                        new BsonDocument
                            {
                                { "_id", "$_id" },
                                { "titulo",
                        new BsonDocument("$first", "$titulo") },
                                { "idcreador",
                        new BsonDocument("$first", "$idcreador") },
                                { "fechacreacion",
                        new BsonDocument("$first", "$fechacreacion") },
                                { "area",
                        new BsonDocument("$first", "$area") },
                                { "contenido",
                        new BsonDocument("$first", "$contenido") },
                                { "participantes",
                        new BsonDocument("$addToSet", "$contenido.participantes") },
                                { "tipo",
                        new BsonDocument("$first", "$tipo") }
                            });

            var project2 = new BsonDocument("$project",
                        new BsonDocument
                            {
                                { "_id", 1 },
                                { "titulo", 1 },
                                { "idcreador", 1 },
                                { "fechacreacion", 1 },
                                { "area", 1 },
                                { "contenido",
                        new BsonDocument("participantes", "$participantes") },
                                { "tipo", 1 }
                            });

            var sort = new BsonDocument("$sort",
                        new BsonDocument("_id", 1));

            sesionEducativaDTO = await _sesioneducativa.Aggregate()
                                .AppendStage<dynamic>(match)
                                .AppendStage<dynamic>(unwind)
                                .AppendStage<dynamic>(addfields)
                                .AppendStage<dynamic>(lookup)
                                .AppendStage<dynamic>(unwind2)
                                .AppendStage<dynamic>(addfields2)
                                .AppendStage<dynamic>(project)
                                .AppendStage<dynamic>(group)
                                .AppendStage<dynamic>(project2)
                                .AppendStage<SesionEducativaDTO>(sort)
                                .FirstAsync();
            return sesionEducativaDTO;

        }

        //Trae una sesion educativas segun su id
        public SesionEducativa GetById(string id)
        {
            SesionEducativa sesionedu = new SesionEducativa();
            sesionedu = _sesioneducativa.Find(sesionedu => sesionedu.id == id).FirstOrDefault();
            return sesionedu;
        }

        public async Task<SesionEducativa> CreateSesionEducativa(SesionEducativa sesioneducativa)
        { 
           sesioneducativa.fechaCreacion = DateTime.UtcNow.AddHours(-5);
            _sesioneducativa.InsertOne(sesioneducativa);
            return sesioneducativa;
        }

        public async Task<SesionEducativa> ModifySesionEducativa(SesionEducativa sesioneducativa)
        {
            var filter = Builders<SesionEducativa>.Filter.Eq("id", sesioneducativa.id);
            var update = Builders<SesionEducativa>.Update
                .Set("titulo", sesioneducativa.titulo)
                .Set("idcreador", sesioneducativa.idCreador)
                .Set("fechacreacion", sesioneducativa.fechaCreacion)
                .Set("area", sesioneducativa.area)
                .Set("contenido", sesioneducativa.contenido)
                .Set("tipo", sesioneducativa.tipo);
            _sesioneducativa.UpdateOne(filter, update);
            return sesioneducativa;
        }
    }
}
