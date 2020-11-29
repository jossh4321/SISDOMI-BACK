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
                    area = "Psicologica";
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
    }
}
