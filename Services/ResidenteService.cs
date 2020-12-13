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
        public async Task<Residentes> CreateUser(Residentes residente)
        {
            _residente.InsertOne(residente);
            Expediente expediente = new Expediente();
            Fase fase = new Fase();



            Usuario usuario = new Usuario();
            expediente.idresidente = residente.id;
            expediente.fechainicio = residente.fechaIngreso;
            fase.idresidente = residente.id;
            fase.progreso = new List<ProgresoFase>();
            fase.progreso.Add(new ProgresoFase());
            fase.progreso[0].fase = residente.progreso[0].fase;
            fase.progreso[0].documentotransicion.fecha = residente.progreso[0].fechaingreso;
            //fase.progreso[0].documentotransicion.idcreador = usuario.id;
            await saveExpediente(expediente);
            _documentofase.InsertOne(fase);
            return residente;
        }


        /*public Fase generarProgresoFase()
        {
            List<ProgresoFase> progreso = new List<ProgresoFase>() { new ProgresoFase() 
            { documentotransicion= new DocumentacionTransicion(),
                fase=1,educativa= new ContenidoFase() { estado="icompleto", documentos = new List<Documentos>(){ new Documentos() { estado="Pendiente", tipo=""} } } } }

        }*/

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
            if(residenteFase.promocion == true)
            {
                Fase fase = new Fase();
                //fase.progreso.Add(residenteFase.progresoFase);
                var filter2 = Builders<Fase>.Filter.Eq("idresidente", residenteFase.residente.id);
                var update2 = Builders<Fase>.Update
                    .Push("progreso", residenteFase.progresoFase);
                _documentofase.FindOneAndUpdate<Fase>(filter2, update2);
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
                                                                    new BsonDocument("$eq",
                                                                    new BsonArray
                                                                    {
                                                                        "$tipo",
                                                                        "PlanIntervencionIndividual"
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
                                                        "$lastprogress.nombre",
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
                                                                "PlanIntervencionIndividual",
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
    }
}