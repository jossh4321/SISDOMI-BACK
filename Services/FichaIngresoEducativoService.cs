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
        private readonly IDocument document;

        public FichaIngresoEducativoService(ISysdomiDatabaseSettings settings, ExpedienteService expedienteService, IDocument document)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _documentos = database.GetCollection<Documento>("documentos");

            this.expedienteService = expedienteService;
            this.document = document;
        }
        public List<FichaIngresoEducativa> GetAll()
        {
            List<FichaIngresoEducativa> listFichaIngresoEducativa = new List<FichaIngresoEducativa>();

            listFichaIngresoEducativa = _documentos.AsQueryable().OfType<FichaIngresoEducativa>().ToList();

            return listFichaIngresoEducativa;
        }
        public FichaIngresoEducativa CreateFichaIngresoEducativo(FichaIngresoEducativa documento)
        {
            _documentos.InsertOne(documento);
            return documento;
        }
        public FichaIngresoEducativa GetById(string id)
        {
            FichaIngresoEducativa documento = new FichaIngresoEducativa();
            documento = _documentos.AsQueryable().OfType<FichaIngresoEducativa>().ToList().Find(documento  => documento.id == id);
            return documento;
          
        }
        public FichaIngresoEducativa ModifyFichaIngresoEducativa(FichaIngresoEducativa documento)
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
            documento = doc as FichaIngresoEducativa;
            return documento;
        }

        public FichaIngresoEducativa GetByResidenteId(string idResidente)
        {
            FichaIngresoEducativa documento = new FichaIngresoEducativa();
            documento = _documentos.AsQueryable().OfType<FichaIngresoEducativa>().ToList().Find(documento => documento.idresidente == idResidente);
            return documento;
        }


    }
}
