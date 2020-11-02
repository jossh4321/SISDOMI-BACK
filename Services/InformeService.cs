using MongoDB.Bson;
using MongoDB.Driver;
using SISDOMI.DTOs;
using SISDOMI.Entities;
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

        public InformeService(ISysdomiDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _documentos = database.GetCollection<Documento>("documentos");
            _expedientes = database.GetCollection<Expediente>("expedientes");
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
                                { "idresidente",
                                new BsonDocument("$toObjectId", "$contenido.idresidente") },
                                { "codigodocumento", "$contenido.codigodocumento" }
                           });

            var lookup = new BsonDocument("$lookup",
                         new BsonDocument
                         {
                        { "from", "residentes" },
                        { "localField", "idresidente" },
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
                               { "datosresidente.nombre", 1 },
                               {"datosresidente.apellido", 1 }
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
            await _documentos.InsertOneAsync(informe);
            DocumentoExpediente docexpe = new DocumentoExpediente()
            {
                tipo = informe.tipo,
                iddocumento = informe.id
            };
            UpdateDefinition<Expediente> updateExpediente = Builders<Expediente>.Update.Push("documentos", docexpe);
            _expedientes.FindOneAndUpdate(x => x.idresidente == informe.contenido.idresidente, updateExpediente);
            return informe;
        }
        public async Task<InformeEducativoEvolutivo> RegistrarInformeEE(InformeEducativoEvolutivo informe)
        {
            await _documentos.InsertOneAsync(informe);
            DocumentoExpediente docexpe = new DocumentoExpediente()
            {
                tipo = informe.tipo,
                iddocumento = informe.id
            };
            UpdateDefinition<Expediente> updateExpediente = Builders<Expediente>.Update.Push("documentos", docexpe);
            _expedientes.FindOneAndUpdate(x => x.idresidente == informe.contenido.idresidente, updateExpediente);
            return informe;
        }
        public async Task<InformeSocialInicial> RegistrarInformeSI(InformeSocialInicial informe)
        {
            await _documentos.InsertOneAsync(informe);
            DocumentoExpediente docexpe = new DocumentoExpediente()
            {
                tipo = informe.tipo,
                iddocumento = informe.id
            };
            UpdateDefinition<Expediente> updateExpediente = Builders<Expediente>.Update.Push("documentos", docexpe);
            _expedientes.FindOneAndUpdate(x => x.idresidente == informe.contenido.idresidente, updateExpediente);
            return informe;
        }
        public async Task<InformeSocialEvolutivo> RegistrarInformeSE(InformeSocialEvolutivo informe)
        {
            await _documentos.InsertOneAsync(informe);
            DocumentoExpediente docexpe = new DocumentoExpediente()
            {
                tipo = informe.tipo,
                iddocumento = informe.id
            };
            UpdateDefinition<Expediente> updateExpediente = Builders<Expediente>.Update.Push("documentos", docexpe);
            _expedientes.FindOneAndUpdate(x => x.idresidente == informe.contenido.idresidente, updateExpediente);
            return informe;
        }

        //falta el PsicologicoInicial
        public async Task<InformePsicologicoEvolutivo> RegistrarInformePE(InformePsicologicoEvolutivo informe)
        {
            await _documentos.InsertOneAsync(informe);
            DocumentoExpediente docexpe = new DocumentoExpediente()
            {
                tipo = informe.tipo,
                iddocumento = informe.id
            };
            UpdateDefinition<Expediente> updateExpediente = Builders<Expediente>.Update.Push("documentos", docexpe);
            _expedientes.FindOneAndUpdate(x => x.idresidente == informe.contenido.idresidente, updateExpediente);
            return informe;
        }
    }
}
