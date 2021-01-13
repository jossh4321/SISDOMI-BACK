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
    public class EntrevistaFamiliarService
    {
        private readonly IMongoCollection<Documento> _documentos;
        private readonly IMongoCollection<Expediente> _expedientes;
        private readonly ExpedienteService expedienteService;
        private readonly IDocument document;

        public EntrevistaFamiliarService(ISysdomiDatabaseSettings settings,
            ExpedienteService expedienteService, IDocument document,
            FichaIngresoSocialService fichaIngresoSocialService,
            FaseService faseService)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _documentos = database.GetCollection<Documento>("documentos");
            _expedientes = database.GetCollection<Expediente>("expedientes");

            this.expedienteService = expedienteService;
            this.document = document;
        }
        public async Task<List<EntrevistaFamiliarDTO>> GetAll()
        {
            List<EntrevistaFamiliarDTO> entrevista = new List<EntrevistaFamiliarDTO>();
            var match = new BsonDocument("$match",
                        new BsonDocument("tipo",
                        new BsonDocument("$eq", "EntrevistaFamiliar")));

            var addfields = new BsonDocument("$addFields",
                            new BsonDocument("idresidentepro",
                            new BsonDocument("$toObjectId", "$idresidente")));

            var lookup = new BsonDocument("$lookup",
                         new BsonDocument
                         {
                            { "from", "residentes" },
                            { "localField", "idresidentepro" },
                            { "foreignField", "_id" },
                            { "as", "contenido.datosresidente" }
                         });
            var unwind = new BsonDocument("$unwind",
                         new BsonDocument("path", "$contenido.datosresidente"));

            var project = new BsonDocument("$project",
                          new BsonDocument
                          {
                            { "idresidentepro", 0 },
                            { "contenido",
                          new BsonDocument("datosresidente",
                          new BsonDocument
                            {
                                { "_id", 0 },
                                { "creadordocumento", 0 },
                                { "lugarnacimiento", 0 },
                                { "ubigeo", 0 },
                                { "juzgadoprocedencia", 0 },
                                { "fechanacimiento", 0 },
                                { "sexo", 0 },
                                { "motivoingreso", 0 },
                                { "estado", 0 },
                                { "fechaingreso", 0 },
                                { "telefonosreferencias", 0 },
                                { "progreso", 0 }
                            }) }
                          });

            entrevista = await _documentos.Aggregate()
                                .AppendStage<dynamic>(match)
                                .AppendStage<dynamic>(addfields)
                                .AppendStage<dynamic>(lookup)
                                .AppendStage<dynamic>(unwind)
                                .AppendStage<EntrevistaFamiliarDTO>(project)
                                .ToListAsync();
            return entrevista;
        }
        public async Task<EntrevistaFamiliarDTO> getByIdEntrevistaFamiliar(string id)
        {
            EntrevistaFamiliarDTO entrevista = new EntrevistaFamiliarDTO();
            var match = new BsonDocument("$match",
                        new BsonDocument("_id",
                        new ObjectId(id)));

            var addfields = new BsonDocument("$addFields",
                            new BsonDocument("idresidentepro",
                            new BsonDocument("$toObjectId", "$idresidente")));

            var lookup = new BsonDocument("$lookup",
                         new BsonDocument
                         {
                            { "from", "residentes" },
                            { "localField", "idresidentepro" },
                            { "foreignField", "_id" },
                            { "as", "contenido.datosresidente" }
                         });
            var unwind = new BsonDocument("$unwind",
                         new BsonDocument("path", "$contenido.datosresidente"));

            var project = new BsonDocument("$project",
                          new BsonDocument
                          {
                            { "idresidentepro", 0 },
                            { "contenido",
                          new BsonDocument("datosresidente",
                          new BsonDocument
                            {
                                { "_id", 0 },
                                { "creadordocumento", 0 },
                                { "lugarnacimiento", 0 },
                                { "ubigeo", 0 },
                                { "juzgadoprocedencia", 0 },
                                { "fechanacimiento", 0 },
                                { "sexo", 0 },
                                { "motivoingreso", 0 },
                                { "estado", 0 },
                                { "fechaingreso", 0 },
                                { "telefonosreferencias", 0 },
                                { "progreso", 0 }
                            }) }
                          });

            entrevista = await _documentos.Aggregate()
                                .AppendStage<dynamic>(match)
                                .AppendStage<dynamic>(addfields)
                                .AppendStage<dynamic>(lookup)
                                .AppendStage<dynamic>(unwind)
                                .AppendStage<EntrevistaFamiliarDTO>(project)
                                .FirstAsync();
            return entrevista;
        }
        public async Task<EntrevistaFamiliar> CreateEntrevistaFamiliar(EntrevistaFamiliar documento)
        {
            //Se registra el documento de entrevista familiar
            documento.fechacreacion = DateTime.UtcNow.AddHours(-5);
            await _documentos.InsertOneAsync(documento);

            //Se actualiza el expediente con el nuevo documento de la entrevista familiar
            Expediente expediente = await expedienteService.GetByResident(documento.idresidente);
            DocumentoExpediente docexpe = new DocumentoExpediente()
            {
                tipo = documento.tipo,
                iddocumento = documento.id
            };
            UpdateDefinition<Expediente> updateExpediente = Builders<Expediente>.Update.Push("documentos", docexpe);
            _expedientes.FindOneAndUpdate(x => x.idresidente == documento.idresidente, updateExpediente);

            return documento;
        }
        public EntrevistaFamiliar ModifyEntrevistaFamiliar(EntrevistaFamiliar documento)
        {
            var filter = Builders<Documento>.Filter.Eq("id", documento.id);
            var update = Builders<Documento>.Update
                .Set("tipo", documento.tipo)
                .Set("historialcontenido", documento.historialcontenido)
                .Set("creadordocumento", documento.creadordocumento)
                .Set("fechacreacion", documento.fechacreacion)
                .Set("area", documento.area)
                 .Set("fase", documento.fase)
                .Set("estado", documento.estado)
            .Set("contenido", documento.contenido);

            var doc = _documentos.FindOneAndUpdate<Documento>(filter, update, new FindOneAndUpdateOptions<Documento>
            {
                ReturnDocument = ReturnDocument.After
            });
            documento = doc as EntrevistaFamiliar;
            return documento;
        }
    }
}
