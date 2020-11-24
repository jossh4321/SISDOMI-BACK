using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SISDOMI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.DTOs
{
    public class FichaIngresoDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        [BsonElement("tipo")]
        public string tipo { get; set; }
        [BsonElement("historialcontenido")]
        public List<string> historialcontenido { get; set; }
        [BsonElement("creadordocumento")]
        public string creadordocumento { get; set; }
        [BsonElement("fechacreacion")]
        public DateTime fechacreacion { get; set; }
        [BsonElement("area")]
        public string area { get; set; }
        [BsonElement("fase")]
        public string fase { get; set; }
        [BsonElement("idresidente")]
        public Residentes idresidente { get; set; }
        [BsonElement("estado")]
        public string estado { get; set; }
        [BsonElement("contenido")] 
        public object contenido { get; set; }

        [BsonElement("codigodocumento")]
        public string codigodocumento { get; set; }
        [BsonElement("residenteresultado")]
        public string residenteresultado { get; set; }

        [BsonElement("numerodocumento")]
        public string numerodocumento { get; set; }
    }
    

} 

