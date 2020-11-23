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
        public DateTime fechacreacion { get; set; }
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
}
