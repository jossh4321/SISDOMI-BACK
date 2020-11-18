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
    public class SeguimientoEducativoService
    {
        private readonly IMongoCollection<Documento> _documento;
        private readonly IMongoCollection<Expediente> _expediente;
    
        public SeguimientoEducativoService(ISysdomiDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _documento = database.GetCollection<Documento>("documentos");
            _expediente = database.GetCollection<Expediente>("expedientes");
        }
        public List<Documento> GetAll()
        {
            List<Documento> documento = new List<Documento>();
            documento = _documento.Find(Documento => true).ToList();
            return documento;
          
        }
    }
}
