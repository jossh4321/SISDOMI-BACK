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

    public class ActaDeExternamientoService
    {
        private readonly IMongoCollection<Documento> _documentos;
        private readonly ExpedienteService expedienteService;
        private readonly IDocument document;
        private readonly RolService rolService;

        public ActaDeExternamientoService(ISysdomiDatabaseSettings settings, ExpedienteService expedienteService, IDocument document)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _documentos = database.GetCollection<Documento>("documentos");

            this.expedienteService = expedienteService;
            this.document = document;
        }


        public async Task<List<ActaExternamientoDTO>> GetAll()
        {
            List<ActaExternamientoDTO> listActaExternamiento;

            var matchPlan = new BsonDocument("$match", new BsonDocument("tipo", "ActaExternamiento"));
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

            var unwindPlan = new BsonDocument("$unwind", new BsonDocument("path", "$residente"));
            var projectPlan = new BsonDocument("$project", new BsonDocument
            {
                { "tipo", 1},
                {"fechacreacion", 1 },
                {"historialcontenido", 1 },
                {"creadordocumento", 1 },
                {"idresidente", 1 },
                { "area", 1 },
                { "fase", 1 },
                { "estado", 1 },
                {"responsable","$contenido.responsable"},
                { "residente", new BsonDocument("$concat",
                               new BsonArray{
                                   "$residente.nombre",
                                   " ",
                                   "$residente.apellido"
                               })
                }

            });

            listActaExternamiento = await _documentos.Aggregate()
                                                    .AppendStage<dynamic>(matchPlan)
                                                    .AppendStage<dynamic>(lookupPlan)
                                                    .AppendStage<dynamic>(unwindPlan)
                                                    .AppendStage<ActaExternamientoDTO>(projectPlan)
                                                    .ToListAsync();

            return listActaExternamiento;
        }
        public async Task<Documento> GetById(string id)
        {
            Documento documento;
            var filterId = Builders<Documento>.Filter.Eq("id", id);
            documento = await _documentos.Find(filterId).FirstOrDefaultAsync();
            return documento;

        }

        public Documento Update(ActaExternamiento documento) 
        {

            var filterId = Builders<Documento>.Filter.Eq("id", documento.id);
            var update = Builders<Documento>.Update
                .Set("responsable", documento.contenido.responsable)
                .Set("estado",documento.estado)
                .Set("fase",documento.fase)
                .Set("estado","modificado")
                .Set("entidaddisposicion", documento.contenido.entidaddisposicion)
                ;
           var documentoUpdate = _documentos.FindOneAndUpdate<Documento>(filterId, update, new FindOneAndUpdateOptions<Documento>
            {
                ReturnDocument = ReturnDocument.After

            });
            return documentoUpdate;
        }

        public async Task<ActaExternamiento> Register(ActaExternamiento actaExternamiento)
        {
            DateTime dateTime = DateTime.UtcNow.AddHours(-5);

            Expediente expediente = await expedienteService.GetByResident(actaExternamiento.idresidente);

            actaExternamiento.creadordocumento = document.CreateCodeDocument(dateTime, actaExternamiento.tipo, expediente.documentos.Count + 1);

            //Rol rol = await rolService.Get(actaExternamiento.contenido.firmas.ElementAt(0).cargo);
            
            //actaExternamiento.contenido.firmas.ElementAt(0).cargo = rol.nombre;
            
            await _documentos.InsertOneAsync(actaExternamiento);

            DocumentoExpediente documentoExpediente = new DocumentoExpediente {
                iddocumento = actaExternamiento.id,
                tipo = actaExternamiento.tipo

            };

            await expedienteService.UpdateDocuments(documentoExpediente, expediente.id);

            return actaExternamiento;
        }
    }
}