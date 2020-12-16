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

        
    }

    public class FichaIngresoDTO2 : Documento
    {
        [BsonElement("contenido")]
        public object contenido { get; set; }
    }

    public class FichaIngresoDetalleDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        [BsonElement("tipo")]
        public string tipo { get; set; }
        [BsonElement("historialcontenido")]
        public List<string> historialContenido { get; set; }
        [BsonElement("creadordocumento")]
        public Usuario creadorDocumento { get; set; }
        [BsonElement("fechacreacion")]
        public DateTime fechaCreacion { get; set; }
        [BsonElement("area")]
        public string area { get; set; }
        [BsonElement("fase")]
        public string fase { get; set; }
        public Object contenido { get; set; }
        public Residentes residente { get; set; }
    }

    public class FamiliarIngreso
    {
        public List<String> motivoingreso { get; set; } = new List<String>();
        public List<Familiar> familiares { get; set; } = new List<Familiar>();
        public string tipofamilia { get; set; }
        public string problematicafam { get; set; }
    }

    public class Vivienda
    {
        public string ubicacion { get; set; }
        public string descripcionubicacion { get; set; }
        public List<String> habitantes { get; set; } = new List<String>();
        public string habitacionesdormir { get; set; }
        public string tipopropiedad { get; set; }
        public string tipo { get; set; }
        public string material { get; set; }
        public string tipopiso { get; set; }
        public string tipotecho { get; set; }
        public List<Servicio> servicios { get; set; } = new List<Servicio>();

    }

    public class Servicio
    {
        public String servicio { get; set; }
        public String tipo { get; set; }
    }

    public class Economico
    {
        public string condicionlaboral { get; set; }
        public string ocupacion { get; set; }
        public List<String> ingresos { get; set; } = new List<String>();
        public List<String> egresos { get; set; } = new List<String>();
        public string observacion { get; set; }
    }

    public class Legal
    {
        public List<Penal> penales { get; set; } = new List<Penal>();
        public String apoyolocal { get; set; }
    }

    public class Penal
    {
        public String familiar { get; set; }
        public string motivo { get; set; }
    }
} 

