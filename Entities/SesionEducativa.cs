using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SISDOMI.Entities
{
    public class SesionEducativa
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        [BsonElement("titulo")]
        public string titulo { get; set; }
        [BsonElement("idcreador")]
        public string idCreador { get; set; }
        [BsonElement("fechacreacion")]
        public DateTime fechaCreacion { get; set; }
        [BsonElement("area")]
        public string area { get; set; }
        [BsonElement("contenido")]
        public ContenidoSesionEducativa contenido { get; set; }
        [BsonElement("tipo")]
        public string tipo { get; set; }
    }

    public class ContenidoSesionEducativa
    {
        public List<Participante> participantes { get; set; }
    }
    public class Participante
    {
        public string idparticipante { get; set; }
        public string grado { get; set; }
        public DateTime fecha { get; set; }
        public string firma { get; set; }
        public string observaciones { get; set; }
    }
}
