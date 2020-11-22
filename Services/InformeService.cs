using MongoDB.Bson;
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
        private readonly IDocument document;

        public InformeService(ISysdomiDatabaseSettings settings, IDocument document, ExpedienteService expedienteService)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _documentos = database.GetCollection<Documento>("documentos");
            _expedientes = database.GetCollection<Expediente>("expedientes");
            this.expedienteService = expedienteService;
            this.document = document;
        }

        public async Task<List<InformeDTO>> GetAllReportsDTO()
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
            return informe;
        }
        public async Task<InformeEducativoEvolutivo> RegistrarInformeEE(InformeEducativoEvolutivo informe)
        {
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
            return informe;
        }
        public async Task<InformeSocialInicial> RegistrarInformeSI(InformeSocialInicial informe)
        {
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
            return informe;
        }
        public async Task<InformeSocialEvolutivo> RegistrarInformeSE(InformeSocialEvolutivo informe)
        {
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
            return informe;
        }

        //falta el PsicologicoInicial
        public async Task<InformePsicologicoEvolutivo> RegistrarInformePE(InformePsicologicoEvolutivo informe)
        {
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
            return informe;
        }

        //Modificar Informes
        public async Task<InformeEducativoInicial> ModificarInformeEI(InformeEducativoInicial informe)
        {
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
        public InformeSocialInicial ModificarInformeSI(InformeSocialInicial informe)
        {
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
        public InformeSocialEvolutivo ModificarInformeSE(InformeSocialEvolutivo informe)
        {
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
        public InformePsicologicoEvolutivo ModificarInformePE(InformePsicologicoEvolutivo informe)
        {
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
    }
}
