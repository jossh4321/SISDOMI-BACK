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
            return new List<PlanIntervencionIndividual>();
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
            return new PlanIntervencionIndividual();
        }

        // Angello
        public PlanIntervencionIndividual ModifyIndividualInterventionPlanState(PlanIntervencionIndividual planIntervencionIndividual)
        {
            return new PlanIntervencionIndividual();
        }

    }
}
