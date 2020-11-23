using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SISDOMI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.DTOs
{
    public class TallerEscuelaPadres
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public String id { get; set; }
        public DateTime fechacreacion { get; set; }
        public DateTime fechainicio { get; set; }
        public DateTime fechafin { get; set; }
        public String titulo { get; set; }
        public String tipo { get; set; }
        public String descripcion { get; set; }
        public String creadordocumento { get; set; }
        public List<Tutor> tutores { get; set; }
    }
}
