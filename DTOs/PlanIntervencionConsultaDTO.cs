using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using SISDOMI.Entities;

namespace SISDOMI.DTOs
{
    public class PlanIntervencionConsultaDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public String id { get; set; }
        public String tipo { get; set; }
        public String area { get; set; }
        public String fase { get; set; }
        public String estado { get; set; }
        public Object contenido { get; set; }
        public Residentes residente { get; set; }
        public String creador { get; set; }
        public Int32 documentos { get; set; }

    }
}
