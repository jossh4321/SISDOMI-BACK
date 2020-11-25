using MongoDB.Driver;
using SISDOMI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Services
{
    public class RolService
    {
        private readonly IMongoCollection<Rol> _roles;

        public RolService(ISysdomiDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _roles = database.GetCollection<Rol>("roles");
        }

        public async Task<Rol> Get(String id)
        {
            Rol rol =await  _roles.Find(x => x.id == id).FirstOrDefaultAsync();

            return rol;
        }
    }
}
