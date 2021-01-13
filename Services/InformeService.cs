﻿using MongoDB.Bson;
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
    public class InformeService
    {
        private readonly IMongoCollection<Documento> _documentos;
        private readonly IMongoCollection<Expediente> _expedientes;
        private readonly ExpedienteService expedienteService;
        private readonly FaseService faseService;
        private readonly IDocument document;

        public InformeService(ISysdomiDatabaseSettings settings, IDocument document, ExpedienteService expedienteService, FaseService faseService)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _documentos = database.GetCollection<Documento>("documentos");
            _expedientes = database.GetCollection<Expediente>("expedientes");
            this.expedienteService = expedienteService;
            this.faseService = faseService;
            this.document = document;
        }

        public async Task<List<InformeDTO>> GetAllReportsDTO(String fromDate, String toDate)
        {
            var match = new BsonDocument("$match",
                        new BsonDocument("tipo",
                        new BsonDocument("$in",
                        new BsonArray
                        {
                        "InformeEducativoInicial",
                        "InformeEducativoEvolutivo",
                        "InformeEducativoFinal",
                        "InformeSocialInicial",
                        "InformeSocialEvolutivo",
                        "InformeSocialFinal",
                        "InformePsicologicoInicial",
                        "InformePsicologicoEvolutivo",
                        "InformePsicologicoFinal"
                        })));

            // Para realizar el filtro de los planes por fechas
            var addFieldDayYearMonth = new BsonDocument("$addFields",
                                       new BsonDocument
                                       {
                                           { "mes", new BsonDocument("$month", "$fechacreacion") },
                                           { "ano", new BsonDocument("$year", "$fechacreacion") },
                                           { "dia", new BsonDocument("$dayOfMonth", "$fechacreacion") }
                                       });

            //Se obtiene solamente la fecha sin los minutos ni los milisegundos
            var addFieldDate = new BsonDocument("$addFields",
                               new BsonDocument("fecha",
                               new BsonDocument("$toDate",
                               new BsonDocument("$concat",
                               new BsonArray
                               {
                                   new BsonDocument("$toString", "$ano"),
                                   "-",
                                   new BsonDocument("$toString", "$mes"),
                                   "-",
                                   new BsonDocument("$toString", "$dia")
                               }))));

            BsonValue fromDateTransform;
            BsonValue toDateTransform;

            if (fromDate != null)
            {
                fromDateTransform = fromDate;
            }
            else
            {
                fromDateTransform = BsonNull.Value;
            }

            if (toDate != null)
            {
                toDateTransform = toDate;
            }
            else
            {
                toDateTransform = BsonNull.Value;
            }

            //Obtener los planes donde solamente este entre las fechas consultadas
            var matchPlanesBetweenDate = new BsonDocument("$match",
                                         new BsonDocument("$expr",
                                         new BsonDocument("$and",
                                         new BsonArray
                                         {
                                             new BsonDocument("$or",
                                             new BsonArray
                                             {
                                                 new BsonDocument("$gte",
                                                 new BsonArray
                                                 {
                                                     "$fecha",
                                                     new BsonDocument("$toDate", fromDateTransform)
                                                 }),
                                                 new BsonDocument("$eq",
                                                 new BsonArray
                                                 {
                                                     fromDateTransform,
                                                     BsonNull.Value
                                                 })
                                             }),
                                             new BsonDocument("$or",
                                             new BsonArray
                                             {
                                                 new BsonDocument("$lte",
                                                 new BsonArray
                                                 {
                                                     "$fecha",
                                                     new BsonDocument("$toDate", toDateTransform)
                                                 }),
                                                 new BsonDocument("$eq",
                                                 new BsonArray
                                                 {
                                                     toDateTransform,
                                                     BsonNull.Value
                                                 })
                                             })
                                         }
                                         )));

            // Para eliminar las variables creadas para la consulta entre fechas
            var projectPlanNormal = new BsonDocument("$project", new BsonDocument
            {
                { "_id", 1},
                {"_t", 1 },
                { "tipo", 1 },
                { "historialcontenido", 1 },
                { "creadordocumento", 1 },
                { "fechacreacion", 1 },
                { "area", 1 },
                { "fase", 1 },
                { "idresidente", 1 },
                { "estado", 1 },
                { "contenido", 1 }

            });


            var addfield = new BsonDocument("$addFields",
                           new BsonDocument
                           {
                                { "idresidentepro",
                                new BsonDocument("$toObjectId", "$idresidente") },
                                { "codigodocumento", "$contenido.codigodocumento" }
                           });

            var lookup = new BsonDocument("$lookup",
                         new BsonDocument
                         {
                        { "from", "residentes" },
                        { "localField", "idresidentepro" },
                        { "foreignField", "_id" },
                        { "as", "datosresidente" }
                         });
            var unwind = new BsonDocument("$unwind",
                         new BsonDocument
                         {
                        { "path", "$datosresidente" },
                        { "preserveNullAndEmptyArrays", true }
                         });
            var project = new BsonDocument("$project",
                          new BsonDocument
                          {
                               { "_id", 1 },
                               { "_t", 1 },
                               { "tipo", 1 },
                               { "fechacreacion", 1 },
                               { "codigodocumento", 1 },
                               { "nombrecompleto",
                                    new BsonDocument("$concat",
                                        new BsonArray
                                        {
                                            "$datosresidente.nombre",
                                            " ",
                                            "$datosresidente.apellido"
                                        }) }
                          });

            List<InformeDTO> listainformes = new List<InformeDTO>();

            listainformes = await _documentos.Aggregate()
                            .AppendStage<dynamic>(match)
                            .AppendStage<dynamic>(addFieldDayYearMonth)
                            .AppendStage<dynamic>(addFieldDate)
                            .AppendStage<dynamic>(matchPlanesBetweenDate)
                            .AppendStage<dynamic>(projectPlanNormal)
                            .AppendStage<dynamic>(addfield)
                            .AppendStage<dynamic>(lookup)
                            .AppendStage<dynamic>(unwind)
                            .AppendStage<InformeDTO>(project)
                            .ToListAsync();
            return listainformes;
        }

        //Este metodo puede ser usado para traeer cualquier tipo de documento con su contenido
        public async Task<DocumentoDTO> GetById(string id)
        {
            var match = new BsonDocument("$match",
                        new BsonDocument("_id",
                        new ObjectId(id)));

            DocumentoDTO documento = new DocumentoDTO();
            documento = await _documentos.Aggregate()
                            .AppendStage<DocumentoDTO>(match)
                            .FirstAsync();
            return documento;
        }

        //Todos los registrar Informes

        public async Task<InformeEducativoInicial> RegistrarInformeEI(InformeEducativoInicial informe)
        {
            informe.fechacreacion = DateTime.UtcNow.AddHours(-5);
            DateTime DateNow = DateTime.UtcNow.AddHours(-5);
            Expediente expediente = await expedienteService.GetByResident(informe.idresidente);
            informe.contenido.codigodocumento = document.CreateCodeDocument(DateNow, informe.tipo, expediente.documentos.Count + 1);
            //informe.fechacreacion = DateNow;
            await _documentos.InsertOneAsync(informe);
            DocumentoExpediente docexpe = new DocumentoExpediente()
            {
                tipo = informe.tipo,
                iddocumento = informe.id
            };
            UpdateDefinition<Expediente> updateExpediente = Builders<Expediente>.Update.Push("documentos", docexpe);
            _expedientes.FindOneAndUpdate(x => x.idresidente == informe.idresidente, updateExpediente);
            Fase fase = faseService.ModifyStateForDocument(informe.idresidente, informe.fase, informe.area, informe.tipo);
            return informe;
        }
        public async Task<InformeEducativoEvolutivo> RegistrarInformeEE(InformeEducativoEvolutivo informe)
        {
            informe.fechacreacion = DateTime.UtcNow.AddHours(-5);
            DateTime DateNow = DateTime.UtcNow.AddHours(-5);
            Expediente expediente = await expedienteService.GetByResident(informe.idresidente);
            informe.contenido.codigodocumento = document.CreateCodeDocument(DateNow, informe.tipo, expediente.documentos.Count + 1);
            await _documentos.InsertOneAsync(informe);
            DocumentoExpediente docexpe = new DocumentoExpediente()
            {
                tipo = informe.tipo,
                iddocumento = informe.id
            };
            await expedienteService.UpdateDocuments(docexpe, expediente.id);
            Fase fase = faseService.ModifyStateForDocument(informe.idresidente, informe.fase, informe.area, informe.tipo);
            return informe;
        }
        public async Task<InformeSocialInicial> RegistrarInformeSI(InformeSocialInicial informe)
        {
            informe.fechacreacion = DateTime.UtcNow.AddHours(-5);
            DateTime DateNow = DateTime.UtcNow.AddHours(-5);
            Expediente expediente = await expedienteService.GetByResident(informe.idresidente);
            informe.contenido.codigodocumento = document.CreateCodeDocument(DateNow, informe.tipo, expediente.documentos.Count + 1);
            await _documentos.InsertOneAsync(informe);
            DocumentoExpediente docexpe = new DocumentoExpediente()
            {
                tipo = informe.tipo,
                iddocumento = informe.id
            };
            UpdateDefinition<Expediente> updateExpediente = Builders<Expediente>.Update.Push("documentos", docexpe);
            _expedientes.FindOneAndUpdate(x => x.idresidente == informe.idresidente, updateExpediente);
            Fase fase = faseService.ModifyStateForDocument(informe.idresidente, informe.fase, informe.area, informe.tipo);
            return informe;
        }
        public async Task<InformeSocialEvolutivo> RegistrarInformeSE(InformeSocialEvolutivo informe)
        {
            informe.fechacreacion = DateTime.UtcNow.AddHours(-5);
            DateTime DateNow = DateTime.UtcNow.AddHours(-5);
            Expediente expediente = await expedienteService.GetByResident(informe.idresidente);
            informe.contenido.codigodocumento = document.CreateCodeDocument(DateNow, informe.tipo, expediente.documentos.Count + 1);
            await _documentos.InsertOneAsync(informe);
            DocumentoExpediente docexpe = new DocumentoExpediente()
            {
                tipo = informe.tipo,
                iddocumento = informe.id
            };
            UpdateDefinition<Expediente> updateExpediente = Builders<Expediente>.Update.Push("documentos", docexpe);
            _expedientes.FindOneAndUpdate(x => x.idresidente == informe.idresidente, updateExpediente);
            Fase fase = faseService.ModifyStateForDocument(informe.idresidente, informe.fase, informe.area, informe.tipo);
            return informe;
        }

        //falta el PsicologicoInicial -> ya está nwn
        public async Task<InformePsicologicoInicial> RegistrarInformePI(InformePsicologicoInicial informe)
        {
            informe.fechacreacion = DateTime.UtcNow.AddHours(-5);
            DateTime DateNow = DateTime.UtcNow.AddHours(-5);
            Expediente expediente = await expedienteService.GetByResident(informe.idresidente);
            informe.contenido.codigodocumento = document.CreateCodeDocument(DateNow, informe.tipo, expediente.documentos.Count + 1);
            await _documentos.InsertOneAsync(informe);
            DocumentoExpediente docexpe = new DocumentoExpediente()
            {
                tipo = informe.tipo,
                iddocumento = informe.id
            };
            UpdateDefinition<Expediente> updateExpediente = Builders<Expediente>.Update.Push("documentos", docexpe);
            _expedientes.FindOneAndUpdate(x => x.idresidente == informe.idresidente, updateExpediente);
            Fase fase = faseService.ModifyStateForDocument(informe.idresidente, informe.fase, informe.area, informe.tipo);
            return informe;
        }

        public async Task<InformePsicologicoEvolutivo> RegistrarInformePE(InformePsicologicoEvolutivo informe)
        {
            informe.fechacreacion = DateTime.UtcNow.AddHours(-5);
            DateTime DateNow = DateTime.UtcNow.AddHours(-5);
            Expediente expediente = await expedienteService.GetByResident(informe.idresidente);
            informe.contenido.codigodocumento = document.CreateCodeDocument(DateNow, informe.tipo, expediente.documentos.Count + 1);
            await _documentos.InsertOneAsync(informe);
            DocumentoExpediente docexpe = new DocumentoExpediente()
            {
                tipo = informe.tipo,
                iddocumento = informe.id
            };
            UpdateDefinition<Expediente> updateExpediente = Builders<Expediente>.Update.Push("documentos", docexpe);
            _expedientes.FindOneAndUpdate(x => x.idresidente == informe.idresidente, updateExpediente);
            Fase fase = faseService.ModifyStateForDocument(informe.idresidente, informe.fase, informe.area, informe.tipo);
            return informe;
        }

        //Modificar Informes
        public async Task<InformeEducativoInicial> ModificarInformeEI(InformeEducativoInicial informe)
        {
            var actualDocumento = await _documentos.Find(x => x.id == informe.id).FirstOrDefaultAsync();

            if (actualDocumento != null)
            {
                if (actualDocumento.idresidente != informe.idresidente)
                {
                    DocumentoExpediente docexpe = new DocumentoExpediente()
                    {
                        tipo = informe.tipo,
                        iddocumento = informe.id
                    };
                    UpdateDefinition<Expediente> updateExpedienteD = Builders<Expediente>.Update.Pull("documentos", docexpe);
                    _expedientes.FindOneAndUpdate(x => x.idresidente == actualDocumento.idresidente, updateExpedienteD);

                    UpdateDefinition<Expediente> updateExpediente = Builders<Expediente>.Update.Push("documentos", docexpe);
                    _expedientes.FindOneAndUpdate(x => x.idresidente == informe.idresidente, updateExpediente);
                }
            }
            var filter = Builders<Documento>.Filter.Eq("id", informe.id);
            var update = Builders<Documento>.Update
                .Set("historialcontenido", informe.historialcontenido)
                .Set("idresidente", informe.idresidente)
                .Set("creadordocumento", informe.creadordocumento)
                .Set("fechacreacion", informe.fechacreacion)
                .Set("area", informe.area)
                .Set("fase", informe.fase)
                .Set("contenido", informe.contenido);

            _documentos.UpdateOne(filter, update);
            return informe;
        }
        public async Task<InformeEducativoEvolutivo> ModificarInformeEE(InformeEducativoEvolutivo informe)
        {
            var actualDocumento = await _documentos.Find(x => x.id == informe.id).FirstOrDefaultAsync();

            if (actualDocumento != null)
            {
                if (actualDocumento.idresidente != informe.idresidente)
                {
                    DocumentoExpediente docexpe = new DocumentoExpediente()
                    {
                        tipo = informe.tipo,
                        iddocumento = informe.id
                    };
                    UpdateDefinition<Expediente> updateExpedienteD = Builders<Expediente>.Update.Pull("documentos", docexpe);
                    _expedientes.FindOneAndUpdate(x => x.idresidente == actualDocumento.idresidente, updateExpedienteD);

                    UpdateDefinition<Expediente> updateExpediente = Builders<Expediente>.Update.Push("documentos", docexpe);
                    _expedientes.FindOneAndUpdate(x => x.idresidente == informe.idresidente, updateExpediente);
                }
            }
            var filter = Builders<Documento>.Filter.Eq("id", informe.id);
            var update = Builders<Documento>.Update
                .Set("historialcontenido", informe.historialcontenido)
                .Set("creadordocumento", informe.creadordocumento)
                .Set("idresidente", informe.idresidente)
                .Set("fechacreacion", informe.fechacreacion)
                .Set("area", informe.area)
                .Set("fase", informe.fase)
                .Set("contenido", informe.contenido);

            _documentos.UpdateOne(filter, update);
            return informe;
        }
        public async Task<InformeSocialInicial> ModificarInformeSI(InformeSocialInicial informe)
        {
            var actualDocumento = await _documentos.Find(x => x.id == informe.id).FirstOrDefaultAsync();

            if (actualDocumento != null)
            {
                if (actualDocumento.idresidente != informe.idresidente)
                {
                    DocumentoExpediente docexpe = new DocumentoExpediente()
                    {
                        tipo = informe.tipo,
                        iddocumento = informe.id
                    };
                    UpdateDefinition<Expediente> updateExpedienteD = Builders<Expediente>.Update.Pull("documentos", docexpe);
                    _expedientes.FindOneAndUpdate(x => x.idresidente == actualDocumento.idresidente, updateExpedienteD);

                    UpdateDefinition<Expediente> updateExpediente = Builders<Expediente>.Update.Push("documentos", docexpe);
                    _expedientes.FindOneAndUpdate(x => x.idresidente == informe.idresidente, updateExpediente);
                }
            }
            var filter = Builders<Documento>.Filter.Eq("id", informe.id);
            var update = Builders<Documento>.Update
                .Set("historialcontenido", informe.historialcontenido)
                .Set("idresidente", informe.idresidente)
                .Set("creadordocumento", informe.creadordocumento)
                .Set("fechacreacion", informe.fechacreacion)
                .Set("area", informe.area)
                .Set("fase", informe.fase)
                .Set("contenido", informe.contenido);

            _documentos.UpdateOne(filter, update);
            return informe;
        }
        public async Task<InformeSocialEvolutivo> ModificarInformeSE(InformeSocialEvolutivo informe)
        {
            var actualDocumento = await _documentos.Find(x => x.id == informe.id).FirstOrDefaultAsync();

            if (actualDocumento != null)
            {
                if (actualDocumento.idresidente != informe.idresidente)
                {
                    DocumentoExpediente docexpe = new DocumentoExpediente()
                    {
                        tipo = informe.tipo,
                        iddocumento = informe.id
                    };
                    UpdateDefinition<Expediente> updateExpedienteD = Builders<Expediente>.Update.Pull("documentos", docexpe);
                    _expedientes.FindOneAndUpdate(x => x.idresidente == actualDocumento.idresidente, updateExpedienteD);

                    UpdateDefinition<Expediente> updateExpediente = Builders<Expediente>.Update.Push("documentos", docexpe);
                    _expedientes.FindOneAndUpdate(x => x.idresidente == informe.idresidente, updateExpediente);
                }
            }
            var filter = Builders<Documento>.Filter.Eq("id", informe.id);
            var update = Builders<Documento>.Update
                .Set("historialcontenido", informe.historialcontenido)
                .Set("creadordocumento", informe.creadordocumento)
                .Set("idresidente", informe.idresidente)
                .Set("fechacreacion", informe.fechacreacion)
                .Set("area", informe.area)
                .Set("fase", informe.fase)
                .Set("contenido", informe.contenido);

            _documentos.UpdateOne(filter, update);
            return informe;
        }
        public async Task<InformePsicologicoInicial> ModificarInformePI(InformePsicologicoInicial informe)
        {
            var actualDocumento = await _documentos.Find(x => x.id == informe.id).FirstOrDefaultAsync();

            if (actualDocumento != null)
            {
                if (actualDocumento.idresidente != informe.idresidente)
                {
                    DocumentoExpediente docexpe = new DocumentoExpediente()
                    {
                        tipo = informe.tipo,
                        iddocumento = informe.id
                    };
                    UpdateDefinition<Expediente> updateExpedienteD = Builders<Expediente>.Update.Pull("documentos", docexpe);
                    _expedientes.FindOneAndUpdate(x => x.idresidente == actualDocumento.idresidente, updateExpedienteD);

                    UpdateDefinition<Expediente> updateExpediente = Builders<Expediente>.Update.Push("documentos", docexpe);
                    _expedientes.FindOneAndUpdate(x => x.idresidente == informe.idresidente, updateExpediente);
                }
            }
            var filter = Builders<Documento>.Filter.Eq("id", informe.id);
            var update = Builders<Documento>.Update
                .Set("historialcontenido", informe.historialcontenido)
                .Set("idresidente", informe.idresidente)
                .Set("creadordocumento", informe.creadordocumento)
                .Set("fechacreacion", informe.fechacreacion)
                .Set("area", informe.area)
                .Set("fase", informe.fase)
                .Set("contenido", informe.contenido);

            _documentos.UpdateOne(filter, update);
            return informe;
        }

        public async Task<InformePsicologicoEvolutivo> ModificarInformePE(InformePsicologicoEvolutivo informe)
        {
            var actualDocumento = await _documentos.Find(x => x.id == informe.id).FirstOrDefaultAsync();

            if (actualDocumento != null)
            {
                if (actualDocumento.idresidente != informe.idresidente)
                {
                    DocumentoExpediente docexpe = new DocumentoExpediente()
                    {
                        tipo = informe.tipo,
                        iddocumento = informe.id
                    };
                    UpdateDefinition<Expediente> updateExpedienteD = Builders<Expediente>.Update.Pull("documentos", docexpe);
                    _expedientes.FindOneAndUpdate(x => x.idresidente == actualDocumento.idresidente, updateExpedienteD);

                    UpdateDefinition<Expediente> updateExpediente = Builders<Expediente>.Update.Push("documentos", docexpe);
                    _expedientes.FindOneAndUpdate(x => x.idresidente == informe.idresidente, updateExpediente);
                }
            }
            var filter = Builders<Documento>.Filter.Eq("id", informe.id);
            var update = Builders<Documento>.Update
                .Set("historialcontenido", informe.historialcontenido)
                .Set("idresidente", informe.idresidente)
                .Set("creadordocumento", informe.creadordocumento)
                .Set("fechacreacion", informe.fechacreacion)
                .Set("area", informe.area)
                .Set("fase", informe.fase)
                .Set("contenido", informe.contenido);

            _documentos.UpdateOne(filter, update);
            return informe;
        }

        public async Task<Boolean> ComprobarDocumento (BuscarExpedienteDocumentoDTO documento)
        {
            var match = new BsonDocument("$match",
                            new BsonDocument{
                                { "idresidente", documento.idresidente },
                                { "documentos.tipo", documento.tipo }
                            });
            Expediente exp = new Expediente();
            exp = await _expedientes.Aggregate()
                .AppendStage<Expediente>(match)
                .FirstOrDefaultAsync();
            if (exp == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<AvanceSeguimiento> RegistrarAvanceSeguimiento(AvanceSeguimiento docAvance)
        {
            docAvance.fechacreacion = DateTime.UtcNow.AddHours(-5);
            DateTime DateNow = DateTime.UtcNow.AddHours(-5);
            Expediente expediente = await expedienteService.GetByResident(docAvance.idresidente);
            docAvance.contenido.codigodocumento = document.CreateCodeDocument(DateNow, docAvance.tipo, expediente.documentos.Count + 1);
            await _documentos.InsertOneAsync(docAvance);
            DocumentoExpediente docexpe = new DocumentoExpediente()
            {
                tipo = docAvance.tipo,
                iddocumento = docAvance.id
            };
            UpdateDefinition<Expediente> updateExpediente = Builders<Expediente>.Update.Push("documentos", docexpe);
            _expedientes.FindOneAndUpdate(x => x.idresidente == docAvance.idresidente, updateExpediente);
            Fase fase = faseService.ModifyStateForDocument(docAvance.idresidente, docAvance.fase, docAvance.area, docAvance.tipo);
            return docAvance;
        }
    }
}
