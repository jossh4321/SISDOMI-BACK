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
        private readonly RolService rolService;
        private readonly FaseService faseService;

        public PlanIntervencionIndividualService(ISysdomiDatabaseSettings settings, ExpedienteService expedienteService, IDocument document, RolService rolService, FaseService faseService)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _documentos = database.GetCollection<Documento>("documentos");

            this.expedienteService = expedienteService;
            this.rolService = rolService;
            this.faseService = faseService;
            this.document = document;
        }

        //Sebastian
        public async Task<List<PlanIntervencionDTO>> GetAll()
        {
            List<PlanIntervencionDTO> listPlanIntervencionIndividuals;

            //Esto si solo se quiere la información de una colección que usa polimorfismo y que quiere un tipo de clase específica
           // listPlanIntervencionIndividuals =_documentos.AsQueryable().OfType<PlanIntervencionIndividual>().ToList();

            //Para obtener un tipo específico usando el match
            var matchPlan = new BsonDocument("$match",
                                             new BsonDocument("$expr",
                                                               new BsonDocument("$or",
                                                               new BsonArray 
                                                               {
                                                                   new BsonDocument("$eq",
                                                                   new BsonArray {
                                                                       "$tipo",
                                                                       "PlanIntervencionIndividualEducativo"
                                                                   }),
                                                                   new BsonDocument("$eq",
                                                                   new BsonArray {
                                                                       "$tipo",
                                                                       "PlanIntervencionIndividualSocial"
                                                                   }),
                                                                   new BsonDocument("$eq",
                                                                   new BsonArray {
                                                                       "$tipo",
                                                                       "PlanIntervencionIndividualPsicologico"
                                                                   })
                                                               })));

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

        //General
        public async Task<PlanIntervencionConsultaDTO> GetPlanById(String id)
        {
            PlanIntervencionConsultaDTO planIntervencionConsultaDTO;

            var matchId = new BsonDocument("$match",
                                           new BsonDocument("$expr", 
                                                    new BsonDocument("$eq", new BsonArray
                                                    {
                                                        "$_id",
                                                        new BsonDocument("$toObjectId", id)
                                                    })));


            var projectGeneralDataPlan = new BsonDocument("$project", new BsonDocument
            {
                { "tipo", 1},
                { "creadordocumento", 1 },
                {"idresidente", 1 },
                { "area", 1  },
                { "fase", 1  },
                { "estado", 1  },
                { "contenido", 1 }
            });


            var lookupResidente = new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "residentes" },
                { "let", new BsonDocument("residenteId", "$idresidente") },
                { "pipeline", new BsonArray 
                              {
                                 new BsonDocument("$match",
                                 new BsonDocument("$expr",
                                 new BsonDocument("$eq",
                                 new BsonArray {
                                     "$_id",
                                     new BsonDocument("$toObjectId", "$$residenteId")
                                 })))  
                              } 
                },
                { "as", "residente" }
            });

            var unwindResidente = new BsonDocument("$unwind", new BsonDocument("path", "$residente"));

            var lookupUsuario = new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "usuarios"},
                { "let", new BsonDocument("usuarioId", "$creadordocumento") },
                { "pipeline", new BsonArray
                              {
                                 new BsonDocument("$match",
                                 new BsonDocument("$expr",
                                 new BsonDocument("$eq",
                                 new BsonArray {
                                     "$_id",
                                     new BsonDocument("$toObjectId", "$$usuarioId")
                                 })))
                              } 
                },
                { "as", "creador" }
            });

            var unwindUsuario = new BsonDocument("$unwind", new BsonDocument("path", "$creador"));

            var projectFinalData = new BsonDocument("$project", new BsonDocument
            {
                { "tipo", 1  },
                {"area", 1 },
                {"fase", 1 },
                {"estado", 1 },
                { "contenido", 1},
                { "residente", 1},
                { "creador", new BsonDocument("$toString", "$creador._id") }
                /*{ "creador", new BsonDocument("$concat", new BsonArray{ 
                                                            "$creador.datos.nombre",
                                                            " ",
                                                            "$creador.datos.apellido"
                                                         }) 
                }*/
            });

            var lookupDocumentosNotPlan = new BsonDocument("$lookup",
                                          new BsonDocument
                                          {
                                              { "from", "documentos" },
                                              { "let", 
                                                    new BsonDocument
                                                    {
                                                        { "residenteid", "$residente._id" },
                                                        { "fase", "$fase" },
                                                        { "area", "$area" }
                                                    }
                                              },
                                              { "pipeline",
                                                    new BsonArray
                                                    {
                                                        new BsonDocument("$match",
                                                        new BsonDocument("$expr",
                                                        new BsonDocument("$and",
                                                        new BsonArray
                                                        {
                                                            new BsonDocument("$eq",
                                                            new BsonArray
                                                            {
                                                                new BsonDocument("$toObjectId", "$idresidente"),
                                                                "$$residenteid"
                                                            }),
                                                            new BsonDocument("$eq",
                                                            new BsonArray
                                                            {
                                                                "$fase",
                                                                "$$fase"
                                                            }),
                                                            new BsonDocument("$eq",
                                                            new BsonArray
                                                            {
                                                                "$area",
                                                                "$$area"
                                                            }),
                                                            new BsonDocument("$not",
                                                            new BsonDocument("$in",
                                                            new BsonArray {
                                                                "$tipo",
                                                                new BsonArray
                                                                {
                                                                    "PlanIntervencionIndividualPsicologico",
                                                                    "PlanIntervencionIndividualSocial",
                                                                    "PlanIntervencionIndividualEducativo"
                                                                }
                                                            }))
                                                        })))
                                                    }
                                              },
                                              { "as", "documentos" }
                                          });

            var setDocumentos = new BsonDocument("$set",
                                new BsonDocument("documentos",
                                new BsonDocument("$size", "$documentos")));

            planIntervencionConsultaDTO = await _documentos.Aggregate()
                                                .AppendStage<dynamic>(matchId)
                                                .AppendStage<dynamic>(projectGeneralDataPlan)
                                                .AppendStage<dynamic>(lookupResidente)
                                                .AppendStage<dynamic>(unwindResidente)
                                                .AppendStage<dynamic>(lookupUsuario)
                                                .AppendStage<dynamic>(unwindUsuario)
                                                .AppendStage<dynamic>(projectFinalData)
                                                .AppendStage<dynamic>(lookupDocumentosNotPlan)
                                                .AppendStage<PlanIntervencionConsultaDTO>(setDocumentos)
                                                .FirstOrDefaultAsync();

            return planIntervencionConsultaDTO;
        }

        public async Task UpdatePlanState(PlanState planState)
        {
            var filter = Builders<Documento>.Filter.Eq("id", planState.idDocumento);

            var update = Builders<Documento>.Update.Set("estado", planState.estado);

            await _documentos.FindOneAndUpdateAsync<Documento>(filter, update);
        }


        // Plan de Intervención Educativo
        public async Task<PlanIntervencionIndividualEducativo> CreateIndividualInterventionPlan(PlanResidente planIntervencionIndividual)
        {
            planIntervencionIndividual.planintervencionindividual.fechacreacion = DateTime.UtcNow.AddHours(-5);
            DateTime DateNow = DateTime.UtcNow.AddHours(-5);

            Expediente expediente =await expedienteService.GetByResident(planIntervencionIndividual.idresidente);

            planIntervencionIndividual.planintervencionindividual.contenido.codigoDocumento = document.CreateCodeDocument(DateNow, planIntervencionIndividual.planintervencionindividual.tipo, expediente.documentos.Count + 1);

            //Rol rol = await rolService.Get(planIntervencionIndividual.planintervencionindividual.contenido.firmas.ElementAt(0).cargo);

            //planIntervencionIndividual.planintervencionindividual.contenido.firmas.ElementAt(0).cargo = rol.nombre;

            await _documentos.InsertOneAsync(planIntervencionIndividual.planintervencionindividual);

            DocumentoExpediente documentoExpediente = new DocumentoExpediente
            {
                iddocumento = planIntervencionIndividual.planintervencionindividual.id,
                tipo = planIntervencionIndividual.planintervencionindividual.tipo
            };

            await expedienteService.UpdateDocuments(documentoExpediente, expediente.id);
            Fase fase = faseService.ModifyStateForDocument(planIntervencionIndividual.planintervencionindividual.idresidente, planIntervencionIndividual.planintervencionindividual.fase, planIntervencionIndividual.planintervencionindividual.area, planIntervencionIndividual.planintervencionindividual.tipo);
            return planIntervencionIndividual.planintervencionindividual;
        }

        public async Task<PlanIntervencionIndividualEducativo> ModifyIndividualInterventionPlan(PlanEducativoUpdate planIntervencionIndividual)
        {
            var actualDocumento = await _documentos.Find(x => x.id == planIntervencionIndividual.id).FirstOrDefaultAsync();

            if (actualDocumento != null)
            {
                if (actualDocumento.idresidente != planIntervencionIndividual.idresidente)
                {
                    DocumentoExpediente documentoExpediente = new DocumentoExpediente
                    {
                        tipo = planIntervencionIndividual.tipo,
                        iddocumento = planIntervencionIndividual.id
                    };

                    await expedienteService.RevomePushDocumentos(documentoExpediente, actualDocumento.idresidente, planIntervencionIndividual.idresidente);
                }
            }

            var filter = Builders<Documento>.Filter.Eq("id", planIntervencionIndividual.id);
            var update = Builders<Documento>.Update
                .Set("estado", "modificado")
                .Set("idresidente", planIntervencionIndividual.idresidente)
                .Set("contenido", planIntervencionIndividual.contenido)
                .Set("historialcontenido", planIntervencionIndividual.historialcontenido);

            var doc = _documentos.FindOneAndUpdate<Documento>(filter, update, new FindOneAndUpdateOptions<Documento>
            {
                ReturnDocument = ReturnDocument.After
            });

            PlanIntervencionIndividualEducativo planIntervencionEducativo = doc as PlanIntervencionIndividualEducativo;
            return planIntervencionEducativo;
        }

        //Plan Intervencion Psicologica
        public async Task<PlanIntervencionIndividualPsicologico> CreatePsycologicalInterventionPlan(PlanResidentePsicologico planResidentePsicologico)
        {
            planResidentePsicologico.planIntervencionIndividualPsicologico.fechacreacion =  DateTime.UtcNow.AddHours(-5);
            DateTime DateNow = DateTime.UtcNow.AddHours(-5);

            Expediente expediente = await expedienteService.GetByResident(planResidentePsicologico.idresidente);

            planResidentePsicologico.planIntervencionIndividualPsicologico.contenido.codigoDocumento = document.CreateCodeDocument(DateNow, planResidentePsicologico.planIntervencionIndividualPsicologico.tipo, expediente.documentos.Count + 1);

            //Rol rol = await rolService.Get(planResidentePsicologico.planIntervencionIndividualPsicologico.contenido.firmas.ElementAt(0).cargo);

            //planResidentePsicologico.planIntervencionIndividualPsicologico.contenido.firmas.ElementAt(0).cargo = rol.nombre;

            await _documentos.InsertOneAsync(planResidentePsicologico.planIntervencionIndividualPsicologico);

            DocumentoExpediente documentoExpediente = new DocumentoExpediente
            {
                iddocumento = planResidentePsicologico.planIntervencionIndividualPsicologico.id,
                tipo = planResidentePsicologico.planIntervencionIndividualPsicologico.tipo
            };

            await expedienteService.UpdateDocuments(documentoExpediente, expediente.id);
            Fase fase = faseService.ModifyStateForDocument(planResidentePsicologico.planIntervencionIndividualPsicologico.idresidente, planResidentePsicologico.planIntervencionIndividualPsicologico.fase, planResidentePsicologico.planIntervencionIndividualPsicologico.area, planResidentePsicologico.planIntervencionIndividualPsicologico.tipo);
            return planResidentePsicologico.planIntervencionIndividualPsicologico;
        }

        public async Task<PlanIntervencionIndividualPsicologico> ModifyPsycologicalIndividualInterventionPlan(PlanPsicologoUpdate planIntervencionIndividual)
        {
            var actualDocumento = await _documentos.Find(x => x.id == planIntervencionIndividual.id).FirstOrDefaultAsync();

            if (actualDocumento != null)
            {
                if (actualDocumento.idresidente != planIntervencionIndividual.idresidente)
                {
                    DocumentoExpediente documentoExpediente = new DocumentoExpediente
                    {
                        tipo = planIntervencionIndividual.tipo,
                        iddocumento = planIntervencionIndividual.id
                    };

                    await expedienteService.RevomePushDocumentos(documentoExpediente, actualDocumento.idresidente, planIntervencionIndividual.idresidente);
                }
            }

            var filter = Builders<Documento>.Filter.Eq("id", planIntervencionIndividual.id);
            var update = Builders<Documento>.Update
                .Set("estado", "modificado")
                .Set("idresidente", planIntervencionIndividual.idresidente)
                .Set("contenido", planIntervencionIndividual.contenido)
                .Set("historialcontenido", planIntervencionIndividual.historialcontenido);

            var doc = _documentos.FindOneAndUpdate<Documento>(filter, update, new FindOneAndUpdateOptions<Documento>
            {
                ReturnDocument = ReturnDocument.After
            });

            PlanIntervencionIndividualPsicologico planIntervencionPsicologico = doc as PlanIntervencionIndividualPsicologico;
            return planIntervencionPsicologico;
        }

        //Plan Intervencion Social
        public async Task<PlanIntervencionIndividualSocial> CreateSocialInterventionPlan(PlanResidenteSocial planResidenteSocial)
        {
            planResidenteSocial.planIntervencionIndividualSocial.fechacreacion = DateTime.UtcNow.AddHours(-5);
            DateTime DateNow = DateTime.UtcNow.AddHours(-5);

            Expediente expediente = await expedienteService.GetByResident(planResidenteSocial.idresidente);

            planResidenteSocial.planIntervencionIndividualSocial.contenido.codigoDocumento = document.CreateCodeDocument(DateNow, planResidenteSocial.planIntervencionIndividualSocial.tipo, expediente.documentos.Count + 1);

            //Rol rol = await rolService.Get(planResidenteSocial.planIntervencionIndividualSocial.contenido.firmas.ElementAt(0).cargo);

            //planResidenteSocial.planIntervencionIndividualSocial.contenido.firmas.ElementAt(0).cargo = rol.nombre;

            await _documentos.InsertOneAsync(planResidenteSocial.planIntervencionIndividualSocial);

            DocumentoExpediente documentoExpediente = new DocumentoExpediente
            {
                iddocumento = planResidenteSocial.planIntervencionIndividualSocial.id,
                tipo = planResidenteSocial.planIntervencionIndividualSocial.tipo
            };

            await expedienteService.UpdateDocuments(documentoExpediente, expediente.id);
            Fase fase = faseService.ModifyStateForDocument(planResidenteSocial.planIntervencionIndividualSocial.idresidente, planResidenteSocial.planIntervencionIndividualSocial.fase, planResidenteSocial.planIntervencionIndividualSocial.area, planResidenteSocial.planIntervencionIndividualSocial.tipo);
            return planResidenteSocial.planIntervencionIndividualSocial;
        }

        public async Task<PlanIntervencionIndividualSocial> ModifySocialIndividualInterventionPlan(PlanSocialUpdate planIntervencionIndividual)
        {
            var actualDocumento = await _documentos.Find(x => x.id == planIntervencionIndividual.id).FirstOrDefaultAsync();

            if(actualDocumento != null)
            {
                if(actualDocumento.idresidente != planIntervencionIndividual.id)
                {
                    DocumentoExpediente documentoExpediente = new DocumentoExpediente
                    {
                        tipo = planIntervencionIndividual.tipo,
                        iddocumento = planIntervencionIndividual.id
                    };

                    await expedienteService.RevomePushDocumentos(documentoExpediente, actualDocumento.idresidente, planIntervencionIndividual.idresidente);

                }
            }

            var filter = Builders<Documento>.Filter.Eq("id", planIntervencionIndividual.id);
            var update = Builders<Documento>.Update
                .Set("estado", "modificado")
                .Set("idresidente", planIntervencionIndividual.idresidente)
                .Set("contenido", planIntervencionIndividual.contenido)
                .Set("historialcontenido", planIntervencionIndividual.historialcontenido);
            var doc = _documentos.FindOneAndUpdate<Documento>(filter, update, new FindOneAndUpdateOptions<Documento>
            {
                ReturnDocument = ReturnDocument.After
            });

            PlanIntervencionIndividualSocial planIntervencionSocial = doc as PlanIntervencionIndividualSocial;
            return planIntervencionSocial;
        }


    }
}
