using MongoDB.Driver;
using SISDOMI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using SISDOMI.DTOs;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using static SISDOMI.DTOs.ResidenteDTO;
using static SISDOMI.DTOs.ResidenteFasesDocumentosDTO;

namespace SISDOMI.Services
{
    public class ResidenteService
    {
        private readonly IMongoCollection<Residentes> _residente;
        private readonly IMongoCollection<Documento> _documento;
        private readonly IMongoCollection<Expediente> _expedientes;
        private readonly IMongoCollection<Fase> _documentofase;
        public ResidenteService(ISysdomiDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _residente = database.GetCollection<Residentes>("residentes");
            _expedientes = database.GetCollection<Expediente>("expedientes");
            _documentofase = database.GetCollection<Fase>("fases");

        }
        public List<Residentes> GetAll()
        {
            List<Residentes> residentes = new List<Residentes>();
            residentes = _residente.Find(Residentes => true).ToList();
            return residentes;
        }
        public Residentes GetById(string id)
        {
            Residentes residente = new Residentes();
            residente = _residente.Find(residente => residente.id == id).FirstOrDefault();
            return residente;
        }
        public Documento GetByIdDoc(string id)
        {
            Documento documento = new Documento();
            documento = _documento.Find(documento => documento.idresidente == id).FirstOrDefault();
            return documento;
        }
        public async Task<Residentes> CreateUser(ResidenteDTO2 residente)
        {
            Residentes res = new Residentes();
            //res.id = residente.id;
            res.nombre = residente.nombre;
            res.apellido = residente.apellido;
            res.tipoDocumento = residente.tipoDocumento;
            res.numeroDocumento = residente.numeroDocumento;
            res.lugarNacimiento = residente.lugarNacimiento;
            res.ubigeo = residente.ubigeo;
            res.juzgadoProcedencia = residente.juzgadoProcedencia;
            res.fechaNacimiento = residente.fechaNacimiento;
            res.sexo = residente.sexo;
            res.telefonosReferencia = residente.telefonosReferencia;
            res.fechaIngreso = residente.fechaIngreso;
            res.motivoIngreso = residente.motivoIngreso;
            res.progreso = residente.progreso;
            res.estado = residente.estado;
            _residente.InsertOne(res);
            Expediente expediente = new Expediente();
            var fase = generarProgresoFase(residente, res.id);
            expediente.idresidente = res.id;
            expediente.fechainicio = residente.fechaIngreso;
            await saveExpediente(expediente);
            _documentofase.InsertOne(fase);
            return res;
        }


        public Fase generarProgresoFase(ResidenteDTO2 residente, string id)
        {
            Fase fase = new Fase();
            fase.idresidente = id;
            fase.progreso = new List<ProgresoFase>();
            fase.progreso.Add(new ProgresoFase());
            fase.progreso[0].educativa = new ContenidoFase()
            {
                estado = "incompleto",
                documentos = new List<Documentos>()
                    {
                        new Documentos() { estado = "Pendiente", tipo = "FichaEducativaIngreso" },
                        new Documentos() { estado = "Pendiente", tipo = "InformeEducativoInicial" },
                        new Documentos() { estado = "Pendiente", tipo = "PlanIntervencionIndividualEducativo" },
                        new Documentos() { estado = "Pendiente", tipo = "InformeSeguimientoEducativo" },
                    }
            };
            if (residente.progreso.Count() > 1)
            {
                fase.progreso.Add(new ProgresoFase());
                if (residente.progreso[1].fase == 2)
                {
                    fase.progreso[1].educativa = new ContenidoFase()
                    {
                        estado = "incompleto",
                        documentos = new List<Documentos>()
                        {
                            new Documentos() { estado = "Pendiente", tipo = "PlanIntervencionIndividualEducativo" },
                            new Documentos() { estado = "Pendiente", tipo = "InformeEducativoEvolutivo" },
                        }
                    };
                }
                else if (residente.progreso[1].fase == 3)
                {
                    fase.progreso[1].educativa = new ContenidoFase()
                    {
                        estado = "incompleto",
                        documentos = new List<Documentos>()
                        {
                            new Documentos() { estado = "Pendiente", tipo = "InformeEducativoFinal" },
                        }
                    };
                }
            }
            if (residente.progreso.Count() > 2)
            {
                fase.progreso.Add(new ProgresoFase());
                fase.progreso[2].educativa = new ContenidoFase()
                {
                    estado = "icompleto",
                    documentos = new List<Documentos>()
                    {
                        new Documentos() { estado = "Pendiente", tipo = "InformeEducativoFinal" },
                    }
                };
            }
            if (residente.progreso.Count() > 3)
            {
                fase.progreso.Add(new ProgresoFase());
                fase.progreso[3].educativa = new ContenidoFase()
                {
                    estado = "icompleto",
                    documentos = new List<Documentos>()
                    {
                        new Documentos() { estado = "Pendiente", tipo = "NO SE QUE DOCUMENTO VA AQUI" },
                    }
                };
            }
            for (int i = 0; i < fase.progreso.Count(); i++)
            {
                fase.progreso[i].fase = residente.progreso[i].fase;
                fase.progreso[i].documentotransicion.fecha = residente.progreso[i].fechaingreso;
                fase.progreso[i].documentotransicion.idcreador = residente.idcreador;
                fase.progreso[i].documentotransicion.observaciones = residente.observaciones;
                fase.progreso[i].documentotransicion.firma = residente.firma;
            }
            return fase;
        }

