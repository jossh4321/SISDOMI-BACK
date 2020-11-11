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
        public PlanIntervencionIndividual GetById(String id)
        {
            PlanIntervencionIndividual planesI = new PlanIntervencionIndividual();
            planesI = _documentos.AsQueryable().OfType<PlanIntervencionIndividual>().ToList().Find(planesI => planesI.id == id);
            return planesI;
        }

        //Sebastian
        public async Task<PlanIntervencionIndividual> CreateIndividualInterventionPlan(PlanResidente planIntervencionIndividual)
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

        // Jaime xd
        public PlanIntervencionIndividual ModifyIndividualInterventionPlanState(PlanIntervencionIndividual planIntervencionIndividual)
        {
            var filter = Builders<Documento>.Filter.Eq("id", planIntervencionIndividual.id);
            var update = Builders<Documento>.Update
               .Set("estado", planIntervencionIndividual.estado);
                
                

            _documentos.UpdateOne(filter, update);
            return new PlanIntervencionIndividual();
        }

    }
}
