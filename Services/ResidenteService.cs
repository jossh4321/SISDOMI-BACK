using MongoDB.Driver;
using SISDOMI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using SISDOMI.DTOs;

namespace SISDOMI.Services
{
    public class ResidenteService
    {
        private readonly IMongoCollection<Residentes> _residente;
        private readonly IMongoCollection<Documento> _documento;
        private readonly IMongoCollection<Expediente> _expedientes;

        public ResidenteService(ISysdomiDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _residente = database.GetCollection<Residentes>("residentes");
            _expedientes = database.GetCollection<Expediente>("expedientes");

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
        public Documento  GetByIdDoc(string id)
        {
            Documento documento = new Documento();
            documento = _documento.Find(documento => documento.idresidente == id).FirstOrDefault();
            return documento;
        }
        public async Task<Residentes> CreateUser(Residentes residente)
        {
            _residente.InsertOne(residente);
            Expediente expediente = new Expediente();
            expediente.idresidente = residente.id;
            expediente.fechainicio = residente.fechaIngreso;
            await saveExpediente(expediente);
            return residente;
        }
        public async Task saveExpediente(Expediente expediente)
        {
            Expediente exp  = await ObtenerUltimoExpediente();
            string[] arregloInicial = exp.numeroexpediente.Split(' ');
            int numeroExpediente = Int32.Parse(arregloInicial[1]);
            string numeroExpedienteFinal = $"EO {numeroExpediente+1}";
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


        public Residentes ModifyUser(Residentes residente)
        {
            var filter = Builders<Residentes>.Filter.Eq("id", residente.id);
            var update = Builders<Residentes>.Update
                .Set("nombre", residente.nombre)
                .Set("apellido", residente.apellido)
                .Set("tipodocumento", residente.tipoDocumento)
                .Set("numerodocumento", residente.numeroDocumento)
                .Set("lugarnacimiento", residente.lugarNacimiento)
                .Set("ubigeo", residente.ubigeo)
                .Set("juzgadoprocedencia", residente.juzgadoProcedencia)
                .Set("fechanacimiento", residente.fechaNacimiento)
                .Set("sexo", residente.sexo)
                .Set("telefonosreferencias", residente.telefonosReferencia)
                .Set("fechaingreso", residente.fechaIngreso)
                .Set("motivoingreso", residente.motivoIngreso)
                .Set("progreso", residente.progreso)
                .Set("estado", residente.estado);
            residente = _residente.FindOneAndUpdate<Residentes>(filter, update, new FindOneAndUpdateOptions<Residentes>
            {
                ReturnDocument = ReturnDocument.After
            });
            return residente;
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
    }
}