        public async Task saveExpediente(Expediente expediente)
        {
            Expediente exp = await ObtenerUltimoExpediente();
            string[] arregloInicial = exp.numeroexpediente.Split(' ');
            int numeroExpediente = Int32.Parse(arregloInicial[1]);
            string numeroExpedienteFinal = $"EO {numeroExpediente + 1}";
            expediente.numeroexpediente = numeroExpedienteFinal;
            _expedientes.InsertOne(expediente);
        }

        public async Task<Expediente> ObtenerUltimoExpediente()
        {
            Expediente exp = new Expediente();
            var sort =
                new BsonDocument("$sort",
                new BsonDocument("_id", -1));
            var limit = new BsonDocument("$limit", 1);

            exp = await _expedientes.Aggregate()
                            .AppendStage<Expediente>(sort)
                            .AppendStage<Expediente>(limit)
                            .FirstAsync();
            return exp;
        }


        public Residentes ModifyUser(ResidenteFaseDTO residenteFase)
        {
            if (residenteFase.promocion == true)
            {
                Fase fase = new Fase();
                //fase.progreso.Add(residenteFase.progresoFase);
                var filter2 = Builders<Fase>.Filter.Eq("idresidente", residenteFase.residente.id);
                var update2 = Builders<Fase>.Update
                    .Set("progreso.$[elem].documentotransicion.fecha", residenteFase.progresoFase.documentotransicion.fecha)
                    .Set("progreso.$[elem].documentotransicion.idcreador", residenteFase.progresoFase.documentotransicion.idcreador)
                    .Set("progreso.$[elem].documentotransicion.observaciones", residenteFase.progresoFase.documentotransicion.observaciones)
                    .Set("progreso.$[elem].documentotransicion.firma.urlfirma", residenteFase.progresoFase.documentotransicion.firma.urlfirma)
                    .Set("progreso.$[elem].documentotransicion.firma.nombre", residenteFase.progresoFase.documentotransicion.firma.nombre)
                    .Set("progreso.$[elem].documentotransicion.firma.cargo", residenteFase.progresoFase.documentotransicion.firma.cargo);
                var arrayFilter = new List<ArrayFilterDefinition>();

                arrayFilter.Add(new BsonDocumentArrayFilterDefinition<Fase>(new BsonDocument("elem.fase", residenteFase.faseAnterior)));
                _documentofase.FindOneAndUpdate(filter2, update2, new FindOneAndUpdateOptions<Fase>
                {
                    ArrayFilters = arrayFilter
                });

                residenteFase.progresoFase.documentotransicion.fecha = new DateTime();
                residenteFase.progresoFase.documentotransicion.idcreador = "";
                residenteFase.progresoFase.documentotransicion.observaciones = "";
                residenteFase.progresoFase.documentotransicion.firma.urlfirma = "";
                residenteFase.progresoFase.documentotransicion.firma.nombre = "";
                residenteFase.progresoFase.documentotransicion.firma.cargo = "";

                var update3 = Builders<Fase>.Update.Push("progreso", residenteFase.progresoFase);

                _documentofase.FindOneAndUpdate(filter2,update3);
            }
            var filter = Builders<Residentes>.Filter.Eq("id", residenteFase.residente.id);
            var update = Builders<Residentes>.Update
                .Set("nombre", residenteFase.residente.nombre)
                .Set("apellido", residenteFase.residente.apellido)
                .Set("tipodocumento", residenteFase.residente.tipoDocumento)
                .Set("numerodocumento", residenteFase.residente.numeroDocumento)
                .Set("lugarnacimiento", residenteFase.residente.lugarNacimiento)
                .Set("ubigeo", residenteFase.residente.ubigeo)
                .Set("juzgadoprocedencia", residenteFase.residente.juzgadoProcedencia)
                .Set("fechanacimiento", residenteFase.residente.fechaNacimiento)
                .Set("sexo", residenteFase.residente.sexo)
                .Set("telefonosreferencias", residenteFase.residente.telefonosReferencia)
                .Set("fechaingreso", residenteFase.residente.fechaIngreso)
                .Set("motivoingreso", residenteFase.residente.motivoIngreso)
                .Set("progreso", residenteFase.residente.progreso)
                .Set("estado", residenteFase.residente.estado);
            residenteFase.residente = _residente.FindOneAndUpdate<Residentes>(filter, update, new FindOneAndUpdateOptions<Residentes>
            {
                ReturnDocument = ReturnDocument.After
            });



            return residenteFase.residente;
        }
        public async Task<List<Residentes>> GetResidenteByNombre(String nombre)
        {
            var filter = Builders<Residentes>.Filter.Regex("nombre", new BsonRegularExpression(nombre));
            return await _residente.Find(filter).ToListAsync();
        }
        public async Task<List<Residentes>> ListResidentByAreaAndByNotPlan(String areaPlan)
        {

            List<Residentes> lstResidentes;

            var lookUpDocuments = new BsonDocument("$lookup",
                                      new BsonDocument
                                      {
                                          { "from", "documentos" },
                                          { "let", new BsonDocument("residenteid", "$_id") },
                                          { "pipeline", new BsonArray
                                                        {
                                                            new BsonDocument("$match",
                                                                new BsonDocument("$expr",
                                                                new BsonDocument("$and",
                                                                new BsonArray
                                                                {
                                                                    new BsonDocument("$eq",
                                                                    new BsonArray
                                                                    {
                                                                        "$$residenteid",
                                                                        new BsonDocument("$toObjectId", "$idresidente")
                                                                    }),
                                                                    new BsonDocument("$in",
                                                                    new BsonArray
                                                                    {
                                                                        "$tipo",
                                                                        new BsonArray
                                                                        {
                                                                            "PlanIntervencionIndividualEducativo",
                                                                            "PlanIntervencionIndividualSocial",
                                                                            "PlanIntervencionIndividualPsicologico"
                                                                        }

                                                                    }),
                                                                    new BsonDocument("$eq",
                                                                    new BsonArray
                                                                    {
                                                                        "$area",
                                                                        areaPlan
                                                                    })
                                                                })))
                                                        }
                                          },
                                          { "as", "documentos" }
                                      });

            var projectDocumentByType = new BsonDocument("$project",
                                            new BsonDocument
                                            {
                                                { "nombre", 1 },
                                                { "apellido", 1 },
                                                { "tipodocumento", 1 },
                                                { "numerodocumento", 1 },
                                                { "fechanacimiento", 1 },
                                                { "sexo", 1 },
                                                { "motivoingreso", 1 },
                                                { "estado", 1 },
                                                { "progreso", 1 },
                                                { "documentos", 1 },
                                                {
                                                    "lastprogress", new BsonDocument("$arrayElemAt",
                                                                    new BsonArray
                                                                    {
                                                                        "$progreso",
                                                                        -1
                                                                    })
                                                }
                                            });


            var matchResidents = new BsonDocument("$match",
                                    new BsonDocument("$expr",
                                        new BsonDocument("$or",
                                        new BsonArray
                                        {
                                            new BsonDocument("$eq",
                                            new BsonArray
                                            {
                                                "$documentos",
                                                new BsonArray()
                                            }),
                                            new BsonDocument("$eq",
                                                new BsonArray
                                                {
                                                    new BsonDocument("$in",
                                                    new BsonArray
                                                    {
                                                        new BsonDocument("$toString", "$lastprogress.fase"),
                                                        "$documentos.fase"
                                                    }),
                                                    false
                                                })
                                        }
                                        )));

            var projectFinalResident = new BsonDocument("$project",
                                          new BsonDocument
                                          {
                                              { "nombre", 1 },
                                              { "apellido", 1 },
                                              { "tipodocumento", 1 },
                                              { "numerodocumento", 1 },
                                              { "fechanacimiento", 1 },
                                              { "sexo", 1 },
                                              { "motivoingreso", 1 },
                                              { "estado", 1 },
                                              { "progreso", 1 }
                                          });

            lstResidentes = await _residente.Aggregate()
                                    .AppendStage<dynamic>(lookUpDocuments)
                                    .AppendStage<dynamic>(projectDocumentByType)
                                    .AppendStage<dynamic>(matchResidents)
                                    .AppendStage<Residentes>(projectFinalResident)
                                    .ToListAsync();

            return lstResidentes;

        }

