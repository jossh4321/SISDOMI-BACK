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
    public class IncidenciaService
    {
        private readonly IMongoCollection<Incidencia> _incidencias;

        public IncidenciaService(ISysdomiDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _incidencias = database.GetCollection<Incidencia>("incidencias");
        }

        public async Task<List<Incidencia>> GetAll()
        {
            List<Incidencia> incidencias = new List<Incidencia>();
            incidencias = await _incidencias.Find(inicidencia => true).ToListAsync();
            return incidencias;
        }
        public async Task<Incidencia> GetIncidencia(string idincidencia)
        {
            Incidencia incidencia = new Incidencia();
            incidencia = await _incidencias.Find(incidencia => incidencia.id == idincidencia).SingleOrDefaultAsync();
            return incidencia;
        }
        public async Task<IncidenciaDTO> PostIncidencia(Incidencia incidencia)
        {
            incidencia.fecha.AddHours(-5);
            await _incidencias.InsertOneAsync(incidencia);
            IncidenciaDTO incidenciaDTO = await GetDetalleIncidencia(incidencia.id);
            return incidenciaDTO;
        }
        public async Task<IncidenciaDTO> PutIncidencia(Incidencia incidencia)
        {
            incidencia.fecha.AddHours(-5);
            var updatefilter = Builders<Incidencia>
                                .Filter
                                .Eq("id", incidencia.id);
            var update = Builders<Incidencia>.Update
                //.Set("usuario", incidencia.usuario)
                .Set("fecha", incidencia.fecha)
                .Set("titulo", incidencia.titulo)
                .Set("descripcion", incidencia.descripcion)
                .Set("observaciones", incidencia.observaciones)
                .Set("incidencias", incidencia.incidencias)
                .Set("residentes", incidencia.residentes)
                .Set("firma", incidencia.firma);
            incidencia = await  _incidencias.FindOneAndUpdateAsync<Incidencia>
                            (updatefilter, update, new FindOneAndUpdateOptions<Incidencia>
                            {
                                ReturnDocument = ReturnDocument.After
                            });
            IncidenciaDTO incidenciaDTO = await GetDetalleIncidencia(incidencia.id);
            return incidenciaDTO;
        }

        public async Task<List<IncidenciaDTO>> GetListDetalleIncidencia()
        {
            var unwind1 = new BsonDocument("$unwind",
                              new BsonDocument("path", "$residentes"));
            var subpipeline1 = new BsonArray
                        {
                            new BsonDocument("$match",
                            new BsonDocument("$expr",
                            new BsonDocument("$eq",
                            new BsonArray
                                        {
                                            "$_id",
                                            new BsonDocument("$toObjectId", "$$idres")
                                        })))
                        };
            var lookup1 = new BsonDocument("$lookup",
              new BsonDocument
                  {
                        { "from", "residentes" },
                        { "let",
                new BsonDocument("idres", "$residentes") },
                        { "pipeline", subpipeline1},
                        { "as", "residente" }
                  });

            var unwind2 = new BsonDocument("$unwind",
                    new BsonDocument("path", "$residente"));

            var group = new BsonDocument("$group",
                                new BsonDocument
                                    {
                                        { "_id", "$_id" },
                                        { "usuario",
                                new BsonDocument("$first", "$usuario") },
                                        { "fecharegistro",
                                new BsonDocument("$first", "$fecharegistro") },
                                        { "fecha",
                                new BsonDocument("$first", "$fecha") },
                                        { "titulo",
                                new BsonDocument("$first", "$titulo") },
                                        { "descripcion",
                                new BsonDocument("$first", "$descripcion") },
                                        { "observaciones",
                                new BsonDocument("$first", "$observaciones") },
                                        { "incidencias",
                                new BsonDocument("$first", "$incidencias") },
                                        { "residentes",
                                new BsonDocument("$push", "$residente") },
                                        { "firma",
                                new BsonDocument("$first", "$firma") }
                                    });

            var subpipeline2 = new BsonArray
                                        {
                                            new BsonDocument("$match",
                                            new BsonDocument("$expr",
                                            new BsonDocument("$eq",
                                            new BsonArray
                                                        {
                                                            "$_id",
                                                            new BsonDocument("$toObjectId", "$$idusu")
                                                        })))
                                        };

            var lookup2 = new BsonDocument("$lookup",
                                new BsonDocument
                                    {
                                        { "from", "usuarios" },
                                        { "let",
                                new BsonDocument("idusu", "$usuario") },
                                        { "pipeline", subpipeline2 },
                                        { "as", "autor" }
                                    });
            var unwind3 = new BsonDocument("$unwind",
            new BsonDocument("path", "$autor"));
            var project = new BsonDocument("$project",
                                new BsonDocument("usuario", 0));

            List<IncidenciaDTO> incidencia = new List<IncidenciaDTO>();
            incidencia = await _incidencias.Aggregate()
                            .AppendStage<dynamic>(unwind1)
                            .AppendStage<dynamic>(lookup1)
                            .AppendStage<dynamic>(unwind2)
                            .AppendStage<dynamic>(group)
                            .AppendStage<dynamic>(lookup2)
                            .AppendStage<dynamic>(unwind3)
                            .AppendStage<IncidenciaDTO>(project)
                            .ToListAsync();
            return incidencia;
        }

        public async Task<IncidenciaDTO> GetDetalleIncidencia(string id)
        {
            var match1 = new BsonDocument("$match",
                                new BsonDocument("_id",
                                new ObjectId(id)));

            var unwind1 = new BsonDocument("$unwind",
                                new BsonDocument("path", "$residentes"));
            var subpipeline1 = new BsonArray
                        {
                            new BsonDocument("$match",
                            new BsonDocument("$expr",
                            new BsonDocument("$eq",
                            new BsonArray
                                        {
                                            "$_id",
                                            new BsonDocument("$toObjectId", "$$idres")
                                        })))
                        } ;
            var lookup1 = new BsonDocument("$lookup",
              new BsonDocument
                  {
                        { "from", "residentes" },
                        { "let",
                new BsonDocument("idres", "$residentes") },
                        { "pipeline", subpipeline1},
                        { "as", "residente" }
                  });

            var unwind2 =  new BsonDocument("$unwind",
                    new BsonDocument("path", "$residente"));

            var group = new BsonDocument("$group",
                                new BsonDocument
                                    {
                                        { "_id", "$_id" },
                                        { "usuario",
                                new BsonDocument("$first", "$usuario") },
                                        { "fecharegistro",
                                new BsonDocument("$first", "$fecharegistro") },
                                        { "fecha",
                                new BsonDocument("$first", "$fecha") },
                                        { "titulo",
                                new BsonDocument("$first", "$titulo") },
                                        { "descripcion",
                                new BsonDocument("$first", "$descripcion") },
                                        { "observaciones",
                                new BsonDocument("$first", "$observaciones") },
                                        { "incidencias",
                                new BsonDocument("$first", "$incidencias") },
                                        { "residentes",
                                new BsonDocument("$push", "$residente") },
                                        { "firma",
                                new BsonDocument("$first", "$firma") }
                                    });

            var subpipeline2 = new BsonArray
                                        {
                                            new BsonDocument("$match",
                                            new BsonDocument("$expr",
                                            new BsonDocument("$eq",
                                            new BsonArray
                                                        {
                                                            "$_id",
                                                            new BsonDocument("$toObjectId", "$$idusu")
                                                        })))
                                        };

            var lookup2 = new BsonDocument("$lookup",
                                new BsonDocument
                                    {
                                        { "from", "usuarios" },
                                        { "let",
                                new BsonDocument("idusu", "$usuario") },
                                        { "pipeline", subpipeline2 },
                                        { "as", "autor" }
                                    });
            var unwind3 =  new BsonDocument("$unwind",
            new BsonDocument("path", "$autor"));
            var project = new BsonDocument("$project",
                                new BsonDocument("usuario", 0));

            IncidenciaDTO incidencia = new IncidenciaDTO();
            incidencia = await _incidencias.Aggregate()
                            .AppendStage<dynamic>(match1)
                            .AppendStage<dynamic>(unwind1)
                            .AppendStage<dynamic>(lookup1)
                            .AppendStage<dynamic>(unwind2)
                            .AppendStage<dynamic>(group)
                            .AppendStage<dynamic>(lookup2)
                            .AppendStage<dynamic>(unwind3)
                            .AppendStage<IncidenciaDTO>(project)
                            .FirstOrDefaultAsync();
            return incidencia;
        }
    }
}
