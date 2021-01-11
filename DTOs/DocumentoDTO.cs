using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SISDOMI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.DTOs
{
    public class DocumentoDTO { 
    
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string tipo { get; set; }
        public List<string> historialcontenido { get; set; }
        public string creadordocumento { get; set; }
        public DateTime? fechacreacion { get; set; }
        public string area { get; set; }
        public string fase { get; set; }
        public string idresidente { get; set; }
        public string estado { get; set; }
        public Object contenido { get; set; }

      
    }

    // DTO para obtener todos los documentos mediante el tipo y el id del residente
    public class DocumentTypeResidentDTO
    {
        public String tipo { get; set; }
        public List<String> documentos { get; set; }
    }

    public class DocumentoExpedienteDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public String id { get; set; }
        public String tipo { get; set; }
        public String area { get; set; }
        public String fase { get; set; }
        public String estado { get; set; }
        public DateTime? fechacreacion { get; set; }
        public Object contenido { get; set; }
        public Residentes residente { get; set; }
        public UsuarioFirma creador { get; set; }

        public class UsuarioFirma
        {
            public String nombres { get; set; }
            public String firma { get; set; }
        }
    }

    public class BuscarExpedienteDocumentoDTO
    {
        public string idresidente { get; set; }
        public string tipo { get; set; }
    }
    
}
