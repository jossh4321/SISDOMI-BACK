using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SISDOMI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.DTOs
{
    public class SesionEducativaDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string titulo { get; set; }
        public string idcreador { get; set; }
        public DateTime? fechacreacion { get; set; }
        public string area { get; set; }
        public ContenidoSesionEducativaDTO contenido { get; set; }
        public string tipo { get; set; }
    }
    public class ContenidoSesionEducativaDTO
    {
        public List<ParticipanteDTO> participantes { get; set; }
    }

    public class ParticipanteDTO
    {
        public string idparticipante { get; set; }
        public string grado { get; set; }
        public DateTime fecha { get; set; }
        public string firma { get; set; }
        public string observaciones { get; set; }
        public DatosResidenteDTO datosresidente { get; set; }
    }
    public class DatosResidenteDTO
    {
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string tipodocumento { get; set; }
        public string numerodocumento { get; set; }
        public ProgresoActualDTO progresoactual { get; set; }
    }
    public class ProgresoActualDTO
    {
        public int fase { get; set; }
        public string nombre { get; set; }

    }
    public class SesionEducativaDTOInicial
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }

        [BsonElement("titulo")]
        public string titulo { get; set; }
        [BsonElement("idcreador")]
        public string idCreador { get; set; }

        [BsonElement("fechacreacion")]
        public DateTime? fechaCreacion { get; set; }

        [BsonElement("area")]
        public string area { get; set; }

        [BsonElement("contenido")]
        public ContenidoSesionEducativa contenido { get; set; }

        [BsonElement("tipo")]
        public string tipo { get; set; }
        [BsonElement("datoscreador")]
        public DatosCreador datoscreador { get; set; }
    }
    public class DatosCreador
    {
        public string usuario { get; set; }
        public DatosCreadorContenido datos { get; set; }
        public string estado { get; set; }
        public string rol { get; set; }
    }
    public class DatosCreadorContenido
    {
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string email { get; set; }
        public string imagen { get; set; }        
    }
}
