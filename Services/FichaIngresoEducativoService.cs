using MongoDB.Driver;
using SISDOMI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Services
{
    public class FichaIngresoEducativoService
    {
        private readonly IMongoCollection<Documento> _documentos;

        public FichaIngresoEducativoService(ISysdomiDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _documentos = database.GetCollection<Documento>("documentos");
        }
        public List<FichaIngresoEducativa> GetAll()
        {
            List<FichaIngresoEducativa> listFichaIngresoEducativa = new List<FichaIngresoEducativa>();

            listFichaIngresoEducativa = _documentos.AsQueryable().OfType<FichaIngresoEducativa>().ToList();

            return listFichaIngresoEducativa;
        }
        public Documento CreateFichaIngresoEducativo(Documento documento)
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
        public FichaIngresoEducativa ModifyFichaIngresoEducativa(FichaIngresoEducativa fichaIngresoEducativa)
        {
            var filter = Builders<Documento>.Filter.Eq("id", fichaIngresoEducativa.id);
            var update = Builders<Documento>.Update
                .Set("historialcontenido", fichaIngresoEducativa.historialcontenido)
                .Set("creadordocumento", fichaIngresoEducativa.creadordocumento)
                .Set("fechacreacion", fichaIngresoEducativa.fechacreacion)
                .Set("area", fichaIngresoEducativa.area)
                 .Set("fase", fichaIngresoEducativa.fase)
                .Set("contenido", fichaIngresoEducativa.contenido);
            var doc = _documentos.FindOneAndUpdate<Documento>(filter, update, new FindOneAndUpdateOptions<Documento>
            {
                ReturnDocument = ReturnDocument.After
            });
            fichaIngresoEducativa = doc as FichaIngresoEducativa;
            return fichaIngresoEducativa;
        }


    }
}
