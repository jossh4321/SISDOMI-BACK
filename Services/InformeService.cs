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

        public InformeService(ISysdomiDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _documentos = database.GetCollection<Documento>("documentos");
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
    }
}
