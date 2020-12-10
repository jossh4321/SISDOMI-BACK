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

namespace SISDOMI.Services{
    public class FichaEvaluacionDiagnosticoEducativoService{
        private readonly IMongoCollection<Documento> documento;
        private readonly IMongoCollection<FichaEvaluacionDiagnosticoEducativo> evaluacion;
        private readonly IMongoCollection<Expediente> expediente;
        private readonly ExpedienteService expedienteService;
        private readonly IDocument idocumento;

        public FichaEvaluacionDiagnosticoEducativoService(ISysdomiDatabaseSettings settings, IDocument idocumento, ExpedienteService expedienteService)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            this.documento = database.GetCollection<Documento>("documentos");
            this.expediente = database.GetCollection<Expediente>("expedientes");
            this.expedienteService = expedienteService;
            this.idocumento = idocumento;
        }

        public List<FichaEvaluacionDiagnosticoEducativo> GetAll(){
            /*
            List<FichaEvaluacionDiagnosticoEducativo> listevaluacion = new List<FichaEvaluacionDiagnosticoEducativo>();
            listevaluacion = evaluacion.AsQueryable().OfType<FichaEvaluacionDiagnosticoEducativo>().ToList();
            return listevaluacion;*/
            return this.documento.AsQueryable().OfType<FichaEvaluacionDiagnosticoEducativo>().ToList();
        }

        public async Task<DocumentoDTO> GetById(string id){
            /*
            FichaEvaluacionDiagnosticoEducativo evaluacion = new FichaEvaluacionDiagnosticoEducativo(); 
            evaluacion = this.evaluacion.AsQueryable().OfType<FichaEvaluacionDiagnosticoEducativo>().ToList().Find(evaluacion => evaluacion.id == id);
            return evaluacion;
            */
            //return this.documento.AsQueryable().OfType<FichaEvaluacionDiagnosticoEducativo>().ToList().Find(evaluacion => evaluacion.id == id);
            var match = new BsonDocument("$match",
                        new BsonDocument("_id",
                        new ObjectId(id)));

            DocumentoDTO documento = new DocumentoDTO();
            documento = await this.documento.Aggregate()
                            .AppendStage<DocumentoDTO>(match)
                            .FirstAsync();
            return documento;
        }

        public async Task<FichaEvaluacionDiagnosticoEducativo> CreateEvaluation(FichaEvaluacionDiagnosticoEducativo evaluacion){
            /*
            this.evaluacion.InsertOne(evaluacion);
            return evaluacion;
            */
            DateTime DateNow = DateTime.UtcNow.AddHours(-5);
            Expediente expediente = await expedienteService.GetByResident(evaluacion.idresidente);
            evaluacion.contenido.codigodocumento = idocumento.CreateCodeDocument(DateNow, evaluacion.tipo, expediente.documentos.Count + 1);
            await documento.InsertOneAsync(evaluacion);
            DocumentoExpediente docexpe = new DocumentoExpediente(){
                tipo = evaluacion.tipo,
                iddocumento = evaluacion.id
            };
            UpdateDefinition<Expediente> updateExpediente = Builders<Expediente>.Update.Push("documentos", docexpe);
            this.expediente.FindOneAndUpdate(x => x.idresidente == evaluacion.idresidente, updateExpediente);
            return evaluacion;
        }

        public FichaEvaluacionDiagnosticoEducativo ModifyEvaluation(FichaEvaluacionDiagnosticoEducativo evaluacion)
        {
            var filter = Builders<Documento>.Filter.Eq("id", evaluacion.id);
            var update = Builders<Documento>.Update
                .Set("tipo", evaluacion.tipo)
                .Set("historialcontenido", evaluacion.historialcontenido)
                .Set("creadordocumento", evaluacion.creadordocumento)
                .Set("fechacreacion", evaluacion.fechacreacion)
                .Set("area", evaluacion.area)
                .Set("fase", evaluacion.fase)
                .Set("estado", evaluacion.estado)
                .Set("contenido", evaluacion.contenido);
            /*var doc = this.documento.FindOneAndUpdate<Documento>(filter, update, new FindOneAndUpdateOptions<Documento>{
                ReturnDocument = ReturnDocument.After
            });*/
            /*
            evaluacion = doc as FichaEvaluacionDiagnosticoEducativo;
            return evaluacion;
            */
            this.documento.UpdateOne(filter, update);
            return evaluacion;
        }
    }
}
