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
    public class TallerEscuelaPadresService
    {
        private readonly IMongoCollection<TallerEscuelaPadres> _talleres;

        public TallerEscuelaPadresService(ISysdomiDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _talleres = database.GetCollection<TallerEscuelaPadres>("talleres");
        }

        public async Task<List<TallerEscuelaPadres>> GetAll()
        {
            List<TallerEscuelaPadres> listTaller = new List<TallerEscuelaPadres>();

            listTaller = _talleres.AsQueryable().OfType<TallerEscuelaPadres>().ToList();

            return listTaller;
        }
    }
}
