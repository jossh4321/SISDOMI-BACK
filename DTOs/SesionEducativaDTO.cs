using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SISDOMI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.DTOs
{
    public class SesionEducativaDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string titulo { get; set; }
        public DateTime fechacreacion { get; set; }
        public string area { get; set; }

        public DatosResidente datosresidente { get; set; }
    }

    public class DatosResidente
    {
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string nombrecompleto { get; set; }
        public string tipodocumento { get; set; }
        public string numerodocumento { get; set; }
        public string sexo { get; set; }
        public int faseactual { get; set; }
        public int acogida { get; set; }
    }
}
