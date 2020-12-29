using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SISDOMI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.DTOs
{
    public class AnexoDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string titulo { get; set; }
        public string descripcion { get; set; }
        public string idresidente { get; set; }
        public string residente { get; set; }
        public string idcreador { get; set; }
        public string creador { get; set; }
        public DateTime? fechacreacion { get; set; }
        public string area { get; set; }
        public List<Enlace> enlaces { get; set; }
    }
}
