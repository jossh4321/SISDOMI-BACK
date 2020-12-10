using MongoDB.Bson;
using MongoDB.Driver;
using SISDOMI.DTOs;
using SISDOMI.Entities;
using SISDOMI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace SISDOMI.Services
{
    public class FichaIngresoSocialService
    {
        private readonly IMongoCollection<Documento> _documentos;

        public FichaIngresoSocialService(ISysdomiDatabaseSettings settings){
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _documentos = database.GetCollection<Documento>("documentos");
        }

        public List<FichaIngresoSocial> GetAll()
        {
            List<FichaIngresoSocial> listFichaIngresoSocial = new List<FichaIngresoSocial>();

            listFichaIngresoSocial = _documentos.AsQueryable().OfType<FichaIngresoSocial>().ToList();

            return listFichaIngresoSocial;
        }
        //
        public FichaIngresoSocial CreateFichaIngresoSocial(FichaIngresoSocial documento)
        {
            _documentos.InsertOne(documento);
            return documento;
        }
        public FichaIngresoSocial GetById(string id)
        {
            FichaIngresoSocial documento = new FichaIngresoSocial();
            documento = _documentos.AsQueryable().OfType<FichaIngresoSocial>().ToList().Find(documento => documento.id == id);
            return documento;
        }
        public FichaIngresoSocial ModifyFichaIngresoSocial(FichaIngresoSocial documento)
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
            return documento;
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

        public async Task<List<FichaIngresoDTO>> obtenerResidientesFichaIngreso()
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
                 .AppendStage<dynamic>(lookup_fichaIngreso)
                   .AppendStage<dynamic>(unwindFicha)
                 .AppendStage<FichaIngresoDTO>(project).ToListAsync();



            return fichaIngreso;
        }
       

    }

}
