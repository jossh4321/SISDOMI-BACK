using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SISDOMI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SISDOMI.DTOs
{
    public class ActaExternamientoDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string tipo { get; set; }
         public List<HistorialContenido> historialcontenido { get; set; }
        public string creadordocumento { get; set; }
        public DateTime? fechacreacion { get; set; }
        public string area { get; set; }
           public string fase { get; set; }
        public string responsable { get; set; }
        public string residente { get; set; }
        public string idresidente { get; set; }
        public string estado { get; set; }
        //public Object contenido { get; set; }

    }
}