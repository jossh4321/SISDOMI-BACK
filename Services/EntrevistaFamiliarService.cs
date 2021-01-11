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
    public class EntrevistaFamiliarService
    {
        private readonly IMongoCollection<Documento> _documentos;
        private readonly IMongoCollection<Expediente> _expedientes;
        private readonly ExpedienteService expedienteService;
        private readonly IDocument document;

        public EntrevistaFamiliarService(ISysdomiDatabaseSettings settings,
            ExpedienteService expedienteService, IDocument document,
            FichaIngresoSocialService fichaIngresoSocialService,
            FaseService faseService)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _documentos = database.GetCollection<Documento>("documentos");
            _expedientes = database.GetCollection<Expediente>("expedientes");

            this.expedienteService = expedienteService;
            this.document = document;
        }
        public List<EntrevistaFamiliar> GetAll()
        {
            List<EntrevistaFamiliar> entrevista = new List<EntrevistaFamiliar>();
            entrevista = _documentos.AsQueryable().OfType<EntrevistaFamiliar>().ToList();
            return entrevista;
        }
        public EntrevistaFamiliar getByIdEntrevistaFamiliar(string id)
        {
            EntrevistaFamiliar entrevista = new EntrevistaFamiliar();
            entrevista = _documentos.AsQueryable().OfType<EntrevistaFamiliar>().ToList().Find(documento => documento.id == id);
            return entrevista;
        }
        public async Task<EntrevistaFamiliar> CreateEntrevistaFamiliar(EntrevistaFamiliar documento)
        {
            //Se registra el documento de entrevista familiar
            documento.fechacreacion = DateTime.UtcNow.AddHours(-5);
            await _documentos.InsertOneAsync(documento);

            //Se actualiza el expediente con el nuevo documento de la entrevista familiar
            Expediente expediente = await expedienteService.GetByResident(documento.idresidente);
            DocumentoExpediente docexpe = new DocumentoExpediente()
            {
                tipo = documento.tipo,
                iddocumento = documento.id
            };
            UpdateDefinition<Expediente> updateExpediente = Builders<Expediente>.Update.Push("documentos", docexpe);
            _expedientes.FindOneAndUpdate(x => x.idresidente == documento.idresidente, updateExpediente);

            return documento;
        }
        public EntrevistaFamiliar ModifyEntrevistaFamiliar(EntrevistaFamiliar documento)
        {
            var filter = Builders<Documento>.Filter.Eq("id", documento.id);
            var update = Builders<Documento>.Update
                .Set("tipo", documento.tipo)
                .Set("historialcontenido", documento.historialcontenido)
                .Set("creadordocumento", documento.creadordocumento)
                .Set("fechacreacion", documento.fechacreacion)
                .Set("area", documento.area)
                 .Set("fase", documento.fase)
                .Set("estado", documento.estado)
            .Set("contenido", documento.contenido);

            var doc = _documentos.FindOneAndUpdate<Documento>(filter, update, new FindOneAndUpdateOptions<Documento>
            {
                ReturnDocument = ReturnDocument.After
            });
            documento = doc as EntrevistaFamiliar;
            return documento;
        }
    }
}
