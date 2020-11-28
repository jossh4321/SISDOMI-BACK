using MongoDB.Driver;
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
    }
}
