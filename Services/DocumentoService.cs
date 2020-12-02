using MongoDB.Bson;
using MongoDB.Driver;
using SISDOMI.DTOs;
using SISDOMI.Entities;
using SISDOMI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Services
{
    public class DocumentoService
    {
        private readonly IMongoCollection<Documento> _documentos;
        
        public DocumentoService(ISysdomiDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _documentos = database.GetCollection<Documento>("documentos");

        }

        public async Task<List<DocumentTypeResidentDTO>> ListDocumentsByTypeAndResident(String type, String residenteid)
        {
            List<DocumentTypeResidentDTO> lstDocumentTypeResidentDTOs;
            String area = "";

            switch (type)
            {
                case "educativa":
                    area = "Educativo";
                    break;
                case "psicologica":
                    area = "Psicologico";
                    break;
                case "social":
                    area = "Social";
                    break;
                default:
                    break;
            }


            var matchDocuments = new BsonDocument("$match",
                                 new BsonDocument("$expr",
                                 new BsonDocument("$and",
                                       new BsonArray
                                       {
                                           new BsonDocument("$eq",
                                           new BsonArray
                                           {
                                               "$area",
                                               type
                                           }),
                                           new BsonDocument("$eq",
                                           new BsonArray
                                           {
                                               "$idresidente",
                                               residenteid
                                           }),
                                           new BsonDocument("$in",
                                           new BsonArray
                                           {
                                               "$tipo",
                                               new BsonArray
                                               {
                                                   "Informe" + area + "Inicial",
                                                   "Informe" + area + "Evolutivo",
                                                   "Informe" + area + "Final",
                                                   "PlanIntervencionIndividual"
                                               }
                                           })

                                       })));

            var groupDocuments = new BsonDocument("$group",
                                 new BsonDocument
                                 {
                                     { "_id", "$tipo" },
                                     { "documentos",
                                        new BsonDocument("$push",
                                        new BsonDocument("$toString", "$_id"))
                                     }
                                 });

            var projectDocuments = new BsonDocument("$project",
                                    new BsonDocument
                                    {
                                        { "_id", 0 },
                                        { "tipo", "$_id" },
                                        { "documentos", 1 }
                                    });

            lstDocumentTypeResidentDTOs = await _documentos.Aggregate()
                                                        .AppendStage<dynamic>(matchDocuments)
                                                        .AppendStage<dynamic>(groupDocuments)
                                                        .AppendStage<DocumentTypeResidentDTO>(projectDocuments)
                                                        .ToListAsync();
                                            
            return lstDocumentTypeResidentDTOs;
        }

        public async Task<DocumentoExpedienteDTO> GetById(String id)
        {
            DocumentoExpedienteDTO documentoExpedienteDTO;

            var matchId = new BsonDocument("$match",
                                           new BsonDocument("$expr",
                                                    new BsonDocument("$eq", new BsonArray
                                                    {
                                                        "$_id",
                                                        new BsonDocument("$toObjectId", id)
                                                    })));

            var projectGeneralDocument = new BsonDocument("$project", new BsonDocument
            {
                { "tipo", 1},
                { "creadordocumento", 1 },
                {"idresidente", 1 },
                { "area", 1  },
                { "fase", 1  },
                { "estado", 1  },
                { "contenido", 1 }
            });


            var lookupResidente = new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "residentes" },
                { "let", new BsonDocument("residenteId", "$idresidente") },
                { "pipeline", new BsonArray
                              {
                                 new BsonDocument("$match",
                                 new BsonDocument("$expr",
                                 new BsonDocument("$eq",
                                 new BsonArray {
                                     "$_id",
                                     new BsonDocument("$toObjectId", "$$residenteId")
                                 })))
                              }
                },
                { "as", "residente" }
            });

            var unwindResidente = new BsonDocument("$unwind", new BsonDocument("path", "$residente"));

            var lookupUsuario = new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "usuarios"},
                { "let", new BsonDocument("usuarioId", "$creadordocumento") },
                { "pipeline", new BsonArray
                              {
                                 new BsonDocument("$match",
                                 new BsonDocument("$expr",
                                 new BsonDocument("$eq",
                                 new BsonArray {
                                     "$_id",
                                     new BsonDocument("$toObjectId", "$$usuarioId")
                                 })))
                              }
                },
                { "as", "creador" }
            });

            var unwindUsuario = new BsonDocument("$unwind", new BsonDocument("path", "$creador"));

            var projectFinalData = new BsonDocument("$project", new BsonDocument
            {
                { "tipo", 1  },
                {"area", 1 },
                {"fase", 1 },
                {"estado", 1 },
                { "contenido", 1},
                { "residente", 1},
                { "creador", new BsonDocument("$concat", new BsonArray{
                                                            "$creador.datos.nombre",
                                                            " ",
                                                            "$creador.datos.apellido"
                                                         })
                }
            });

            documentoExpedienteDTO = await _documentos.Aggregate()
                                            .AppendStage<dynamic>(matchId)
                                            .AppendStage<dynamic>(projectGeneralDocument)
                                            .AppendStage<dynamic>(lookupResidente)
                                            .AppendStage<dynamic>(unwindResidente)
                                            .AppendStage<dynamic>(lookupUsuario)
                                            .AppendStage<dynamic>(unwindUsuario)
                                            .AppendStage<DocumentoExpedienteDTO>(projectFinalData)
                                            .FirstOrDefaultAsync();

            return documentoExpedienteDTO;
        }
    }
}
