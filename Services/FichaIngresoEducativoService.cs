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
    public class FichaIngresoEducativoService
    {
        private readonly IMongoCollection<Documento> _documentos;
        private readonly IMongoCollection<Expediente> _expedientes;
        private readonly ExpedienteService expedienteService;
        private readonly FichaIngresoSocialService fichaIngresoSocialService;
        private readonly FaseService faseService;
        private readonly IDocument document;

        public FichaIngresoEducativoService(ISysdomiDatabaseSettings settings, 
            ExpedienteService expedienteService, IDocument document,
            FichaIngresoSocialService fichaIngresoSocialService,
            FaseService faseService)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _documentos = database.GetCollection<Documento>("documentos");
            _expedientes = database.GetCollection<Expediente>("expedientes");

            this.expedienteService = expedienteService;
            this.fichaIngresoSocialService = fichaIngresoSocialService;
            this.faseService = faseService;
            this.document = document;
        }
        public List<FichaIngresoEducativa> GetAll()
        {
            List<FichaIngresoEducativa> listFichaIngresoEducativa = new List<FichaIngresoEducativa>();

            listFichaIngresoEducativa = _documentos.AsQueryable().OfType<FichaIngresoEducativa>().ToList();

            return listFichaIngresoEducativa;
        }
        public Object GetFichaIngresoDTO2PorId(string id)
        {
            Object fichaIngreso = new Object();
            fichaIngreso =  _documentos.Find(x => x.id == id).FirstOrDefault();
            return fichaIngreso;
        }
        public async  Task<FichaIngresoDTO> CreateFichaIngresoEducativo(FichaIngresoEducativa documento)
        {
            documento.fechacreacion = DateTime.UtcNow.AddHours(-5);
            DateTime DateNow = DateTime.UtcNow.AddHours(-5);
            Expediente expediente = await expedienteService.GetByResident(documento.idresidente);
            documento.contenido.codigoDocumento = document.CreateCodeDocument(DateNow, documento.tipo, expediente.documentos.Count + 1);
            await _documentos.InsertOneAsync(documento);
            FichaIngresoDTO fichaIngresoEducativa =  await fichaIngresoSocialService.obtenerResidienteFichaIngreso(documento.id);

            DocumentoExpediente docexpe = new DocumentoExpediente()
            {
                tipo = documento.tipo,
                iddocumento = documento.id
            };

            UpdateDefinition<Expediente> updateExpediente = Builders<Expediente>.Update.Push("documentos", docexpe);
            _expedientes.FindOneAndUpdate(x => x.idresidente == documento.idresidente, updateExpediente);

            Fase fase = faseService.ModifyStateForDocument(documento.idresidente, documento.fase, documento.area, documento.tipo);
            return fichaIngresoEducativa;
        }
        public FichaIngresoEducativa GetById(string id)
        {
            FichaIngresoEducativa documento = new FichaIngresoEducativa();
            documento = _documentos.AsQueryable().OfType<FichaIngresoEducativa>().ToList().Find(documento  => documento.id == id);
            return documento;
          
        }
        public async Task<FichaIngresoDTO> ModifyFichaIngresoEducativa(FichaIngresoEducativa documento)
        {
            FichaIngresoDTO fichaIngresoDTO = new FichaIngresoDTO();
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
            documento = doc as FichaIngresoEducativa;
            fichaIngresoDTO = await fichaIngresoSocialService.obtenerResidienteFichaIngreso(documento.id);
            return fichaIngresoDTO;
        }

        public FichaIngresoEducativa GetByResidenteId(string idResidente)
        {
            FichaIngresoEducativa documento = new FichaIngresoEducativa();
            documento = _documentos.AsQueryable().OfType<FichaIngresoEducativa>().ToList().Find(documento => documento.idresidente == idResidente);
            return documento;
        }


    }
}
