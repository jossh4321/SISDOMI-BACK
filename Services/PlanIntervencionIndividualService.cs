using MongoDB.Driver;
using SISDOMI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Services
{
    public class PlanIntervencionIndividualService
    {
        private readonly IMongoCollection<Documento> _documentos;
        
        public PlanIntervencionIndividualService(ISysdomiDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _documentos = database.GetCollection<Documento>("documentos");
        }

        //Sebastian
        public List<PlanIntervencionIndividual> GetAll()
        {
            List<PlanIntervencionIndividual> listPlanIntervencionIndividuals = new List<PlanIntervencionIndividual>();

            listPlanIntervencionIndividuals =_documentos.AsQueryable().OfType<PlanIntervencionIndividual>().ToList();

            return listPlanIntervencionIndividuals;
        }

        //Jaime
        public PlanIntervencionIndividual GetById(String id)
        {
            return new PlanIntervencionIndividual();
        }

        //Sebastian
        public PlanIntervencionIndividual CreaterIndividualInterventionPlan(PlanIntervencionIndividual planIntervencionIndividual)
        {
            return new PlanIntervencionIndividual();
        }

        //Fede
        public PlanIntervencionIndividual ModifyIndividualInterventionPlan(PlanIntervencionIndividual planIntervencionIndividual)
        {
            var filter = Builders<Documento>.Filter.Eq("id", planIntervencionIndividual.id);
            var update = Builders<Documento>.Update
                .Set("area", planIntervencionIndividual.area)
                .Set("creadordocumento", planIntervencionIndividual.creadordocumento)
                .Set("fase", planIntervencionIndividual.fase)
                .Set("fechacreacion", planIntervencionIndividual.fechacreacion)
                .Set("contenido",planIntervencionIndividual.contenido)
                .Set("historialcontenido", planIntervencionIndividual.historialcontenido);
            var doc = _documentos.FindOneAndUpdate<Documento>(filter, update, new FindOneAndUpdateOptions<Documento>
            {
                ReturnDocument = ReturnDocument.After
            });
            planIntervencionIndividual = doc as PlanIntervencionIndividual;
            return planIntervencionIndividual;
        }

        // Angello
        public PlanIntervencionIndividual ModifyIndividualInterventionPlanState(PlanIntervencionIndividual planIntervencionIndividual)
        {
            return new PlanIntervencionIndividual();
        }

    }
}
