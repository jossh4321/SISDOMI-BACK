using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.DTOs
{
    public class InformeDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string tipo { get; set; }
        public DateTime? fechacreacion { get; set; }
        public string codigodocumento { get; set; }
        public string nombrecompleto { get; set; }
    }
}
