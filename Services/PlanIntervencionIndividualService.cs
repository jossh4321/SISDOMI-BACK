using MongoDB.Driver;
using SISDOMI.Entities;
using SISDOMI.Helpers;
using SISDOMI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Services
{
    public class PlanIntervencionIndividualService
    {
        private readonly IMongoCollection<Documento> _documentos;
        private readonly ExpedienteService expedienteService;
        private readonly IDocument document;
        
        public PlanIntervencionIndividualService(ISysdomiDatabaseSettings settings, ExpedienteService expedienteService, IDocument document)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _documentos = database.GetCollection<Documento>("documentos");

            this.expedienteService = expedienteService;
            this.document = document;
        }

        //Sebastian
        public List<PlanIntervencionIndividual> GetAll()
        {
            List<PlanIntervencionIndividual> listPlanIntervencionIndividuals;

            listPlanIntervencionIndividuals =_documentos.AsQueryable().OfType<PlanIntervencionIndividual>().ToList();

            return listPlanIntervencionIndividuals;
        }

        //Jaime
        public PlanIntervencionIndividual GetById(String id)
        {
            return new PlanIntervencionIndividual();
        }

        //Sebastian
        public async Task<PlanIntervencionIndividual> CreateIndividualInterventionPlan(PlanResidente planIntervencionIndividual)
        {
            DateTime DateNow = DateTime.UtcNow.AddHours(-5);

            Expediente expediente =await expedienteService.GetByResident(planIntervencionIndividual.idresidente);

            planIntervencionIndividual.planintervencionindividual.contenido.codigodocumento = document.CreateCodeDocument(DateNow, planIntervencionIndividual.planintervencionindividual.tipo, expediente.documentos.Count + 1);

            await _documentos.InsertOneAsync(planIntervencionIndividual.planintervencionindividual);

            DocumentoExpediente documentoExpediente = new DocumentoExpediente
            {
                iddocumento = planIntervencionIndividual.planintervencionindividual.id,
                tipo = planIntervencionIndividual.planintervencionindividual.tipo
            };

            await expedienteService.UpdateDocuments(documentoExpediente, expediente.id);

            return planIntervencionIndividual.planintervencionindividual;
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
                .Set("contenido", planIntervencionIndividual.contenido)
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
