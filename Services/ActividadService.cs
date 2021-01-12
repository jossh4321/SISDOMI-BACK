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
    public class ActividadService
    {

        private readonly IMongoCollection<Actividades> _actividades;

        public ActividadService(ISysdomiDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _actividades = database.GetCollection<Actividades>("actividades");
        }

        public async Task<List<ActividadDTOConsulta>> GetAll()
        {
            List<ActividadDTOConsulta> listActividades;

            //Para obtener los datos del residente usando el lookup
            var lookupCreador = new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "usuarios" },
                {"let", new BsonDocument("idCreador", "$idcreador") },
                { "pipeline", new BsonArray
                             {
                                new BsonDocument("$match",
                                new BsonDocument("$expr",
                                new BsonDocument("$eq",
                                new BsonArray {
                                    "$_id",
                                    new BsonDocument("$toObjectId", "$$idCreador")
                                })))
                             }
                },
                { "as", "creador" }
            });

            //Para cambiar el arrays de creador por un objeto utilizando unwind
            var unwindCreador = new BsonDocument("$unwind", new BsonDocument("path", "$creador"));

            //Para solo enviar los datos que se necesitan utilizando project
            var projectActividad = new BsonDocument("$project", new BsonDocument
            {
                { "tipo", 1 },
                { "nombre", 1 },
                { "descripcion", 1 },
                { "idcreador", 1 },
                { "creador", new BsonDocument("$concat",
                             new BsonArray{
                                   "$creador.datos.nombre",
                                   " ",
                                   "$creador.datos.apellido"
                               })
                },
                { "fechacreacion", 1 },
                { "contenido", 1 },
                { "totalparticipantes", new BsonDocument("$size", "$contenido.participantes") }
            });

            listActividades = await _actividades.Aggregate()
                                                    .AppendStage<dynamic>(lookupCreador)
                                                    .AppendStage<dynamic>(unwindCreador)
                                                    .AppendStage<ActividadDTOConsulta>(projectActividad)
                                                    .ToListAsync();

            return listActividades;
        }

        public async Task<Actividades> GetById(string id)
        {
            Actividades actividad;
            var match = new BsonDocument("$match",
                        new BsonDocument("_id",
                        new ObjectId(id)));

            
             actividad = await _actividades.Aggregate()
                            .AppendStage<Actividades>(match)
                            .FirstAsync();
            return actividad;
        }

        public async Task<ActionResult<Actividades>> CreateActividad(Actividades actividad)
        {
            actividad.fechacreacion = DateTime.UtcNow.AddHours(-5);
            await _actividades.InsertOneAsync(actividad);
            return actividad;
        }
    }
}
