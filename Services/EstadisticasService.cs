using MongoDB.Bson;
using MongoDB.Driver;
using SISDOMI.DTOs;
using SISDOMI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace SISDOMI.Services
{
    public class EstadisticasService
    {
        private readonly IMongoCollection<Residentes> _residentes;
        private readonly IMongoCollection<Documento> _documentos; 

        public EstadisticasService(ISysdomiDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _residentes = database.GetCollection<Residentes>("residentes");
            _documentos = database.GetCollection<Documento>("documentos");

        }

        public async Task<List<EstadisticaDTO>> GetStadisticsResidentByFase()
        {
            List<EstadisticaDTO> lstEstadisticaDTOs;

            var setProgress = new BsonDocument("$set",
                              new BsonDocument("progreso",
                              new BsonDocument("$arrayElemAt",
                              new BsonArray
                              {
                                  "$progreso",
                                  -1
                              })));

            var groupProgressName = new BsonDocument("$group",
                                    new BsonDocument
                                    {
                                        { "_id", "$progreso.nombre" },
                                        {  "cantidad", new BsonDocument("$sum", 1) }
                                    });

            var projectStadistics = new BsonDocument("$project",
                                    new BsonDocument
                                    {
                                        { "_id", 0 },
                                        { "tipo",  "$_id" },
                                        { "cantidad", "$cantidad" }
                                    });

            lstEstadisticaDTOs = await _residentes.Aggregate()
                                    .AppendStage<dynamic>(setProgress)
                                    .AppendStage<dynamic>(groupProgressName)
                                    .AppendStage<EstadisticaDTO>(projectStadistics)
                                    .ToListAsync();


            return lstEstadisticaDTOs;

        }

        public async Task<List<EstadisticaDTO>> GetStadisticsResidentsByRangeAge()
        {
            List<EstadisticaDTO> lstEstadisticaDTOs;

            BigInteger totalMiliSecondsByAge = 31536000000;

            var addFieldsAge = new BsonDocument("$addFields",
                               new BsonDocument("edad",
                               new BsonDocument("$toInt",
                               new BsonDocument("$divide",
                               new BsonArray
                               {
                                   new BsonDocument("$subtract",
                                   new BsonArray
                                   {
                                       DateTime.UtcNow.AddHours(-5),
                                       "$fechanacimiento"
                                   }),
                                   new BsonDocument("$toLong", totalMiliSecondsByAge.ToString())
                               }
                               ))));

            var setTypeAge = new BsonDocument("$set",
                             new BsonDocument("tipoedad",
                             new BsonDocument("$switch",
                             new BsonDocument {
                                 {
                                     "branches",
                                     new BsonArray
                                     {
                                         new BsonDocument
                                         {
                                             { "case",
                                             new BsonDocument("$and",
                                             new BsonArray
                                             {
                                                 new BsonDocument("$gte",
                                                 new BsonArray {
                                                     "$edad",
                                                     0
                                                 }),
                                                 new BsonDocument("$lte",
                                                 new BsonArray {
                                                     "$edad",
                                                     5
                                                 })
                                             })
                                             },
                                             {"then", "0-5" }
                                         },
                                         new BsonDocument
                                         {
                                             { "case",
                                             new BsonDocument("$and",
                                             new BsonArray
                                             {
                                                 new BsonDocument("$gte",
                                                 new BsonArray {
                                                     "$edad",
                                                     6
                                                 }),
                                                 new BsonDocument("$lte",
                                                 new BsonArray {
                                                     "$edad",
                                                     11
                                                 })
                                             })
                                             },
                                             { "then", "6-11" }
                                         },
                                         new BsonDocument
                                         {
                                             { "case",
                                                new BsonDocument("$and",
                                                new BsonArray {

                                                    new BsonDocument("$gte",
                                                    new BsonArray {
                                                        "$edad",
                                                        12
                                                    }),
                                                    new BsonDocument("$lte",
                                                    new BsonArray {
                                                        "$edad",
                                                        17
                                                    })
                                                })
                                             },
                                             { "then", "12-17" }
                                         }
                                     }
                                 },
                                 { "default", ">=18" }
                             })));
                             

            var groupTypeAge = new BsonDocument("$group",
                               new BsonDocument {
                                   { "_id", "$tipoedad" },
                                   { "cantidad",
                                        new BsonDocument("$sum", 1)}
                               });

            var projectStadistics = new BsonDocument("$project",
                                    new BsonDocument
                                    {
                                        { "_id", 0 },
                                        { "tipo", "$_id" },
                                        { "cantidad", 1 }
                                    });


            lstEstadisticaDTOs =await _residentes.Aggregate()
                                        .AppendStage<dynamic>(addFieldsAge)
                                        .AppendStage<dynamic>(setTypeAge)
                                        .AppendStage<dynamic>(groupTypeAge)
                                        .AppendStage<EstadisticaDTO>(projectStadistics)
                                        .ToListAsync();

            return lstEstadisticaDTOs;
        }

        public async Task<List<EstadisticaModalidadDTO>> GetStadisticsModalidadByGrade(String typeModalidad)
        {
            List<EstadisticaModalidadDTO> lstEstadisticaModalidadGradoDTOs;

            var matchFichaEducativaIngresoDocument = new BsonDocument("$match",
                                                     new BsonDocument("tipo", "FichaEducativaIngreso"));

            var groupModalityType = new BsonDocument("$group",
                                     new BsonDocument
                                     {
                                         { "_id", new BsonDocument {
                                             { typeModalidad, "$contenido.ieprocedencia." + typeModalidad },
                                             { "modalidad","$contenido.ieprocedencia.modalidad" }
                                          } 
                                         },
                                         { "cantidad", new BsonDocument("$sum", 1) }
                                     });

            var groupGrades = new BsonDocument("$group",
                              new BsonDocument
                              {
                                  { "_id", "$_id." + typeModalidad },
                                  { "modalidades", new BsonDocument("$push",
                                                   new BsonDocument {
                                                       { "modalidad", "$_id.modalidad"  },
                                                       { "cantidad", "$cantidad" }
                                                   }) 
                                  },
                                  { "cantidad", new BsonDocument("$sum", "$cantidad") }
                              });

            var projectEstadistic = new BsonDocument("$project",
                                    new BsonDocument
                                    {
                                        { "_id", 0 },
                                        { "tipo", "$_id" },
                                        { "modalidades", 1 },
                                        { "cantidad", 1 }
                                    });

            lstEstadisticaModalidadGradoDTOs = await _documentos.Aggregate()
                                                        .AppendStage<dynamic>(matchFichaEducativaIngresoDocument)
                                                        .AppendStage<dynamic>(groupModalityType)
                                                        .AppendStage<dynamic>(groupGrades)
                                                        .AppendStage<EstadisticaModalidadDTO>(projectEstadistic)
                                                        .ToListAsync();

            return lstEstadisticaModalidadGradoDTOs;
        }

        public async Task<EstadisticaResidenteProgresoDTO> GetStadisticProgressResidentByArea(String idResidente, String area)
        {
            EstadisticaResidenteProgresoDTO estadisticaResidenteProgresoDTO;

            var matchResident = new BsonDocument("$match",
                                new BsonDocument("$expr",
                                new BsonDocument("$eq",
                                new BsonArray
                                {
                                    "$_id",
                                    new BsonDocument("$toObjectId", idResidente)
                                })));

            var lookupFases = new BsonDocument("$lookup",
                              new BsonDocument
                              {
                                  { "from", "fases" },
                                  {
                                      "let",
                                      new BsonDocument("residenteid", "$_id")
                                  },
                                  {
                                      "pipeline",
                                      new BsonArray
                                      {
                                          new BsonDocument("$match",
                                          new BsonDocument("$expr",
                                          new BsonDocument("$eq",
                                          new BsonArray 
                                          {
                                              new BsonDocument("$toObjectId", "$idresidente"),
                                              "$$residenteid"
                                          })))
                                      }
                                  },
                                  { "as", "fases" }
                              });

            var unwindFase = new BsonDocument("$unwind",
                             new BsonDocument("path", "$fases"));

            var unwindProgreso = new BsonDocument("$unwind",
                                 new BsonDocument("path", "$fases.progreso"));

            var projectFaseProgreso = new BsonDocument("$project",
                                      new BsonDocument
                                      {
                                          { "_id", 1 },
                                          { "documentosfase", "$fases.progreso." + area },
                                          { "fase", "$fases.progreso.fase" },
                                          { "progreso", 1 }
                                      });

            var unwindFaseDocuments = new BsonDocument("$unwind",
                                  new BsonDocument("path", "$documentosfase.documentos"));

            var lookupDocuments = new BsonDocument("$lookup",
                                  new BsonDocument
                                  {
                                      { "from", "documentos" },
                                      {
                                          "let",
                                          new BsonDocument
                                          {
                                              { "residenteid", "$_id" },
                                              { "tipo", "$documentosfase.documentos.tipo" },
                                              { "fase", "$fase" }
                                          }
                                      },
                                      {
                                          "pipeline",
                                          new BsonArray
                                          {
                                              new BsonDocument("$match",
                                              new BsonDocument("$expr",
                                              new BsonDocument("$and",
                                              new BsonArray
                                              {
                                                  new BsonDocument("$eq",
                                                  new BsonArray
                                                  {
                                                       new BsonDocument("$toObjectId", "$idresidente"),
                                                       "$$residenteid"
                                                  }),
                                                  new BsonDocument("$eq",
                                                  new BsonArray
                                                  {
                                                      "$tipo",
                                                      "$$tipo"
                                                  }),
                                                  new BsonDocument("$eq",
                                                  new BsonArray
                                                  {
                                                      new BsonDocument("$toInt", "$fase"),
                                                      "$$fase"
                                                  })

                                              }))),
                                              new BsonDocument("$project",
                                              new BsonDocument
                                              {
                                                  { "_id", 0 },
                                                  { "tipo", 1 },
                                                  { "fase", 1 },
                                                  { "fechacreacion", 1 }
                                              })
                                          }
                                      },
                                      { "as", "documentos" }
                                  });

            var unwindDocuments = new BsonDocument("$unwind",
                                  new BsonDocument
                                      {
                                        {"path", "$documentos" },
                                        { "preserveNullAndEmptyArrays", true }
                                      });

            var groupFaseDocument = new BsonDocument("$group",
                                    new BsonDocument
                                    {
                                        { "_id", "$fase" },
                                        { "documentos",
                                          new BsonDocument("$push",
                                          new BsonDocument
                                          {
                                              { "tipo", "$documentos.tipo" },
                                              { "fechacreacion", "$documentos.fechacreacion" }
                                          })
                                        },
                                        { "id", new BsonDocument("$first", "$_id") },
                                        { "progreso", new BsonDocument("$first", "$progreso") }
                                    });

            var groupFaseProgreso = new BsonDocument("$group",
                                    new BsonDocument
                                    {
                                        { "_id", "$id" },
                                        {
                                            "fases",
                                            new BsonDocument("$push",
                                            new BsonDocument
                                            {
                                                { "fase", "$_id" },
                                                { "documentos", "$documentos" }
                                            })
                                        },
                                        {
                                            "progreso",
                                            new BsonDocument("$first", "$progreso")
                                        }
                                    });

            var projectResidentProgress = new BsonDocument("$project",
                                          new BsonDocument
                                          {
                                              { "_id", 0 },
                                              { "fases", 1 },
                                              { "progreso", 1 }
                                          });

            estadisticaResidenteProgresoDTO =await _residentes.Aggregate()
                                                .AppendStage<dynamic>(matchResident)
                                                .AppendStage<dynamic>(lookupFases)
                                                .AppendStage<dynamic>(unwindFase)
                                                .AppendStage<dynamic>(unwindProgreso)
                                                .AppendStage<dynamic>(projectFaseProgreso)
                                                .AppendStage<dynamic>(unwindFaseDocuments)
                                                .AppendStage<dynamic>(lookupDocuments)
                                                .AppendStage<dynamic>(unwindDocuments)
                                                .AppendStage<dynamic>(groupFaseDocument)
                                                .AppendStage<dynamic>(groupFaseProgreso)
                                                .AppendStage<EstadisticaResidenteProgresoDTO>(projectResidentProgress)
                                                .FirstOrDefaultAsync();

            return estadisticaResidenteProgresoDTO;
        }

    }
}
