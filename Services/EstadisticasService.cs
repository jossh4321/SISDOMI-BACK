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
    public class EstadisticasService
    {
        private readonly IMongoCollection<Residentes> _residentes;

        public EstadisticasService(ISysdomiDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _residentes = database.GetCollection<Residentes>("residentes");

        }

        public async Task<List<EstadisticaDTO>> GetStadisticsResidentByFase()
        {
            List<EstadisticaDTO> lstEstadisticaDTOs;

            var setProgress = new BsonDocument("$set",
                              new BsonDocument("progreso",
                              new BsonDocument("$arrayElemAt",
                              new BsonArray
                              {
                                  "$progreso",
                                  -1
                              })));

            var groupProgressName = new BsonDocument("$group",
                                    new BsonDocument
                                    {
                                        { "_id", "$progreso.nombre" },
                                        {  "cantidad", new BsonDocument("$sum", 1) }
                                    });

            var projectStadistics = new BsonDocument("$project",
                                    new BsonDocument
                                    {
                                        { "_id", 0 },
                                        { "tipo",  "$_id" },
                                        { "cantidad", "$cantidad" }
                                    });

            lstEstadisticaDTOs = await _residentes.Aggregate()
                                    .AppendStage<dynamic>(setProgress)
                                    .AppendStage<dynamic>(groupProgressName)
                                    .AppendStage<EstadisticaDTO>(projectStadistics)
                                    .ToListAsync();

            return lstEstadisticaDTOs;

        }

        //public async Task<List<EstadisticaDTO>> GetStadisticsResidentsByRangeAge()
        //{
        //    List<EstadisticaDTO> lstEstadisticaDTOs;

        //    var addFieldsAge = new BsonDocument("$addFields",
        //                       new BsonDocument("edad",
        //                       new BsonDocument("$toInt",
        //                       )))

        //    return lstEstadisticaDTOs;
        //}



    }
}
