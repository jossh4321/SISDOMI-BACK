using MongoDB.Bson;
using MongoDB.Driver;
using SISDOMI.DTOs;
using SISDOMI.Entities;
using SISDOMI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Services
{
    public class DocumentoService
    {
        private readonly IMongoCollection<Documento> _documentos;
        
        public DocumentoService(ISysdomiDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _documentos = database.GetCollection<Documento>("documentos");

        }

        public async Task<List<DocumentTypeResidentDTO>> ListDocumentsByTypeAndResident(String type, String residenteid)
        {
            List<DocumentTypeResidentDTO> lstDocumentTypeResidentDTOs;
            String area = "";

            switch (type)
            {
                case "educativa":
                    area = "Educativo";
                    break;
                case "psicologica":
                    area = "Psicologico";
                    break;
                case "social":
                    area = "Social";
                    break;
                default:
                    break;
            }


            var matchDocuments = new BsonDocument("$match",
                                 new BsonDocument("$expr",
                                 new BsonDocument("$and",
                                       new BsonArray
                                       {
                                           new BsonDocument("$eq",
                                           new BsonArray
                                           {
                                               "$area",
                                               type
                                           }),
                                           new BsonDocument("$eq",
                                           new BsonArray
                                           {
                                               "$idresidente",
                                               residenteid
                                           }),
                                           new BsonDocument("$in",
                                           new BsonArray
                                           {
                                               "$tipo",
                                               new BsonArray
                                               {
                                                   "Informe" + area + "Inicial",
                                                   "Informe" + area + "Evolutivo",
                                                   "Informe" + area + "Final",
                                                   "PlanIntervencionIndividual" + area
                                               }
                                           })

                                       })));

            var groupDocuments = new BsonDocument("$group",
                                 new BsonDocument
                                 {
                                     { "_id", "$tipo" },
                                     { "documentos",
                                        new BsonDocument("$push",
                                        new BsonDocument("$toString", "$_id"))
                                     }
                                 });

            var projectDocuments = new BsonDocument("$project",
                                    new BsonDocument
                                    {
                                        { "_id", 0 },
                                        { "tipo", "$_id" },
                                        { "documentos", 1 }
                                    });

            lstDocumentTypeResidentDTOs = await _documentos.Aggregate()
                                                        .AppendStage<dynamic>(matchDocuments)
                                                        .AppendStage<dynamic>(groupDocuments)
                                                        .AppendStage<DocumentTypeResidentDTO>(projectDocuments)
                                                        .ToListAsync();
                                            
            return lstDocumentTypeResidentDTOs;
        }

        public async Task<DocumentoExpedienteDTO> GetById(String id)
        {
            DocumentoExpedienteDTO documentoExpedienteDTO;

            var matchId = new BsonDocument("$match",
                                           new BsonDocument("$expr",
                                                    new BsonDocument("$eq", new BsonArray
                                                    {
                                                        "$_id",
                                                        new BsonDocument("$toObjectId", id)
                                                    })));

            var projectGeneralDocument = new BsonDocument("$project", new BsonDocument
            {
                { "tipo", 1},
                { "creadordocumento", 1 },
                {"idresidente", 1 },
                { "area", 1  },
                { "fase", 1  },
                { "estado", 1  },
                { "contenido", 1 },
                { "fechacreacion", 1 }
            });


            var lookupResidente = new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "residentes" },
                { "let", new BsonDocument("residenteId", "$idresidente") },
                { "pipeline", new BsonArray
                              {
                                 new BsonDocument("$match",
                                 new BsonDocument("$expr",
                                 new BsonDocument("$eq",
                                 new BsonArray {
                                     "$_id",
                                     new BsonDocument("$toObjectId", "$$residenteId")
                                 })))
                              }
                },
                { "as", "residente" }
            });

            var unwindResidente = new BsonDocument("$unwind", new BsonDocument("path", "$residente"));

            var lookupUsuario = new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "usuarios"},
                { "let", new BsonDocument("usuarioId", "$creadordocumento") },
                { "pipeline", new BsonArray
                              {
                                 new BsonDocument("$match",
                                 new BsonDocument("$expr",
                                 new BsonDocument("$eq",
                                 new BsonArray {
                                     "$_id",
                                     new BsonDocument("$toObjectId", "$$usuarioId")
                                 })))
                              }
                },
                { "as", "creador" }
            });

            var unwindUsuario = new BsonDocument("$unwind", new BsonDocument("path", "$creador"));

            var projectFinalData = new BsonDocument("$project", new BsonDocument
            {
                { "tipo", 1  },
                {"area", 1 },
                {"fase", 1 },
                {"estado", 1 },
                { "contenido", 1},
                { "fechacreacion", 1 },
                { "residente", 1},
                { "creador", new BsonDocument("$concat", new BsonArray{
                                                            "$creador.datos.nombre",
                                                            " ",
                                                            "$creador.datos.apellido"
                                                         })
                }
            });

            documentoExpedienteDTO = await _documentos.Aggregate()
                                            .AppendStage<dynamic>(matchId)
                                            .AppendStage<dynamic>(projectGeneralDocument)
                                            .AppendStage<dynamic>(lookupResidente)
                                            .AppendStage<dynamic>(unwindResidente)
                                            .AppendStage<dynamic>(lookupUsuario)
                                            .AppendStage<dynamic>(unwindUsuario)
                                            .AppendStage<DocumentoExpedienteDTO>(projectFinalData)
                                            .FirstOrDefaultAsync();

            return documentoExpedienteDTO;
        }

        public async Task<FichaIngresoDetalleDTO> getFichaIngresoDetalleDtoById(string id)
        {
            var match = new BsonDocument("$match",
                           new BsonDocument("_id",
                           new ObjectId(id)));
            var lookup1 = new BsonDocument("$lookup",
                             new BsonDocument
                                 {
                                    { "from", "residentes" },
                                    { "let",
                            new BsonDocument("idres", "$idresidente") },
                                    { "pipeline",
                            new BsonArray
                                    {
                                        new BsonDocument("$match",
                                        new BsonDocument("$expr",
                                        new BsonDocument("$eq",
                                        new BsonArray
                                                    {
                                                        "$_id",
                                                        new BsonDocument("$toObjectId", "$$idres")
                                                    })))
                                    } },
                                    { "as", "residente" }
                                 });
            var lookup2 = new BsonDocument("$lookup",
                                new BsonDocument
                                    {
                                        { "from", "usuarios" },
                                        { "let",
                                new BsonDocument("idcrea", "$creadordocumento") },
                                        { "pipeline",
                                new BsonArray
                                        {
                                            new BsonDocument("$match",
                                            new BsonDocument("$expr",
                                            new BsonDocument("$eq",
                                            new BsonArray
                                                        {
                                                            "$_id",
                                                            new BsonDocument("$toObjectId", "$$idcrea")
                                                        })))
                                        } },
                                        { "as", "autor" }
                                    });
            var project = new BsonDocument("$project",
                                new BsonDocument
                                    {
                                        { "_id", "$_id" },
                                        { "tipo", "$tipo" },
                                        { "historialcontenido", "$historialcontenido" },
                                        { "creadordocumento",
                                new BsonDocument("$arrayElemAt",
                                new BsonArray
                                            {
                                                "$autor",
                                                0
                                            }) },
                                        { "fechacreacion", "$fechacreacion" },
                                        { "area", "$area" },
                                        { "fase", "$fase" },
                                        { "residente",
                                new BsonDocument("$arrayElemAt",
                                new BsonArray
                                            {
                                                "$residente",
                                                0
                                            }) },
                                        { "contenido", "$contenido" }
                                    });
            FichaIngresoDetalleDTO fichaIngresoDetalle = await _documentos.Aggregate()
                .AppendStage<dynamic>(match)
                .AppendStage<dynamic>(lookup1)
                                .AppendStage<dynamic>(lookup2)
                                .AppendStage<FichaIngresoDetalleDTO>(project).FirstOrDefaultAsync();
            return fichaIngresoDetalle;



    }
    }
}
