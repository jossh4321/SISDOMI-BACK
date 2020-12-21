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
    public class UbigeoService
    {
        private readonly IMongoCollection<Ubigeo.Departamento> _departamentos;
        private readonly IMongoCollection<Ubigeo.Provincia> _provincias;
        private readonly IMongoCollection<Ubigeo.Distrito> _distritos;
        
        public UbigeoService(ISysdomiDatabaseSettings settings)
        {
            var client     = new MongoClient(settings.ConnectionString);
            var database   = client.GetDatabase(settings.DatabaseName);
            _departamentos = database.GetCollection<Ubigeo.Departamento>("departamentos");
            _provincias    = database.GetCollection<Ubigeo.Provincia>("provincias");
            _distritos     = database.GetCollection<Ubigeo.Distrito>("distritos");
            
        }

        public List<Ubigeo.Departamento> GetAllDepartamentos()
        {
            List<Ubigeo.Departamento> departamentos = new List<Ubigeo.Departamento>();
            departamentos = _departamentos.Find(Departamento => true).ToList();
            return departamentos;
        }
        public List<Ubigeo.Provincia> GetAllProvincias()
        {
            List<Ubigeo.Provincia> provincias = new List<Ubigeo.Provincia>();
            provincias = _provincias.Find(Provincia => true).ToList();
            return provincias;
        }
        public List<Ubigeo.Distrito> GetAllDistritos()
        {
            List<Ubigeo.Distrito> distritos = new List<Ubigeo.Distrito>();
            distritos = _distritos.Find(Distrito => true).ToList();
            return distritos;
        }

        public async Task<List<Ubigeo.Provincia>> GetProvincesByIdDepartment(string idDepartamento)
        {            
            List<Ubigeo.Provincia> provincias = new List<Ubigeo.Provincia>();
            provincias = await _provincias.Find(provincias => provincias.idDepartamento == idDepartamento).ToListAsync();
            return provincias;
        }
        public Ubigeo.Distrito GetDistrictById(string idDistrito)
        {
            Ubigeo.Distrito distrito = new Ubigeo.Distrito();
            distrito = _distritos.Find(distrito => distrito.idDistrito == idDistrito).FirstOrDefault();
            return distrito;
        }

        public Ubigeo.Provincia GetProvinceById(string idProvincia)
        {
            Ubigeo.Provincia provincia= new Ubigeo.Provincia();
            provincia = _provincias.Find(provincia => provincia.idProvincia == idProvincia).FirstOrDefault();
            return provincia;
        }

        public async Task<List<Ubigeo.Distrito>> GetDistrictsByIdProvince(string idProvincia)
        {
            List<Ubigeo.Distrito> distritos= new List<Ubigeo.Distrito>();
            distritos= await _distritos.Find(distritos=> distritos.idProvincia == idProvincia).ToListAsync();
            return distritos;
        }




    }
}
