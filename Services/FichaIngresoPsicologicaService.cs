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
        public FichaIngresoPsicologicaService(ISysdomiDatabaseSettings settings, ExpedienteService expedienteService, IDocument document)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _documentos = database.GetCollection<Documento>("documentos");

            this.expedienteService = expedienteService;
            this.document = document;
        }
        public List<FichaIngresoPsicologica> GetAll()
        {
            List<FichaIngresoPsicologica> listFichaIngresoPsicologica = new List<FichaIngresoPsicologica>();

            listFichaIngresoPsicologica = _documentos.AsQueryable().OfType<FichaIngresoPsicologica>().ToList();

            return listFichaIngresoPsicologica;
        }
        public FichaIngresoPsicologica CreateFichaIngresoPsicologica(FichaIngresoPsicologica documento)
        {
            _documentos.InsertOne(documento);
            return documento;
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
