using Microsoft.AspNetCore.Mvc;
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
    public class HojaProductividadService
    {
        private readonly IMongoCollection<Documento> _documentos;
        public HojaProductividadService(ISysdomiDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _documentos = database.GetCollection<Documento>("documentos");
        }
        public async Task<ActionResult<List<EstadisticaDTO>>> GetHojaProductivdadByIdResidente(string id)
        {
            List<EstadisticaDTO> hoja;

            var matchHoja = new BsonDocument("$match", new BsonDocument("idresidente", id));

            var groupHoja = new BsonDocument("$group",
                            new BsonDocument
                            {
                                { "_id", "$tipo" },
                                { "cantidad",
                                new BsonDocument("$sum", 1) }
                            });
            var projectHoja = new BsonDocument("$project",
                                new BsonDocument
                                {
                                    { "_id", 0 },
                                    { "tipo", "$_id" },
                                    { "cantidad", 1 }
                                });
            hoja = await _documentos.Aggregate()
                                    .AppendStage<dynamic>(matchHoja)
                                    .AppendStage<dynamic>(groupHoja)
                                    .AppendStage<EstadisticaDTO>(projectHoja)
                                    .ToListAsync();

            return hoja;
        }
    }
}
