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
namespace SISDOMI.Services
{
    public class SesionesEducativasService
    {
        //mapear sesiones
        private readonly IMongoCollection<SesionEducativa> _sesioneducativa;

        public List<SesionEducativa> GetAll()
        {
            List<SesionEducativa> sesionEducativa = new List<SesionEducativa>();
            sesionEducativa = _sesioneducativa.Find(sesionEducativa => true).ToList();
            return sesionEducativa;
        }
    }
}
