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
        private readonly ExpedienteService expedienteService;
        private readonly FichaIngresoSocialService fichaIngresoSocialService;
        private readonly IDocument document;

        public FichaIngresoEducativoService(ISysdomiDatabaseSettings settings, 
            ExpedienteService expedienteService, IDocument document,
            FichaIngresoSocialService fichaIngresoSocialService)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _documentos = database.GetCollection<Documento>("documentos");

            this.expedienteService = expedienteService;
            this.fichaIngresoSocialService = fichaIngresoSocialService;
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
            DateTime DateNow = DateTime.UtcNow.AddHours(-5);
            Expediente expediente = await expedienteService.GetByResident(documento.idresidente);
            documento.contenido.codigoDocumento = document.CreateCodeDocument(DateNow, documento.tipo, expediente.documentos.Count + 1);
            await _documentos.InsertOneAsync(documento);
            FichaIngresoDTO fichaIngresoEducativa =  await fichaIngresoSocialService.obtenerResidienteFichaIngreso(documento.id);
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
