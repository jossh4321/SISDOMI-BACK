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
    public class FichaIngresoPsicologicaService
    {
        private readonly IMongoCollection<Documento> _documentos;
        private readonly ExpedienteService expedienteService;
        private readonly IDocument document;
        private readonly FichaIngresoSocialService fichaIngresoSocialService;
        public FichaIngresoPsicologicaService(ISysdomiDatabaseSettings settings, FichaIngresoSocialService fichaIngresoSocialService, ExpedienteService expedienteService, IDocument document)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _documentos = database.GetCollection<Documento>("documentos");

            this.expedienteService = expedienteService;
            this.document = document;
            this.fichaIngresoSocialService = fichaIngresoSocialService;
        }
        public List<FichaIngresoPsicologica> GetAll()
        {
            List<FichaIngresoPsicologica> listFichaIngresoPsicologica = new List<FichaIngresoPsicologica>();

            listFichaIngresoPsicologica = _documentos.AsQueryable().OfType<FichaIngresoPsicologica>().ToList();

            return listFichaIngresoPsicologica;
        }
        public async Task<FichaIngresoDTO> CreateFichaIngresoPsicologica(FichaIngresoPsicologica documento)
        {
            DateTime DateNow = DateTime.UtcNow.AddHours(-5);
            Expediente expediente = await expedienteService.GetByResident(documento.idresidente);
            documento.contenido.codigodocumento = document.CreateCodeDocument(DateNow, documento.tipo, expediente.documentos.Count + 1);
            await _documentos.InsertOneAsync(documento);
            DocumentoExpediente docexpe = new DocumentoExpediente()
            {
                tipo = documento.tipo,
                iddocumento = documento.id
            };
            await expedienteService.UpdateDocuments(docexpe, expediente.id);
            FichaIngresoDTO fichaIngreso = await fichaIngresoSocialService.obtenerResidienteFichaIngreso(documento.id);
            //Fase fase = faseService.ModifyStateForDocument(documento.idresidente, documento.fase, documento.area, documento.tipo); cuando se añada a la fase
            return fichaIngreso; 
        }
        public FichaIngresoPsicologica GetById(string id)
        {
            FichaIngresoPsicologica documento = new FichaIngresoPsicologica();
            documento = _documentos.AsQueryable().OfType<FichaIngresoPsicologica>().ToList().Find(documento => documento.id == id);
            return documento;
        }
        public FichaIngresoPsicologica  ModifyFichaIngresoPsicologica(FichaIngresoPsicologica documento)
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
            documento = doc as FichaIngresoPsicologica;
            return documento;
        }
    }
}
