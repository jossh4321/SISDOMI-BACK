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

    public class ActaDeExternamientoService
    {
        private readonly IMongoCollection<Documento> _documentos;
        private readonly ExpedienteService expedienteService;
        private readonly IDocument document;

        public ActaDeExternamientoService(ISysdomiDatabaseSettings settings, ExpedienteService expedienteService, IDocument document)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _documentos = database.GetCollection<Documento>("documentos");

            this.expedienteService = expedienteService;
            this.document = document;
        }


        public async Task<List<ActaExternamientoDTO>> GetAll()
        {
            List<ActaExternamientoDTO> listActaExternamiento;

            //Esto si solo se quiere la información de una colección que usa polimorfismo y que quiere un tipo de clase específica
            // listPlanIntervencionIndividuals =_documentos.AsQueryable().OfType<PlanIntervencionIndividual>().ToList();

            //Para obtener un tipo específico usando el match
            var matchPlan = new BsonDocument("$match", new BsonDocument("tipo", "ActaExternamiento"));

            //Para obtener los datos del residente usando el lookup
            var lookupPlan = new BsonDocument("$lookup", new BsonDocument
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
            var unwindPlan = new BsonDocument("$unwind", new BsonDocument("path", "$residente"));

            //Para solo enviar los datos que se necesitan utilizando project
            var projectPlan = new BsonDocument("$project", new BsonDocument
            {
                { "tipo", 1},
                {"fechacreacion", 1 },
              //  {"historialcontenido", 1 },
                {"creadordocumento", 1 },
                {"idresidente", 1 },
                { "area", 1 },
              { "fase", 1 },
                { "estado", 1 },
                {"responsable","$contenido.responsable"},

                { "residente", new BsonDocument("$concat",
                               new BsonArray{
                                   "$residente.nombre",
                                   " ",
                                   "$residente.apellido"
                               })
                }

            });

            listActaExternamiento = await _documentos.Aggregate()
                                                    .AppendStage<dynamic>(matchPlan)
                                                    .AppendStage<dynamic>(lookupPlan)
                                                    .AppendStage<dynamic>(unwindPlan)
                                                    .AppendStage<ActaExternamientoDTO>(projectPlan)
                                                    .ToListAsync();

            return listActaExternamiento;
        }

    }
}