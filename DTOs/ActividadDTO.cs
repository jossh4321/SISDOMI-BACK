using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SISDOMI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.DTOs
{
    public class ActividadDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string tipo { get; set; }
        public string descripcion { get; set; }
        public DateTime fechacreacion { get; set; }
        public string creadordocumento { get; set; }
        public Object contenido { get; set; }
    }

    public class ActividadDTOConsulta : ActividadDTO
    {
        public string nombrecreador { get; set; }
        public int totalparticipantes { get; set; }
    }
}
