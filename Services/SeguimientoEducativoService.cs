using MongoDB.Driver;
using SISDOMI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using SISDOMI.DTOs;

namespace SISDOMI.Services
{
    public class SeguimientoEducativoService
    {
        private readonly IMongoCollection<Documento> _documentos;
        private readonly IMongoCollection<Expediente> _expedientes;
    
        public SeguimientoEducativoService(ISysdomiDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _documentos = database.GetCollection<Documento>("documentos");
            _expedientes = database.GetCollection<Expediente>("expedientes");
        }
        public async Task<List<SeguimientoDTO>> GetAll()
        {
            var match = new BsonDocument("$match",
                       new BsonDocument("tipo",
                       new BsonDocument("$in",
                       new BsonArray
                       {
                        "InformeSeguimientoEducativo"
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
                               { "estado", new BsonDocument("$concat",
                                        new BsonArray
                                        {"$datosresidente.estado"
                                        }
                                        ) },
                               { "codigodocumento",1},
                               { "nombrecompleto",
                                    new BsonDocument("$concat",
                                        new BsonArray
                                        {
                                            "$datosresidente.nombre",
                                            " ",
                                            "$datosresidente.apellido"
                                        }) }
                          });

            List<SeguimientoDTO> listainformes = new List<SeguimientoDTO>();

            listainformes = await _documentos.Aggregate()
                            .AppendStage<dynamic>(match)
                            .AppendStage<dynamic>(addfield)
                            .AppendStage<dynamic>(lookup)
                            .AppendStage<dynamic>(unwind)
                            .AppendStage<SeguimientoDTO>(project)
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
        public async Task<InformeSeguimientoEducativo> RegistrarInformeSE(InformeSeguimientoEducativo informe)
        {
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
        public async Task <InformeSeguimientoEducativo> ModificarInformeSE(InformeSeguimientoEducativo informe)
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
