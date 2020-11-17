using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Entities
{
    public class Expediente
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public String id { get; set; }

        [BsonElement("numeroexpediente")]
        public String numeroexpediente { get; set; }
        
        [BsonElement("idresidente")]
        public String idresidente { get; set; }

        [BsonElement("fechainicio")]
        public DateTime? fechainicio { get; set; }

        [BsonElement("fechafin")]
        public DateTime? fechafin { get; set; }

        [BsonElement("documentos")]
        public List<DocumentoExpediente> documentos { get; set; } = new List<DocumentoExpediente>();

    }

    public class DocumentoExpediente
    {
        public String tipo { get; set; }
        public String iddocumento { get; set; }
    }
}
