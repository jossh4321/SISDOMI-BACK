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
        public async Task<List<PlanIntervencionDTO>> GetAll()
        {
            List<PlanIntervencionDTO> listPlanIntervencionIndividuals;

            //Esto si solo se quiere la información de una colección que usa polimorfismo y que quiere un tipo de clase específica
           // listPlanIntervencionIndividuals =_documentos.AsQueryable().OfType<PlanIntervencionIndividual>().ToList();

            //Para obtener un tipo específico usando el match
            var matchPlan = new BsonDocument("$match", new BsonDocument("tipo", "PlanIntervencionIndividual"));

            //Para obtener los datos del residente usando el lookup
            var lookupPlan = new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "residentes" },
                {"let", new BsonDocument("idResidente", "$idresidente") },
                { "pipeline", new BsonArray
                             {
                                new BsonDocument("$match", 
                                new BsonDocument("$expr",
                                new BsonDocument("$eq",
                                new BsonArray {
                                    "$_id",
                                    new BsonDocument("$toObjectId", "$$idResidente")
                                })))
                             } 
                },
                { "as", "residente" }
            });

            //Para cambiar el arrays de residentes por un objeto utilizando unwind
            var unwindPlan = new BsonDocument("$unwind", new BsonDocument("path", "$residente"));

            //Para solo enviar los datos que se necesitan utilizando project
            var projectPlan = new BsonDocument("$project", new BsonDocument
            {
                { "tipo", 1},
                {"fechacreacion", 1 },
                { "area", 1 },
                { "fase", 1 },
                { "estado", 1 },
                { "titulo", "$contenido.titulo" },
                { "residente", new BsonDocument("$concat",
                               new BsonArray{
                                   "$residente.nombre",
                                   " ",
                                   "$residente.apellido"
                               })
                }
            });


            listPlanIntervencionIndividuals = await _documentos.Aggregate()
                                                    .AppendStage<dynamic>(matchPlan)
                                                    .AppendStage<dynamic>(lookupPlan)
                                                    .AppendStage<dynamic>(unwindPlan)
                                                    .AppendStage<PlanIntervencionDTO>(projectPlan)
                                                    .ToListAsync();

            return listPlanIntervencionIndividuals;
        }

        //Angello xdd
        public PlanIntervencionIndividualEducativo GetById(String id)
        {
            PlanIntervencionIndividualEducativo planesI = new PlanIntervencionIndividualEducativo();
            planesI = _documentos.AsQueryable().OfType<PlanIntervencionIndividualEducativo>().ToList().Find(planesI => planesI.id == id);
            return planesI;
        }

        //Sebastian
        public async Task<PlanIntervencionIndividualEducativo> CreateIndividualInterventionPlan(PlanResidente planIntervencionIndividual)
        {
            DateTime DateNow = DateTime.UtcNow.AddHours(-5);

            Expediente expediente =await expedienteService.GetByResident(planIntervencionIndividual.idresidente);

            planIntervencionIndividual.planintervencionindividual.contenido.codigoDocumento = document.CreateCodeDocument(DateNow, planIntervencionIndividual.planintervencionindividual.tipo, expediente.documentos.Count + 1);

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
        public PlanIntervencionIndividualEducativo ModifyIndividualInterventionPlan(PlanIntervencionIndividualEducativo planIntervencionIndividual)
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

            planIntervencionIndividual = doc as PlanIntervencionIndividualEducativo;
            return planIntervencionIndividual;
        }

        // Jaime xd
        public PlanIntervencionIndividualEducativo ModifyIndividualInterventionPlanState(PlanIntervencionIndividualEducativo planIntervencionIndividual)
        {
            var filter = Builders<Documento>.Filter.Eq("id", planIntervencionIndividual.id);
            var update = Builders<Documento>.Update
               .Set("estado", planIntervencionIndividual.estado);
                
                

            _documentos.UpdateOne(filter, update);
            return new PlanIntervencionIndividualEducativo();
        }

        //Plan Intervencion Psicologica
        public async Task<PlanIntervencionIndividualPsicologico> CreatePsycologicalInterventionPlan(PlanResidentePsicologico planResidentePsicologico)
        {
            DateTime DateNow = DateTime.UtcNow.AddHours(-5);

            Expediente expediente = await expedienteService.GetByResident(planResidentePsicologico.idresidente);

            planResidentePsicologico.planIntervencionIndividualPsicologico.contenido.codigoDocumento = document.CreateCodeDocument(DateNow, planResidentePsicologico.planIntervencionIndividualPsicologico.tipo, expediente.documentos.Count);

            await _documentos.InsertOneAsync(planResidentePsicologico.planIntervencionIndividualPsicologico);

            DocumentoExpediente documentoExpediente = new DocumentoExpediente
            {
                iddocumento = planResidentePsicologico.planIntervencionIndividualPsicologico.id,
                tipo = planResidentePsicologico.planIntervencionIndividualPsicologico.tipo
            };

            await expedienteService.UpdateDocuments(documentoExpediente, expediente.id);

            return planResidentePsicologico.planIntervencionIndividualPsicologico;
        }


        //Plan Intervencion Social
        public async Task<PlanIntervencionIndividualSocial> CreateSocialInterventionPlan(PlanResidenteSocial planResidenteSocial)
        {
            DateTime DateNow = DateTime.UtcNow.AddHours(-5);

            Expediente expediente = await expedienteService.GetByResident(planResidenteSocial.idresidente);

            planResidenteSocial.planIntervencionIndividualSocial.contenido.codigoDocumento = document.CreateCodeDocument(DateNow, planResidenteSocial.planIntervencionIndividualSocial.tipo, expediente.documentos.Count);

            await _documentos.InsertOneAsync(planResidenteSocial.planIntervencionIndividualSocial);

            DocumentoExpediente documentoExpediente = new DocumentoExpediente
            {
                iddocumento = planResidenteSocial.planIntervencionIndividualSocial.id,
                tipo = planResidenteSocial.planIntervencionIndividualSocial.tipo
            };

            await expedienteService.UpdateDocuments(documentoExpediente, expediente.id);

            return planResidenteSocial.planIntervencionIndividualSocial;
        }
    }
}
