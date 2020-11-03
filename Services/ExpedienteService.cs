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
    }
}
