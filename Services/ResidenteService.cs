using MongoDB.Driver;
using SISDOMI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Services
{
    public class ResidenteService
    {
        private readonly IMongoCollection<Residentes> _residente;
        private readonly IMongoCollection<Documento> _documento;

        public ResidenteService(ISysdomiDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _residente = database.GetCollection<Residentes>("residentes");

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
        public Residentes CreateUser(Residentes residente)
        {
            _residente.InsertOne(residente);
            return residente;
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
                .Set("telefonosreferencia", residente.telefonosReferencia)
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
    }
}