using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.DTOs
{
    public class PlanIntervencionDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public String id { get; set; }
        
        public DateTime? fechacreacion { get; set; }
        public String area { get; set; }
        public String fase { get; set; }
        public String estado { get; set; }
        public String titulo { get; set; }
        public String tipo { get; set; }
        public String residente { get; set; }
    }
}
