using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Entities
{
    public class Incidencia
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        [BsonElement("usuario")]
        public string usuario { get; set; }
        [BsonElement("fecharegistro")]
        public DateTime fechaRegistro { get; set; }
        [BsonElement("fecha")]
        public DateTime fecha { get; set; }
        [BsonElement("titulo")]
        public string titulo { get; set; }
        [BsonElement("descripcion")]
        public string descripcion { get; set; }
        [BsonElement("observaciones")]
        public List<string> observaciones { get; set; } = new List<string>();
        [BsonElement("incidencias")]
        public List<string> incidencias { get; set; } = new List<string>();
        [BsonElement("residentes")]
        public List<string> residentes { get; set; } = new List<string>();
        [BsonElement("firma")]
        public Firma firma { get; set; }
    }
}
