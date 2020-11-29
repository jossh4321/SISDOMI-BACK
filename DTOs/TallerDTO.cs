using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SISDOMI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.DTOs
{
    public class TallerDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public String id { get; set; }
        [BsonElement("fechacreacion")]
        public DateTime fechaCreacion { get; set; }
        [BsonElement("titulo")]
        public String titulo { get; set; }
        [BsonElement("tipo")]
        public String tipo { get; set; }
        [BsonElement("descripcion")]
        public String descripcion { get; set; }
        [BsonElement("creadordocumento")]
        public String creadorDocumento { get; set; }
        [BsonElement("area")]
        public String area { get; set; }
        [BsonElement("fase")]
        public String fase { get; set; }
        [BsonElement("firma")]
        public Firma firma { get; set; }
    }

    public class TallerEscuelaPadres : TallerDTO
    {
        public ContenidoTallerEscuelaPadres contenido { get; set; } = new ContenidoTallerEscuelaPadres();
    }

    public class ContenidoTallerEscuelaPadres
    {
        [BsonElement("fechainicio")]
        public DateTime fechaInicio { get; set; }
        [BsonElement("fechafin")]
        public DateTime fechaFin { get; set; }
        [BsonElement("tutores")]
        public List<Tutor> tutores { get; set; }
    }

    public class TallerEducativo : TallerDTO
    {
        public ContenidoTallerEducativo contenido { get; set; } = new ContenidoTallerEducativo();
    }

    public class ContenidoTallerEducativo
    {
        [BsonElement("fecharealizacion")]
        public DateTime fechaRealizacion { get; set; }
        [BsonElement("turno")]
        public string turno { get; set; }
        [BsonElement("nrosesion")]
        public int nroSesion { get; set; }
        [BsonElement("participantes")]
        public List<ParticipanteTaller> participantes { get; set; }
    }

    public class TallerFormativoEgreso : TallerDTO
    {
        public ContenidoTallerFormativoEgreso contenido { get; set; } = new ContenidoTallerFormativoEgreso();
    }

    public class ContenidoTallerFormativoEgreso
    {
        [BsonElement("fecharealizacion")]
        public DateTime fechaRealizacion { get; set; }
        [BsonElement("turno")]
        public string turno { get; set; }
        [BsonElement("nrosesion")]
        public int nroSesion { get; set; }
        [BsonElement("participantes")]
        public List<ParticipanteTaller> participantes { get; set; }
    }

    public class ParticipanteTaller
    {
        public string cooperacion { get; set; }
        public string puntualidad { get; set; }
        public string aprendizaje { get; set; }
        public string conducta { get; set; }
        public string firma { get; set; }
        public string usuariaid { get; set; }
    }
}
