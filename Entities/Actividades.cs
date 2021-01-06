using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Entities
{
    public class Actividades
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        [BsonElement("tipo")]
        public string tipo { get; set; }
        [BsonElement("nombre")]
        public string nombre { get; set; }
        [BsonElement("descripcion")]
        public string descripcion { get; set; }
        [BsonElement("fechacreacion")]
        public DateTime fechacreacion { get; set; }
        [BsonElement("idcreador")]
        public string idcreador { get; set; }
        public ContenidoActividad contenido { get; set; } = new ContenidoActividad();
    }
    public class ContenidoActividad
    {
        public DateTime fechainicio { get; set; }
        public DateTime fechafin { get; set; }
        public List<ParticipanteActividad> participantes { get; set; }

    }

    public class ParticipanteActividad
    {
        public string idparticipante { get; set; }
        public string observacion { get; set; }
        public string cooperacion { get; set; }
        public string aprendizaje { get; set; }
        public string puntualidad { get; set; }
        public string conducta { get; set; }

    }
}
