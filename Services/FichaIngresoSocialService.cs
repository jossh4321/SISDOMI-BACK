using MongoDB.Driver;
using SISDOMI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Services
{
    public class FichaIngresoSocialService
    {
        private readonly IMongoCollection<Documento> _documentos;

        public FichaIngresoSocialService(ISysdomiDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _documentos = database.GetCollection<Documento>("documentos");
        }
        public List<Documento> GetAll()
        {
            List<Documento> listFichaIngresoSocial = new List<Documento>();

            listFichaIngresoSocial = _documentos.AsQueryable().OfType<Documento>().ToList();

            return listFichaIngresoSocial;
        }
        public Documento CreateFichaIngresoSocial(Documento  documento)
        {
            _documentos.InsertOne(documento);
            return documento;
        }
        public Documento GetById(string id)
        {
            Documento documento = new Documento();
            documento = _documentos.Find(documento => documento.id == id).FirstOrDefault();
            return documento;
        }
        public Documento  ModifyFichaIngresoSocial(Documento documento)
        {
            var filter = Builders<Documento>.Filter.Eq("id", documento.id);
            var update = Builders<Documento>.Update
                .Set("tipo", documento.tipo)
                .Set("historialcontenido", documento.historialcontenido)
                .Set("creadordocumento", documento.creadordocumento)
                .Set("fechacreacion", documento.fechacreacion)
                .Set("area", documento.area)
                .Set("fase", documento.fase)
                .Set("estado", documento.estado);
               
            var doc =_documentos.FindOneAndUpdate<Documento>(filter, update, new FindOneAndUpdateOptions<Documento>
            {
                ReturnDocument = ReturnDocument.After
            });
            documento = doc as FichaIngresoSocial;
            return documento;
        }


    }
}
