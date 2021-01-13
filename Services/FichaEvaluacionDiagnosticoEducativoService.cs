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

namespace SISDOMI.Services {
    public class FichaEvaluacionDiagnosticoEducativoService {
        private readonly IMongoCollection<Documento> _documentos;
        private readonly IMongoCollection<Expediente> _expedientes;

        public FichaEvaluacionDiagnosticoEducativoService(ISysdomiDatabaseSettings settings) {

            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _documentos = database.GetCollection<Documento>("documentos");
            _expedientes = database.GetCollection<Expediente>("expedientes");
        }

        public async Task<List<FichaEvaluacionDiagnosticoEducativoDTO>> GetAll(String fromDate, String toDate) {

            var match = new BsonDocument("$match",
                      new BsonDocument("tipo",
                      new BsonDocument("$in",
                      new BsonArray
                      {
                        "FichaEvaluacionDiagnosticoEducativo"
                      })));

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

            var addfield = new BsonDocument("$addFields",
                           new BsonDocument
                           {
                                { "idresidentepro",
                                new BsonDocument("$toObjectId", "$idresidente") },
                                { "codigodocumento", "$contenido.codigodocumento" }
                           });

            var lookup = new BsonDocument("$lookup",
                         new BsonDocument
                         {
                        { "from", "residentes" },
                        { "localField", "idresidentepro" },
                        { "foreignField", "_id" },
                        { "as", "datosresidente" }
                         });
            var unwind = new BsonDocument("$unwind",
                         new BsonDocument
                         {
                        { "path", "$datosresidente" },
                        { "preserveNullAndEmptyArrays", true }
                         });
            var project = new BsonDocument("$project",
                          new BsonDocument
                          {
                               { "_id", 1 },
                               { "_t", 1 },
                               { "tipo", 1 },
                               { "fechacreacion", 1 },
                               { "estado", new BsonDocument("$concat",
                                        new BsonArray
                                        {"$datosresidente.estado"
                                        }
                                        ) },
                               { "codigodocumento",1},
                               { "nombrecompleto",
                                    new BsonDocument("$concat",
                                        new BsonArray
                                        {
                                            "$datosresidente.nombre",
                                            " ",
                                            "$datosresidente.apellido"
                                        }) }
                          });

            List<FichaEvaluacionDiagnosticoEducativoDTO> listainformes = new List<FichaEvaluacionDiagnosticoEducativoDTO>();

            listainformes = await _documentos.Aggregate()
                            .AppendStage<dynamic>(match)
                            .AppendStage<dynamic>(addfield)
                            .AppendStage<dynamic>(lookup)
                            .AppendStage<dynamic>(unwind)
                            .AppendStage<FichaEvaluacionDiagnosticoEducativoDTO>(project)
                            .ToListAsync();
            return listainformes;
        }


    

    public async Task<DocumentoDTO> GetById(string id)
    {
        var match = new BsonDocument("$match",
                    new BsonDocument("_id",
                    new ObjectId(id)));

        DocumentoDTO documento = new DocumentoDTO();
        documento = await _documentos.Aggregate()
                        .AppendStage<DocumentoDTO>(match)
                        .FirstAsync();
        return documento;
    }

        public async Task<FichaEvaluacionDiagnosticoEducativo> RegistrarFichaEvaluacionDE(FichaEvaluacionDiagnosticoEducativo informe)
        {
            informe.fechacreacion = DateTime.UtcNow.AddHours(-5);
            await _documentos.InsertOneAsync(informe);
            DocumentoExpediente docexpe = new DocumentoExpediente()
            {
                tipo = informe.tipo,
                iddocumento = informe.id
            };
            UpdateDefinition<Expediente> updateExpediente = Builders<Expediente>.Update.Push("documentos", docexpe);
            _expedientes.FindOneAndUpdate(x => x.idresidente == informe.idresidente, updateExpediente);
            return informe;
        }

        public async Task<FichaEvaluacionDiagnosticoEducativo> ModificarFichaEvaluacionDE(FichaEvaluacionDiagnosticoEducativo informe)
        {
            var filter = Builders<Documento>.Filter.Eq("id", informe.id);
            var update = Builders<Documento>.Update
                .Set("historialcontenido", informe.historialcontenido)
                .Set("idresidente", informe.idresidente)
                .Set("creadordocumento", informe.creadordocumento)
                .Set("fechacreacion", informe.fechacreacion)
                .Set("area", informe.area)
                .Set("fase", informe.fase)
                .Set("contenido", informe.contenido);

            _documentos.UpdateOne(filter, update);
            return informe;
        }
    }
}
