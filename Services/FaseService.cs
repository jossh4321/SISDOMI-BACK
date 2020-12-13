using SISDOMI.Entities;
using SISDOMI.Helpers;
using SISDOMI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using SISDOMI.DTOs;


namespace SISDOMI.Services
{
    public class FaseService
    {
        private readonly IMongoCollection<Fase> _documentofase;

        public FaseService(ISysdomiDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _documentofase = database.GetCollection<Fase>("fases");

        }

        public Fase ModifyFase(Fase documentofase)
        {
            var filter = Builders<Fase>.Filter.Eq("id", documentofase.id);
            var update = Builders<Fase>.Update
                .Set("idresidente", documentofase.idresidente)
                .Set("progreso", documentofase.progreso);

            documentofase = _documentofase.FindOneAndUpdate<Fase>(filter, update, new FindOneAndUpdateOptions<Fase>
            {
                ReturnDocument = ReturnDocument.After
            });
            return documentofase;
        }

    }
}