        public async Task<ResidenteDTO> GetResidentAndAnnexesAndDocuments(String idresidente)
        {
            ResidenteDTO residenteDTO;


            var matchResident = new BsonDocument("$match",
                                new BsonDocument("$expr",
                                new BsonDocument("$eq",
                                    new BsonArray
                                    {
                                        "$_id",
                                        new BsonDocument("$toObjectId", idresidente)
                                    })));


            var lookupAnnexes = new BsonDocument("$lookup",
                                new BsonDocument
                                {
                                    { "from", "anexos" },
                                    { "let",
                                        new BsonDocument("residenteid", "$_id")},
                                    { "pipeline",
                                        new BsonArray
                                        {
                                            new BsonDocument("$match",
                                            new BsonDocument("$expr",
                                            new BsonDocument("$eq",
                                                new BsonArray
                                                {
                                                    "$$residenteid",
                                                    new BsonDocument("$toObjectId", "$idresidente")
                                                })))
                                        }
                                    },
                                    { "as", "anexos" }
                                });


            var lookupDocuments = new BsonDocument("$lookup",
                                  new BsonDocument
                                  {
                                      { "from", "documentos" } ,
                                      { "let",
                                        new BsonDocument("residenteid", "$_id")
                                      },
                                      { "pipeline",
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
                                                            "$$residenteid",
                                                            new BsonDocument("$toObjectId", "$idresidente")
                                                        }),
                                                        new BsonDocument("$in",
                                                        new BsonArray
                                                        {
                                                            "$tipo",
                                                            new BsonArray
                                                            {
                                                                "PlanIntervencionIndividualPsicologico",
                                                                "PlanIntervencionIndividualSocial",
                                                                "PlanIntervencionIndividualEducativo",
                                                                "InformeEducativoInicial",
                                                                "InformeEducativoEvolutivo",
                                                                "InformeEducativoFinal",
                                                                "InformeSocialInicial",
                                                                "InformeSocialEvolutivo",
                                                                "InformeSocialFinal",
                                                                "InformePsicologicoInicial",
                                                                "InformePsicologicoEvolutivo",
                                                                "InformePsicologicoFinal"
                                                            }
                                                        })
                                                    }

                                                )))
                                            }
                                      },
                                      { "as", "documentos" }
                                  });

            var unwindDocuments = new BsonDocument("$unwind",
                                  new BsonDocument("path", "$documentos"));


            var groupDocumentsByArea = new BsonDocument("$group",
                                       new BsonDocument
                                       {
                                           { "_id", "$documentos.area" },
                                           { "cantidad", new BsonDocument("$sum", 1) },
                                           { "nombre", new BsonDocument("$first", "$nombre") },
                                           { "apellido", new BsonDocument("$first", "$apellido") },
                                           { "tipodocumento", new BsonDocument("$first", "$tipodocumento") },
                                           { "numerodocumento", new BsonDocument("$first", "$numerodocumento") },
                                           { "lugarnacimiento", new BsonDocument("$first", "$lugarnacimiento") },
                                           { "ubigeo", new BsonDocument("$first", "$ubigeo") },
                                           { "juzgadoprocedencia", new BsonDocument("$first", "$juzgadoprocedencia") },
                                           { "fechaingreso", new BsonDocument("$first", "$fechaingreso") },
                                           { "fechanacimiento", new BsonDocument("$first", "$fechanacimiento") },
                                           { "motivoingreso", new BsonDocument("$first", "$motivoingreso") },
                                           { "sexo", new BsonDocument("$first", "$sexo") },
                                           { "telefonosreferencias", new BsonDocument("$first", "$telefonosreferencias") },
                                           { "anexos", new BsonDocument("$first", "$anexos") },
                                           { "id", new BsonDocument("$first", "$_id") }
                                       });

            var projectDocumentArea = new BsonDocument("$project",
                                      new BsonDocument
                                      {
                                          { "_id", "$id" },
                                          { "nombre", 1 },
                                          { "apellido", 1 },
                                          { "tipodocumento", 1 },
                                          { "numerodocumento", 1 },
                                          { "lugarnacimiento", 1 },
                                          { "ubigeo", 1 },
                                          { "juzgadoprocedencia", 1 },
                                          { "fechaingreso", 1 },
                                          { "motivoingreso", 1 },
                                          { "sexo", 1 },
                                          { "fechanacimiento", 1 },
                                          { "telefonosreferencias", 1 },
                                          { "anexos", 1 },
                                          { "cantidaddocumentos",
                                            new BsonDocument
                                            {
                                                { "area", "$_id" },
                                                { "cantidad", "$cantidad" }
                                            }
                                          }
                                      });

            var groupFinal = new BsonDocument("$group",
                             new BsonDocument
                             {
                                 { "_id", "$_id" },
                                 { "nombre", new BsonDocument("$first", "$nombre") },
                                 { "apellido", new BsonDocument("$first", "$apellido") },
                                 { "tipodocumento", new BsonDocument("$first", "$tipodocumento") },
                                 { "numerodocumento", new BsonDocument("$first", "$numerodocumento") },
                                 { "lugarnacimiento", new BsonDocument("$first", "$lugarnacimiento") },
                                 { "ubigeo", new BsonDocument("$first", "$ubigeo") },
                                 { "juzgadoprocedencia", new BsonDocument("$first", "$juzgadoprocedencia") },
                                 { "fechaingreso", new BsonDocument("$first", "$fechaingreso") },
                                 { "fechanacimiento", new BsonDocument("$first", "$fechanacimiento") },
                                 { "motivoingreso", new BsonDocument("$first", "$motivoingreso") },
                                 { "sexo", new BsonDocument("$first", "$sexo") },
                                 { "telefonosreferencias", new BsonDocument("$first", "$telefonosreferencias") },
                                 { "anexos", new BsonDocument("$first", "$anexos") },
                                 { "cantidaddocumentos", new BsonDocument("$push", "$cantidaddocumentos") }

                             });

            ResidenteAnnexDocumentoDTO residenteAnnexDocumentoDTO = await _residente.Aggregate()
                                    .AppendStage<dynamic>(matchResident)
                                    .AppendStage<dynamic>(lookupAnnexes)
                                    .AppendStage<ResidenteAnnexDocumentoDTO>(lookupDocuments)
                                    .FirstOrDefaultAsync();

            if (residenteAnnexDocumentoDTO.documentos.Count == 0)
            {
                residenteDTO = new ResidenteDTO()
                {
                    id = residenteAnnexDocumentoDTO.id,
                    nombre = residenteAnnexDocumentoDTO.nombre,
                    apellido = residenteAnnexDocumentoDTO.apellido,
                    tipoDocumento = residenteAnnexDocumentoDTO.tipoDocumento,
                    numeroDocumento = residenteAnnexDocumentoDTO.numeroDocumento,
                    lugarNacimiento = residenteAnnexDocumentoDTO.lugarNacimiento,
                    ubigeo = residenteAnnexDocumentoDTO.ubigeo,
                    juzgadoProcedencia = residenteAnnexDocumentoDTO.juzgadoProcedencia,
                    fechaIngreso = residenteAnnexDocumentoDTO.fechaIngreso,
                    motivoIngreso = residenteAnnexDocumentoDTO.motivoIngreso,
                    anexos = residenteAnnexDocumentoDTO.anexos,
                    fechaNacimiento = residenteAnnexDocumentoDTO.fechaNacimiento,
                    sexo = residenteAnnexDocumentoDTO.sexo,
                    telefonosReferencia = residenteAnnexDocumentoDTO.telefonosReferencia,
                    cantidadDocumentos = new List<DocumentoAreaDTO>()

                };

                return residenteDTO;
            }


            residenteDTO = await _residente.Aggregate()
                                        .AppendStage<dynamic>(matchResident)
                                        .AppendStage<dynamic>(lookupAnnexes)
                                        .AppendStage<dynamic>(lookupDocuments)
                                        .AppendStage<dynamic>(unwindDocuments)
                                        .AppendStage<dynamic>(groupDocumentsByArea)
                                        .AppendStage<dynamic>(projectDocumentArea)
                                        .AppendStage<ResidenteDTO>(groupFinal)
                                        .FirstOrDefaultAsync();


            return residenteDTO;

        }

        public async Task<List<Residentes>> ListResidenteByFase(String fase)
        {

            List<Residentes> lstResidentes;

            var proyectinicial = new BsonDocument("$project",
                new BsonDocument
                    {
                        { "nombre", 1 },
                        { "apellido", 1 },
                        { "tipodocumento", 1 },
                        { "numerodocumento", 1 },
                        { "sexo", 1 },
                        { "lastprogreso",
                new BsonDocument("$arrayElemAt",
                new BsonArray
                            {
                                "$progreso",
                                -1
                            }) }
                    });

            var matchResidents = new BsonDocument("$match",
                new BsonDocument("lastprogreso.fase", Convert.ToInt32(fase)));

            var projectFinalResident = new BsonDocument("$project",
                new BsonDocument
                    {
                        { "_id", 1 },
                        { "nombre", 1 },
                        { "apellido", 1 },
                        { "sexo", 1 },
                        { "tipodocumento", 1 },
                        { "numerodocumento", 1 }
                    });

            lstResidentes = await _residente.Aggregate()
                                    .AppendStage<dynamic>(proyectinicial)
                                    .AppendStage<dynamic>(matchResidents)
                                    .AppendStage<Residentes>(projectFinalResident)
                                    .ToListAsync();

            return lstResidentes;

        }

        public async Task<List<Residentes>> ListResidentByFaseAndDocument(ResidenteFaseDocumentoDTO dtoFase)
        {
            if(dtoFase.estadodocumentoactual == null)
            {
                dtoFase.estadodocumentoactual = "Completo";
            }
            if(dtoFase.fasedocumentoanterior == null)
            {
                dtoFase.fasedocumentoanterior = "";
            }
            else
            {
                dtoFase.fasedocumentoanterior = (Convert.ToInt32(dtoFase.fasedocumentoanterior)-1) + ".";
            }
            string fase = Convert.ToString(Convert.ToInt32(dtoFase.fase) - 1);
            List<Residentes> listResidentes;

            var fields = new BsonDocument("$addFields",
                         new BsonDocument("residenteid",
                         new BsonDocument("$toString", "$_id")));

            var lookup = new BsonDocument("$lookup",
                         new BsonDocument
                            {
                                { "from", "fases" },
                                { "localField", "residenteid" },
                                { "foreignField", "idresidente" },
                                { "as", "fases" }
                            });

            var unwind = new BsonDocument("$unwind",
                         new BsonDocument
                         {
                            { "path", "$fases" },
                            { "preserveNullAndEmptyArrays", true }
                         });
            var addfields = new BsonDocument("$addFields",
                            new BsonDocument("ultimafase",
                            new BsonDocument("$arrayElemAt",
                            new BsonArray
                            {
                                "$progreso",
                                -1
                            })));
            var match = new BsonDocument("$match",
                        new BsonDocument("$and",
                        new BsonArray
                        {
                            new BsonDocument("ultimafase.fase", Convert.ToInt32(dtoFase.fase)),
                            new BsonDocument("fases.progreso."+dtoFase.area+".estado", "incompleto"),
                            new BsonDocument("fases.progreso."+dtoFase.fasedocumentoanterior+dtoFase.area+".documentos",
                            new BsonDocument("$in",
                            new BsonArray
                            {
                                new BsonDocument
                                {
                                    { "tipo", dtoFase.documentoanterior },
                                    { "estado", dtoFase.estadodocumentoanterior }
                                }
                            })),
                            new BsonDocument("fases.progreso."+fase+"."+dtoFase.area+".documentos",
                            new BsonDocument("$not",
                            new BsonDocument("$in",
                            new BsonArray
                            {
                                new BsonDocument
                                {
                                    { "tipo", dtoFase.documentoactual },
                                    { "estado", dtoFase.estadodocumentoactual }
                                }
                            })))
                        }));

            var project = new BsonDocument("$project",
                          new BsonDocument
                          {
                            { "fases", 0 },
                            { "residenteid", 0 },
                            { "ultimafase", 0 }
                          });

            listResidentes = await _residente.Aggregate()
                                    .AppendStage<dynamic>(fields)
                                    .AppendStage<dynamic>(lookup)
                                    .AppendStage<dynamic>(unwind)
                                    .AppendStage<dynamic>(addfields)
                                    .AppendStage<dynamic>(match)
                                    .AppendStage<Residentes>(project)
                                    .ToListAsync();

            return listResidentes;

        }

        public async Task<List<Residentes>> ListResidentesByFasesAndEvalutesDocuments(ResidenteFasesDocumentosDTO residenteFasesDocumentosDTO)
        {
            List<Residentes> lstResidentes;

            var lookupFases = new BsonDocument("$lookup",
                              new BsonDocument
                              {
                                  { "from", "fases" },
                                  { "let", new BsonDocument("residenteid", "$_id")},
                                  { "pipeline",
                                    new BsonArray { 
                                        new BsonDocument("$match",
                                        new BsonDocument("$expr",
                                        new BsonDocument("$eq",
                                        new BsonArray {
                                            "$$residenteid",
                                            new BsonDocument("$toObjectId", "$idresidente")
                                        })))
                                    } 
                                  },
                                  { "as", "fases" }
                              });

            var unwindFase = new BsonDocument("$unwind",
                             new BsonDocument
                             {
                                 { "path", "$fases" },
                                 { "preserveNullAndEmptyArrays", true }
                             });

            var addProgressField = new BsonDocument("$addFields",
                                   new BsonDocument
                                   {
                                       { "ultimafase",
                                         new BsonDocument("$arrayElemAt",
                                         new BsonArray {
                                             "$progreso",
                                             -1
                                         })
                                       },
                                       {
                                           "fases",
                                           new BsonDocument("$arrayElemAt",
                                           new BsonArray
                                           {
                                               "$fases.progreso",
                                               -1
                                           })
                                       }
                                   });

            BsonArray bsonArray = new BsonArray();

            residenteFasesDocumentosDTO.fases.ForEach((fase) =>
            {
                bsonArray.Add(fase);
            });


            var bsonDocumentFases = RegisterCondDocumentType(residenteFasesDocumentosDTO, 0);


            var matchFasesAndDocumentType = new BsonDocument("$match",
                                            new BsonDocument("$expr",
                                            new BsonDocument("$and",
                                            new BsonArray
                                            {
                                                new BsonDocument("$in",
                                                new BsonArray
                                                {
                                                    "$ultimafase.fase",
                                                    bsonArray
                                                }),
                                                new BsonDocument("$eq",
                                                new BsonArray
                                                {
                                                    "$fases." + residenteFasesDocumentosDTO.area  + ".estado",
                                                    "incompleto"
                                                }),
                                                bsonDocumentFases
                                            })));

            var projectResidents = new BsonDocument("$project",
                                   new BsonDocument
                                   {
                                       { "fases", 0 },
                                       { "ultimafase", 0 }
                                   });

            lstResidentes = await _residente.Aggregate()
                                    .AppendStage<dynamic>(lookupFases)
                                    .AppendStage<dynamic>(unwindFase)
                                    .AppendStage<dynamic>(addProgressField)
                                    .AppendStage<dynamic>(matchFasesAndDocumentType)
                                    .AppendStage<Residentes>(projectResidents)
                                    .ToListAsync();

            return lstResidentes;
        }


        public BsonDocument RegisterCondDocumentType(ResidenteFasesDocumentosDTO residenteFasesDocumentosDTO, Int32 index)
        {
            if(index == residenteFasesDocumentosDTO.documentoEstadosAnteriores.Count)
            {
                return new BsonDocument();
            }
            else
            {
                return new BsonDocument("$cond",
                new BsonArray
                {
                    new BsonDocument("$eq",
                    new BsonArray {
                          "$ultimafase.fase",
                          index + 1
                    }),
                    new BsonDocument("$and",
                    new BsonArray {
                        new BsonDocument("$in",
                        new BsonArray { 
                            new BsonDocument {
                                { "tipo", residenteFasesDocumentosDTO.documentoEstadosAnteriores.ElementAt(index).tipo },
                                { "estado", residenteFasesDocumentosDTO.documentoEstadosAnteriores.ElementAt(index).estado }
                            },
                            "$fases." + residenteFasesDocumentosDTO.area + ".documentos"
                        }),
                        new BsonDocument("$not",
                        new BsonDocument("$in",
                        new BsonArray { 
                            new BsonDocument
                            {
                                { "tipo", residenteFasesDocumentosDTO.documentoEstadosActuales.ElementAt(index).tipo },
                                { "estado", residenteFasesDocumentosDTO.documentoEstadosActuales.ElementAt(index).estado }
                            },
                             "$fases." + residenteFasesDocumentosDTO.area + ".documentos"
                        }))
                    }),
                    RegisterCondDocumentType(residenteFasesDocumentosDTO, index + 1)
                });
            }
        }

        public async Task<Object> getResidenteProgresoDTO(string idresidente)
        {
                    var match = new BsonDocument("$match",
                                    new BsonDocument("_id",
                                    new ObjectId(idresidente)));
                    var lookup1 = new BsonDocument("$lookup",
                                        new BsonDocument
                                            {
                                            { "from", "fases" },
                                            { "let",
                                    new BsonDocument("idres", "$_id") },
                                            { "pipeline",
                                    new BsonArray
                                            {
                                                new BsonDocument("$match",
                                                new BsonDocument("$expr",
                                                new BsonDocument("$eq",
                                                new BsonArray
                                                            {
                                                                "$idresidente",
                                                                new BsonDocument("$toString", "$$idres")
                                                            })))
                                            } },
                                            { "as", "fases" }
                                            });
                    var unwind1 = new BsonDocument("$unwind",
                                        new BsonDocument("path", "$fases"));
                    var lookup2 = new BsonDocument("$lookup",
                                        new BsonDocument
                                            {
                                            { "from", "expedientes" },
                                            { "let",
                                    new BsonDocument("idres", "$_id") },
                                            { "pipeline",
                                    new BsonArray
                                            {
                                                new BsonDocument("$match",
                                                new BsonDocument("$expr",
                                                new BsonDocument("$eq",
                                                new BsonArray
                                                            {
                                                                "$idresidente",
                                                                new BsonDocument("$toString", "$$idres")
                                                            })))
                                            } },
                                            { "as", "expediente" }
                                            });
                    var unwind2 = new BsonDocument("$unwind",
                                    new BsonDocument("path", "$expediente"));

            Object listaResidenteProgresoDTO = await _residente.Aggregate()
                    .AppendStage<dynamic>(match)
                    .AppendStage<dynamic>(lookup1)
                    .AppendStage<dynamic>(unwind1)
                    .AppendStage<dynamic>(lookup2)
                    .AppendStage<Object>(unwind2).SingleOrDefaultAsync();
            return listaResidenteProgresoDTO;

            }

    }
}