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
    public class ExpedienteService
    {
        private readonly IMongoCollection<Expediente> _expedientes;

        public ExpedienteService(ISysdomiDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _expedientes = database.GetCollection<Expediente>("expedientes");
        }

        public async Task<Expediente> GetByResident(String idresidente)
        {
            Expediente expediente;

            var filterId = Builders<Expediente>.Filter.Eq("idresidente", idresidente);

            expediente =await _expedientes.Find(filterId).FirstOrDefaultAsync();

            return expediente;
        }

        public async Task UpdateDocuments(DocumentoExpediente documentoExpediente, String idExpediente)
        {
            var filterExp = Builders<Expediente>.Filter.Eq("id", idExpediente);
            var pushDocumentsExp = Builders<Expediente>.Update.Push("documentos", documentoExpediente);

            await _expedientes.FindOneAndUpdateAsync(filterExp, pushDocumentsExp);

        }

        public async Task RevomePushDocumentos(DocumentoExpediente documentoExpediente, String idResidenteExpedienteRemove, String idResidenteExpedientePush)
        {
            var removefilterExp = Builders<Expediente>.Filter.Eq("idresidente", idResidenteExpedienteRemove);
            var removeDocumentsExp = Builders<Expediente>.Update.PullFilter("documentos",
                                                                            Builders<DocumentoExpediente>.Filter.Eq("iddocumento", documentoExpediente.iddocumento));

            var pushfilterExp = Builders<Expediente>.Filter.Eq("idresidente", idResidenteExpedientePush);
            var pushDocumentsExp = Builders<Expediente>.Update.Push("documentos", documentoExpediente);

            await _expedientes.UpdateOneAsync(removefilterExp, removeDocumentsExp);
            await _expedientes.UpdateOneAsync(pushfilterExp, pushDocumentsExp);
        }

        public async Task<List<ExpedienteDTO>> GetAll()
        {
            List<ExpedienteDTO> lstExpedienteDTOs;

            var projectExpediente = new BsonDocument("$project",
                                    new BsonDocument
                                    {
                                        { "numeroexpediente", 1 },
                                        { "idresidente", 1 },
                                        { "fechainicio", 1 }
                                    });

            var lookupResidente = new BsonDocument("$lookup",
                                  new BsonDocument
                                  {
                                      { "from", "residentes" },
                                      { "let", new BsonDocument("residenteid", "$idresidente") },
                                      { "pipeline",
                                        new BsonArray
                                        {
                                            new BsonDocument("$match",
                                            new BsonDocument("$expr",
                                            new BsonDocument("$eq", 
                                            new BsonArray 
                                            {
                                                "$_id",
                                                new BsonDocument("$toObjectId", "$$residenteid")
                                            }
                                            ))),
                                            new BsonDocument("$project",
                                            new BsonDocument
                                            {
                                                { "_id", 0 },
                                                { "residente",
                                                  new BsonDocument("$concat", 
                                                    new BsonArray
                                                        {
                                                            "$nombre",
                                                            " ",
                                                            "$apellido"
                                                        }
                                                    )
                                                }
                                            })
                                        }
                                      },
                                      { "as", "residente" }
                                  });

            var unwindResident = new BsonDocument("$unwind",
                                 new BsonDocument("path", "$residente"));


            var setResident = new BsonDocument("$set",
                              new BsonDocument("residente", "$residente.residente"));

            lstExpedienteDTOs = await _expedientes.Aggregate()
                                        .AppendStage<dynamic>(projectExpediente)
                                        .AppendStage<dynamic>(lookupResidente)
                                        .AppendStage<dynamic>(unwindResident)
                                        .AppendStage<ExpedienteDTO>(setResident)
                                        .ToListAsync();

            return lstExpedienteDTOs;
                                      
        }


        
    }
}
