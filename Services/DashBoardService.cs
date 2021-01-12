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
    public class DashBoardService
    {
        private readonly IMongoCollection<Fase> _fases;
        private readonly IMongoCollection<Residentes> _residentes;
        public DashBoardService(ISysdomiDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _fases = database.GetCollection<Fase>("fases");
            _residentes = database.GetCollection<Residentes>("residentes");
        }

        public async Task<DashboardDTO> obtenerDashBoard()
        {
            DashboardDTO dashboardDTO = new DashboardDTO();
            dashboardDTO.residentesMesAño = await ObtenerListaResidentesPorMes();
            dashboardDTO.residentesFase = await obtenerResidentesPorFase();
            dashboardDTO.documentosPendientes = await obtenerListaDocumentosPendientes();
            dashboardDTO.documentosAtrazados = await obtenerDocumentosAtrazados();
            dashboardDTO.documentosPendientesHoy = await obtenerDocumentosPendientesHoy();
            return dashboardDTO;
        }
        public async Task<List<Object>> ObtenerListaResidentesPorMes()
        {
            List<Object> listaUsuariosTiempo = new List<Object>();
            var addFields = new BsonDocument("$addFields",
                                new BsonDocument
                                {
                                    { "faseactual",
                                        new BsonDocument("$arrayElemAt",
                                new BsonArray
                                {
                                    "$progreso",
                                    -1
                                }) },
                                    { "mes",
                                        new BsonDocument("$month", "$fechaingreso") },
                                    { "año",
                                        new BsonDocument("$year", "$fechaingreso") }
                                });

            var group = new BsonDocument("$group",
                            new BsonDocument
                            {
                                { "_id",
                                    new BsonDocument
                                    {
                                        { "mes", "$mes" },
                                        { "año", "$año" }
                                    } },
                                { "residentes",
                                    new BsonDocument("$sum",
                            new BsonDocument("$cond",
                            new BsonArray
                            {
                                new BsonDocument("$and",
                                                new BsonArray
                                                {
                                                    new BsonDocument("$eq",
                                                        new BsonArray
                                                        {
                                                            "$estado",
                                                            "En tratamiento"
                                                        })
                                                }),
                                1,
                                0
                            })) }
                            });
            var match = new BsonDocument("$match",
                                new BsonDocument("residentes",
                                new BsonDocument("$gt", 0)));
            var sort = new BsonDocument("$sort",
                                new BsonDocument
                                    {
                                        { "_id.año", 1 },
                                        { "_id.mes", 1 }
                                    });
        listaUsuariosTiempo = await _residentes.Aggregate()
                .AppendStage<dynamic>(addFields)
                .AppendStage<dynamic>(group)
                .AppendStage<dynamic>(match)
                .AppendStage<Object>(sort)
                .ToListAsync();
            return listaUsuariosTiempo;
        }

        public async Task<List<Object>> obtenerResidentesPorFase()
        {
            List<Object> listaResidentesPorFase = new List<Object>();
            var addFields = new BsonDocument("$addFields",
                                new BsonDocument("faseactual",
                                new BsonDocument("$arrayElemAt",
                                new BsonArray
                                            {
                                                "$progreso",
                                                -1
                                            })));
            var match = new BsonDocument("$match",
                        new BsonDocument("estado", "En tratamiento"));
            var group = new BsonDocument("$group",
                            new BsonDocument
                                {
                                    { "_id", "$estado" },
                                    { "fase1",
                            new BsonDocument("$sum",
                            new BsonDocument("$cond",
                            new BsonArray
                                            {
                                                new BsonDocument("$eq",
                                                new BsonArray
                                                    {
                                                        "$faseactual.fase",
                                                        1
                                                    }),
                                                1,
                                                0
                                            })) },
                                    { "fase2",
                            new BsonDocument("$sum",
                            new BsonDocument("$cond",
                            new BsonArray
                                            {
                                                new BsonDocument("$eq",
                                                new BsonArray
                                                    {
                                                        "$faseactual.fase",
                                                        2
                                                    }),
                                                1,
                                                0
                                            })) },
                                    { "fase3",
                            new BsonDocument("$sum",
                            new BsonDocument("$cond",
                            new BsonArray
                                            {
                                                new BsonDocument("$eq",
                                                new BsonArray
                                                    {
                                                        "$faseactual.fase",
                                                        3
                                                    }),
                                                1,
                                                0
                                            })) }
                                });
            listaResidentesPorFase = await _residentes.Aggregate()
                .AppendStage<dynamic>(addFields)
                .AppendStage<dynamic>(match)
                .AppendStage<Object>(group)
                .ToListAsync();
            return listaResidentesPorFase;
        }


        public async Task<List<Object>> obtenerListaDocumentosPendientes()
        {
            List<Object> listaDocumentoPendientes = new List<Object>();
            var addFields1 = new BsonDocument("$addFields",
                                new BsonDocument("faseactual",
                                new BsonDocument("$arrayElemAt",
                                new BsonArray
                                            {
                                                "$progreso",
                                                -1
                                            })));
            var addFields2 = new BsonDocument("$addFields",
                                new BsonDocument("listadocumentos",
                                new BsonDocument("$concatArrays",
                                new BsonArray
                                            {
                                                "$faseactual.educativa.documentos",
                                                "$faseactual.social.documentos",
                                                "$faseactual.psicologica.documentos"
                                            })));
            var unwind = new BsonDocument("$unwind",
                                new BsonDocument("path", "$listadocumentos"));
            var group = new BsonDocument("$group",
                                new BsonDocument
                                    {
                                        { "_id", "$listadocumentos.tipo" },
                                        { "atrazados",
                                new BsonDocument("$sum",
                                new BsonDocument("$cond",
                                new BsonArray
                                                {
                                                    new BsonDocument("$eq",
                                                    new BsonArray
                                                        {
                                                            "$listadocumentos.estado",
                                                            "Pendiente"
                                                        }),
                                                    1,
                                                    0
                                                })) }
                                    });
            listaDocumentoPendientes = await _fases.Aggregate()
                .AppendStage<dynamic>(addFields1)
                .AppendStage<dynamic>(addFields2)
                .AppendStage<dynamic>(unwind)
                .AppendStage<Object>(group).ToListAsync();
            return listaDocumentoPendientes;
        }
        public async Task<List<Object>> obtenerDocumentosAtrazados()
        {
            List<Object> listaDocumentosAtrazados = new List<Object>();
            var addFields1 = new BsonDocument("$addFields",
                                new BsonDocument("faseactual",
                                new BsonDocument("$arrayElemAt",
                                new BsonArray
                                            {
                                                "$progreso",
                                                -1
                                            })));
            var addFields2 = new BsonDocument("$addFields",
                                new BsonDocument("listadocumentos",
                                new BsonDocument("$concatArrays",
                                new BsonArray
                                            {
                                                "$faseactual.educativa.documentos",
                                                "$faseactual.social.documentos",
                                                "$faseactual.psicologica.documentos"
                                            })));
            var unwind = new BsonDocument("$unwind",
                                new BsonDocument("path", "$listadocumentos"));
            var group = new BsonDocument("$group",
                                new BsonDocument
                                    {
                                        { "_id", "$faseactual.fase" },
                                        { "listadocumentos",
                                new BsonDocument("$push",
                                new BsonDocument("$cond",
                                new BsonArray
                                                {
                                                    new BsonDocument("$and",
                                                    new BsonArray
                                                        {
                                                            new BsonDocument("$eq",
                                                            new BsonArray
                                                                {
                                                                    "$listadocumentos.estado",
                                                                    "Pendiente"
                                                                }),
                                                            new BsonDocument("$lt",
                                                            new BsonArray
                                                                {
                                                                    "$listadocumentos.fechaestimada",
                                                                    "$$NOW"
                                                                })
                                                        }),
                                                    "$listadocumentos",
                                                    BsonNull.Value
                                                })) }
                                    });
            var project = new BsonDocument("$project",
                                    new BsonDocument
                                        {
                                            { "fase", "$fase" },
                                            { "listadocumentos",
                                    new BsonDocument("$setDifference",
                                    new BsonArray
                                                {
                                                    "$listadocumentos",
                                                    new BsonArray
                                                    {
                                                        BsonNull.Value
                                                    }
                                                }) }
                                        });
            var unwind2 = new BsonDocument("$unwind",
                                    new BsonDocument("path", "$listadocumentos"));
            var group2 = new BsonDocument("$group",
                                    new BsonDocument
                                        {
                                            { "_id", "$listadocumentos.tipo" },
                                            { "cantidadpendientes",
                                    new BsonDocument("$sum", 1) },
                                            { "fase",
                                    new BsonDocument("$first", "$_id") }
                                        });
            var group3 = new BsonDocument("$group",
                                    new BsonDocument
                                        {
                                            { "_id", "$fase" },
                                            { "documentosAtrazados",
                                    new BsonDocument("$push",
                                    new BsonDocument
                                                {
                                                    { "tipo", "$_id" },
                                                    { "cantidadpendientes", "$cantidadpendientes" }
                                                }) }
                                        });
            listaDocumentosAtrazados = await  _fases.Aggregate()
                .AppendStage<dynamic>(addFields1)
                .AppendStage<dynamic>(addFields2)
                .AppendStage<dynamic>(unwind)
                .AppendStage<dynamic>(group)
                .AppendStage<dynamic>(project)
                .AppendStage<dynamic>(unwind2)
                .AppendStage<dynamic>(group2)
                .AppendStage<Object>(group3)
                .ToListAsync();
                return listaDocumentosAtrazados;
        }
        
        public async Task<List<Object>> obtenerDocumentosPendientesHoy()
        {
            List<Object> listaDocumentosHoy = new List<Object>();
            var addFileds1 = new BsonDocument("$addFields",
                                new BsonDocument("faseactual",
                                new BsonDocument("$arrayElemAt",
                                new BsonArray
                                            {
                                                "$progreso",
                                                -1
                                            })));
            var addFileds2 = new BsonDocument("$addFields",
                                new BsonDocument("listadocumentos",
                                new BsonDocument("$concatArrays",
                                new BsonArray
                                            {
                                                "$faseactual.educativa.documentos",
                                                "$faseactual.social.documentos",
                                                "$faseactual.psicologica.documentos"
                                            })));
            var unwind = new BsonDocument("$unwind",
                                new BsonDocument("path", "$listadocumentos"));
            var group1 = new BsonDocument("$group",
                                new BsonDocument
                                    {
                                        { "_id", "$listadocumentos.tipo" },
                                        { "fase",
                                new BsonDocument("$first", "$faseactual.fase") },
                                        { "atrazados",
                                new BsonDocument("$sum",
                                new BsonDocument("$cond",
                                new BsonArray
                                                {
                                                    new BsonDocument("$and",
                                                    new BsonArray
                                                        {
                                                            new BsonDocument("$eq",
                                                            new BsonArray
                                                                {
                                                                    "$listadocumentos.estado",
                                                                    "Pendiente"
                                                                }),
                                                            new BsonDocument("$eq",
                                                            new BsonArray
                                                                {
                                                                    "$listadocumentos.fechaestimada",
                                                                    "$$NOW"
                                                                })
                                                        }),
                                                    1,
                                                    0
                                                })) }
                                    });
            var group2 = new BsonDocument("$group",
                                new BsonDocument
                                    {
                                        { "_id", "$fase" },
                                        { "documentospendientes",
                                new BsonDocument("$push",
                                new BsonDocument("$cond",
                                new BsonArray
                                                {
                                                    new BsonDocument("$ne",
                                                    new BsonArray
                                                        {
                                                            "$atrazados",
                                                            0
                                                        }),
                                                    new BsonDocument
                                                    {
                                                        { "documento", "$_id" },
                                                        { "atrazados", "$atrazados" }
                                                    },
                                                    BsonNull.Value
                                                })) }
                                    });
            var project = new BsonDocument("$project",
                                    new BsonDocument
                                        {
                                            { "_id", 1 },
                                            { "documentospendientes",
                                    new BsonDocument("$setDifference",
                                    new BsonArray
                                                {
                                                    "$documentospendientes",
                                                    new BsonArray
                                                    {
                                                        BsonNull.Value
                                                    }
                                                }) }
                                        });
            listaDocumentosHoy = await _fases.Aggregate()
                .AppendStage<dynamic>(addFileds1)
                .AppendStage<dynamic>(addFileds2)
                .AppendStage<dynamic>(unwind)
                .AppendStage<dynamic>(group1)
                .AppendStage<dynamic>(group2)
                .AppendStage<Object>(project).ToListAsync();
            return listaDocumentosHoy;
        }
    }
}
