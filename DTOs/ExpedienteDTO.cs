using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace SISDOMI.DTOs
{
    public class ExpedienteDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public String id { get; set; }
        [BsonElement("numeroexpediente")]
        public String numeroExpediente { get; set; }
        [BsonElement("idresidente")]
        public String idResidente { get; set; }
        [BsonElement("fechainicio")]
        public DateTime fechaInicio { get; set; }
        [BsonElement("residente")]
        public String residente { get; set; }

    }

   
}
