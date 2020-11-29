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
    public class AnexoService
    {
        private readonly IMongoCollection<Anexo> _anexos;

        public AnexoService(ISysdomiDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _anexos = database.GetCollection<Anexo>("anexos");
        }

        public async Task<List<AnexoDTO>> GetAll()
        {
            List<AnexoDTO> listAnexos;

            //Para obtener los datos del residente usando el lookup
            var lookupResidente = new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "residentes" },
                {"let", new BsonDocument("idResidente", "$idresidente") },
                { "pipeline", new BsonArray
                             {
                                new BsonDocument("$match",
                                new BsonDocument("$expr",
                                new BsonDocument("$eq",
                                new BsonArray {
                                    "$_id",
                                    new BsonDocument("$toObjectId", "$$idResidente")
                                })))
                             }
                },
                { "as", "residente" }
            });

            //Para cambiar el arrays de residentes por un objeto utilizando unwind
            var unwindResidente = new BsonDocument("$unwind", new BsonDocument("path", "$residente"));

            //Para solo enviar los datos que se necesitan utilizando project
            var projectAnexo = new BsonDocument("$project", new BsonDocument
            {
                { "titulo", 1 },
                { "descripcion", 1 },
                { "idresidente", 1 },
                { "idcreador", 1 },
                { "creador", 1 },
                { "fechacreacion", 1 },
                { "area", 1 },
                { "enlaces", 1 },
                { "residente", new BsonDocument("$concat",
                               new BsonArray{
                                   "$residente.nombre",
                                   " ",
                                   "$residente.apellido"
                               })
                }
            });

            listAnexos = await _anexos.Aggregate()
                                                    .AppendStage<dynamic>(lookupResidente)
                                                    .AppendStage<dynamic>(unwindResidente)
                                                    .AppendStage<AnexoDTO>(projectAnexo)
                                                    .ToListAsync();

            return listAnexos;
        }
    }
}
