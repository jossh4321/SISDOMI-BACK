using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Entities
{
    public class Notificacion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        [BsonElement("titulo")]
        public string titulo { get; set; }
        [BsonElement("cuerpo")]
        public string cuerpo { get; set; }
        [BsonElement("estado")]
        public string estado { get; set; }
        [BsonElement("idemisor")]
        public string idemisor { get; set; }
        [BsonElement("idreceptor")]
        public string idreceptor { get; set; }
        [BsonElement("fechaemision")]
        public DateTime fechaemision { get; set; }
    }
}
