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
    public class TallerService
    {
        private readonly IMongoCollection<TallerDTO> _talleres;

        public TallerService(ISysdomiDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _talleres = database.GetCollection<TallerDTO>("talleres");
        }

        public async Task<List<Taller>> GetAll()
        {
            List<Taller> listTaller = new List<Taller>();

            var proyect = new BsonDocument("$project",
    new BsonDocument
        {
            { "_id", 1 },
            { "titulo", 1 },
            { "tipo",
    new BsonDocument("$cond",
    new BsonDocument
                {
                    { "if",
    new BsonDocument("$eq",
    new BsonArray
                        {
                            "$tipo",
                            "TallerEscuelaPadres"
                        }) },
                    { "then", "Taller de escuela para padres" },
                    { "else",
    new BsonDocument("$cond",
    new BsonDocument
                        {
                            { "if",
    new BsonDocument("$eq",
    new BsonArray
                                {
                                    "$tipo",
                                    "TallerEducativo"
                                }) },
                            { "then", "Taller Educativo" },
                            { "else", "Taller Formativo de Egreso" }
                        }) }
                }) },
            { "area", 1 },
            { "fase", 1 },
            { "fechacreacion", 1 }
        });

            listTaller = await _talleres.Aggregate()
                                .AppendStage<Taller>(proyect)
                                .ToListAsync();

            return listTaller;
        }

        public async Task<List<TallerEscuelaPadres>> GetAllTEP()
        {
            List<TallerEscuelaPadres> listTaller = new List<TallerEscuelaPadres>();

            var match = new BsonDocument("$match",
                    new BsonDocument("tipo", "TallerEscuelaPadres"));

            listTaller = await _talleres.Aggregate()
                                .AppendStage<TallerEscuelaPadres>(match)
                                .ToListAsync();

            return listTaller;
        }

        public async Task<List<TallerEducativo>> GetAllTE()
        {
            List<TallerEducativo> listTaller = new List<TallerEducativo>();

            var match = new BsonDocument("$match",
                    new BsonDocument("tipo", "TallerEducativo"));

            listTaller = await _talleres.Aggregate()
                                .AppendStage<TallerEducativo>(match)
                                .ToListAsync();

            return listTaller;
        }

        public async Task<List<TallerFormativoEgreso>> GetAllTFE()
        {
            List<TallerFormativoEgreso> listTaller = new List<TallerFormativoEgreso>();

            var match = new BsonDocument("$match",
                    new BsonDocument("tipo", "TallerFormativoEgreso"));

            listTaller = await _talleres.Aggregate()
                                .AppendStage<TallerFormativoEgreso>(match)
                                .ToListAsync();

            return listTaller;
        }

        //Creacion de talleres
        public TallerEscuelaPadres CreateTEP(TallerEscuelaPadres mitaller)
        {
            _talleres.InsertOne(mitaller);
            return mitaller;
        }

        public TallerEducativo CreateTE(TallerEducativo mitaller)
        {
            _talleres.InsertOne(mitaller);
            return mitaller;
        }

        public TallerFormativoEgreso CreateTFE(TallerFormativoEgreso mitaller)
        {
            _talleres.InsertOne(mitaller);
            return mitaller;
        }

        //cONSULTA DE cualquier TALLER POR ID
        public async Task<Taller> GetById(string id)
        {
            var match = new BsonDocument("$match",
                        new BsonDocument("_id",
                        new ObjectId(id)));

            Taller documento = new Taller();
            documento = await _talleres.Aggregate()
                            .AppendStage<Taller>(match)
                            .FirstAsync();
            return documento;
        }
    }
}
