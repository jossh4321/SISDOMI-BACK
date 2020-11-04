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
                .Set("tipodocumento", residente.tipodocumento)
                .Set("numerodocumento", residente.numerodocumento)
                .Set("lugarnacimiento", residente.lugarnacimiento)
                .Set("ubigeo", residente.ubigeo)
                .Set("juzgadoprocedencia", residente.juzgadoprocedencia)
                .Set("fechanacimiento", residente.fechanacimiento)
                .Set("sexo", residente.sexo)
                .Set("telefonosreferencia", residente.telefonosreferencia)
                .Set("fechaingreso", residente.fechaingreso)
                .Set("motivoingreso", residente.motivoingreso)
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