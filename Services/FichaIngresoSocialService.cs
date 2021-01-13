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
    public class FichaIngresoSocialService
    {
        private readonly IMongoCollection<Documento> _documentos;
        private readonly IMongoCollection<Expediente> _expedientes;
        private readonly ExpedienteService expedienteService;
        private readonly IDocument document;
        private readonly FaseService faseService;

        public FichaIngresoSocialService(ISysdomiDatabaseSettings settings, IDocument document, ExpedienteService expedienteService,FaseService faseService)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _documentos = database.GetCollection<Documento>("documentos");
            _expedientes = database.GetCollection<Expediente>("expedientes");
            this.expedienteService = expedienteService;
            this.faseService = faseService;
            this.document = document;
        }

        public List<FichaIngresoSocial> GetAll()
        {
            List<FichaIngresoSocial> listFichaIngresoSocial = new List<FichaIngresoSocial>();

            listFichaIngresoSocial = _documentos.AsQueryable().OfType<FichaIngresoSocial>().ToList();

            return listFichaIngresoSocial;
        }
        //
        public async Task<FichaIngresoDTO> CreateFichaIngresoSocial(FichaIngresoSocial documento)
        {
            documento.fechacreacion = DateTime.UtcNow.AddHours(-5);
            DateTime DateNow = DateTime.UtcNow.AddHours(-5);
            Expediente expediente = await expedienteService.GetByResident(documento.idresidente);
            documento.contenido.codigodocumento = document.CreateCodeDocument(DateNow, documento.tipo, expediente.documentos.Count + 1);
            await _documentos.InsertOneAsync(documento);
            DocumentoExpediente docexpe = new DocumentoExpediente()
            {
                tipo = documento.tipo,
                iddocumento = documento.id
            };
            await expedienteService.UpdateDocuments(docexpe, expediente.id);

            FichaIngresoDTO fichaIngreso = await obtenerResidienteFichaIngreso(documento.id);
            Fase fase = faseService.ModifyStateForDocument(documento.idresidente, documento.fase, documento.area, documento.tipo);
            return fichaIngreso;
        }
        public FichaIngresoSocial GetById(string id)
        {
            FichaIngresoSocial documento = new FichaIngresoSocial();
            documento = _documentos.AsQueryable().OfType<FichaIngresoSocial>().ToList().Find(documento => documento.id == id);
            return documento;
        }
        public async Task<FichaIngresoDTO> ModifyFichaIngresoSocial(FichaIngresoSocial documento)
        {
            var filter = Builders<Documento>.Filter.Eq("id", documento.id);
            var update = Builders<Documento>.Update
                .Set("tipo", documento.tipo)
                .Set("historialcontenido", documento.historialcontenido)
                .Set("creadordocumento", documento.creadordocumento)
                .Set("fechacreacion", documento.fechacreacion)
                .Set("area", documento.area)
                .Set("fase", documento.fase)
                .Set("estado", documento.estado)
                .Set("contenido", documento.contenido);
            var doc = _documentos.FindOneAndUpdate<Documento>(filter, update, new FindOneAndUpdateOptions<Documento>
            {
                ReturnDocument = ReturnDocument.After
            });
            documento = doc as FichaIngresoSocial;
            FichaIngresoDTO fichaIngresoDTO = new FichaIngresoDTO();
            fichaIngresoDTO = await obtenerResidienteFichaIngreso(documento.id);
            return fichaIngresoDTO;
        }
        public async Task<FichaIngresoDTO> obtenerResidienteFichaIngreso(string id)
        {
            var match = new BsonDocument("$match",
                                      new BsonDocument("_id",
                                      new ObjectId(id)));
            // lookup para fichas ingreso 
            var subpipeline_fichaIngreso = new BsonArray
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

            var lookup_fichaIngreso = new BsonDocument("$lookup",
                              new BsonDocument
                                  {
                                          { "from", "residentes" },
                                          { "let",
                                  new BsonDocument("idres", "$idresidente") },
                                          { "pipeline",subpipeline_fichaIngreso
                                      },
                                             { "as", "residenteresultado" }
                                  });

            // 
            var unwindFicha = new BsonDocument("$unwind", new BsonDocument("path", "$residenteresultado"));

            //Proyeccion de cada documentos 
            var project = new BsonDocument("$project",
                          new BsonDocument
                              {
                                          { "_id", 1 },
                                          { "_t", 1 },
                                          { "tipo", 1 },
                                          { "historialcontenido", 1 },
                                          { "creadordocumento", 1 },
                                          { "fechacreacion", 1 },
                                          { "area", 1 },
                                          { "fase", 1 },
                                          { "estado", 1 },
                                          { "codigodocumento", "$contenido.codigodocumento" },
                                          { "residenteresultado",
                                  new BsonDocument("$concat",
                                  new BsonArray
                                              {
                                                   "$residenteresultado.nombre",
                                                     " ",
                                                   "$residenteresultado.apellido"
                                              }) }
                              });
            FichaIngresoDTO fichaIngreso = await _documentos.Aggregate()
                 .AppendStage<dynamic>(match)
                 .AppendStage<dynamic>(lookup_fichaIngreso)
                   .AppendStage<dynamic>(unwindFicha)
                 .AppendStage<FichaIngresoDTO>(project).SingleAsync();



            return fichaIngreso;
        }

        public async Task<List<FichaIngresoDTO>> obtenerResidientesFichaIngreso(String fromDate, String toDate)
        {
            var match = new BsonDocument("$match",
                                      new BsonDocument("tipo",
                                      new BsonDocument("$in",
                                      new BsonArray
                  {
                    "FichaEducativaIngreso",
                    "FichaSocialIngreso",
                    "FichaPsicologicaIngreso"
                  })));

            // Para realizar el filtro de los planes por fechas
            var addFieldDayYearMonth = new BsonDocument("$addFields",
                                       new BsonDocument
                                       {
                                           { "mes", new BsonDocument("$month", "$fechacreacion") },
                                           { "ano", new BsonDocument("$year", "$fechacreacion") },
                                           { "dia", new BsonDocument("$dayOfMonth", "$fechacreacion") }
                                       });

            //Se obtiene solamente la fecha sin los minutos ni los milisegundos
            var addFieldDate = new BsonDocument("$addFields",
                               new BsonDocument("fecha",
                               new BsonDocument("$toDate",
                               new BsonDocument("$concat",
                               new BsonArray
                               {
                                   new BsonDocument("$toString", "$ano"),
                                   "-",
                                   new BsonDocument("$toString", "$mes"),
                                   "-",
                                   new BsonDocument("$toString", "$dia")
                               }))));

            BsonValue fromDateTransform;
            BsonValue toDateTransform;

            if (fromDate != null)
            {
                fromDateTransform = fromDate;
            }
            else
            {
                fromDateTransform = BsonNull.Value;
            }

            if (toDate != null)
            {
                toDateTransform = toDate;
            }
            else
            {
                toDateTransform = BsonNull.Value;
            }

            //Obtener los planes donde solamente este entre las fechas consultadas
            var matchPlanesBetweenDate = new BsonDocument("$match",
                                         new BsonDocument("$expr",
                                         new BsonDocument("$and",
                                         new BsonArray
                                         {
                                             new BsonDocument("$or",
                                             new BsonArray
                                             {
                                                 new BsonDocument("$gte",
                                                 new BsonArray
                                                 {
                                                     "$fecha",
                                                     new BsonDocument("$toDate", fromDateTransform)
                                                 }),
                                                 new BsonDocument("$eq",
                                                 new BsonArray
                                                 {
                                                     fromDateTransform,
                                                     BsonNull.Value
                                                 })
                                             }),
                                             new BsonDocument("$or",
                                             new BsonArray
                                             {
                                                 new BsonDocument("$lte",
                                                 new BsonArray
                                                 {
                                                     "$fecha",
                                                     new BsonDocument("$toDate", toDateTransform)
                                                 }),
                                                 new BsonDocument("$eq",
                                                 new BsonArray
                                                 {
                                                     toDateTransform,
                                                     BsonNull.Value
                                                 })
                                             })
                                         }
                                         )));

            // Para eliminar las variables creadas para la consulta entre fechas
            var projectPlanNormal = new BsonDocument("$project", new BsonDocument
            {
                { "_id", 1},
                {"_t", 1 },
                { "tipo", 1 },
                { "historialcontenido", 1 },
                { "creadordocumento", 1 },
                { "fechacreacion", 1 },
                { "area", 1 },
                { "fase", 1 },
                { "idresidente", 1 },
                { "estado", 1 },
                { "contenido", 1 }

            });


            // lookup para fichas ingreso 
            var subpipeline_fichaIngreso = new BsonArray
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

            var lookup_fichaIngreso = new BsonDocument("$lookup",
                              new BsonDocument
                                  {
                                          { "from", "residentes" },
                                          { "let",
                                  new BsonDocument("idres", "$idresidente") },
                                          { "pipeline",subpipeline_fichaIngreso
                                      },
                                             { "as", "residenteresultado" }
                                  });

            // 
            var unwindFicha = new BsonDocument("$unwind", new BsonDocument("path", "$residenteresultado"));

            //Proyeccion de cada documentos 
            var project = new BsonDocument("$project",
                          new BsonDocument
                              {
                                          { "_id", 1 },
                                          { "_t", 1 },
                                          { "tipo", 1 },
                                          { "historialcontenido", 1 },
                                          { "creadordocumento", 1 },
                                          { "fechacreacion", 1 },
                                          { "area", 1 },
                                          { "fase", 1 },
                                          { "estado", 1 },
                                          { "codigodocumento", "$contenido.codigodocumento" },
                                          { "residenteresultado",
                                  new BsonDocument("$concat",
                                  new BsonArray
                                              {
                                                   "$residenteresultado.nombre",
                                                     " ",
                                                   "$residenteresultado.apellido"
                                              }) }
                              });
            List<FichaIngresoDTO> fichaIngreso = await _documentos.Aggregate()
                 .AppendStage<dynamic>(match)
                 .AppendStage<dynamic>(addFieldDayYearMonth)
                 .AppendStage<dynamic>(addFieldDate)
                 .AppendStage<dynamic>(matchPlanesBetweenDate)
                 .AppendStage<dynamic>(projectPlanNormal)
                 .AppendStage<dynamic>(lookup_fichaIngreso)
                 .AppendStage<dynamic>(unwindFicha)
                 .AppendStage<FichaIngresoDTO>(project).ToListAsync();



            return fichaIngreso;
        }
       

    }

}
