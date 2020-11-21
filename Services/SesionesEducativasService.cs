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
    public class SesionesEducativasService
    {
        private readonly IMongoCollection<SesionEducativa> _sesioneducativa;

        public SesionesEducativasService(ISysdomiDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _sesioneducativa = database.GetCollection<SesionEducativa>("sesioneseducativas");

        }

        //Trae la lista de sesiones educativas de la bd
        public async Task<List<SesionEducativa>> GetAll()
        {
            List<SesionEducativa> sesionEducativa = new List<SesionEducativa>();

            var match = new BsonDocument("$match",
                        new BsonDocument("tipo",
                        new BsonDocument("$eq", "Sesion Educativa")));

            sesionEducativa = await _sesioneducativa.Aggregate()
                                .AppendStage<SesionEducativa>(match)
                                .ToListAsync();

            return sesionEducativa;
        }
    }
}
