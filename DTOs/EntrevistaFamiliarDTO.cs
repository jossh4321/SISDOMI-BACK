using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SISDOMI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.DTOs
{
    public class EntrevistaFamiliarDTO : Documento
    {
        //Entrevistas Familiares con datos del residente
        public ContenidoEntrevistaFamiliarDTO contenido { get; set; } = new ContenidoEntrevistaFamiliarDTO();
    }
    public class ContenidoEntrevistaFamiliarDTO
    {
        [BsonElement("fechaentrevista")]
        public DateTime? fechaEntrevista { get; set; }
        [BsonElement("nombreapoderado")]
        public string nombreApoderado { get; set; }
        [BsonElement("apellidoapoderado")]
        public string apellidoApoderado { get; set; }
        [BsonElement("dniapoderado")]
        public string dniApoderado { get; set; }
        [BsonElement("observaciones")]
        public string observaciones { get; set; }
        public DatosResidenteDTO2 datosresidente { get; set; }
    }

    public class DatosResidenteDTO2
    {
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string tipodocumento { get; set; }
        public string numerodocumento { get; set; }
    }
}
