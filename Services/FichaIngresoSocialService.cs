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
        public List<FichaIngresoSocial> GetAll()
        {
            List<FichaIngresoSocial> listFichaIngresoSocial = new List<FichaIngresoSocial>();

            listFichaIngresoSocial = _documentos.AsQueryable().OfType<FichaIngresoSocial>().ToList();

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
        public FichaIngresoSocial ModifyFichaIngresoSocial(FichaIngresoSocial fichaIngresoSocial)
        {
            var filter = Builders<Documento>.Filter.Eq("id", fichaIngresoSocial.id);
            var update = Builders<Documento>.Update
                .Set("historialcontenido", fichaIngresoSocial.historialcontenido)
                .Set("creadordocumento", fichaIngresoSocial.creadordocumento)
                .Set("fechacreacion", fichaIngresoSocial.fechacreacion)
                .Set("area", fichaIngresoSocial.area)
                 .Set("fase", fichaIngresoSocial.fase)
                .Set("contenido", fichaIngresoSocial.contenido);
            var doc =_documentos.FindOneAndUpdate<Documento>(filter, update, new FindOneAndUpdateOptions<Documento>
            {
                ReturnDocument = ReturnDocument.After
            });
            fichaIngresoSocial = doc as FichaIngresoSocial;
            return fichaIngresoSocial;
        }


    }
}
