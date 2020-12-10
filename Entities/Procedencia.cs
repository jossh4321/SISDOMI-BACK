using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Entities
{
    public class Procedencia
    {
        public string  nombre { get; set; }
        public string  tipo { get; set; }
        public string  modalidad { get; set; }
        public string  nivel { get; set; }
        public string  grado { get; set; } 
        public string  observacion { get; set; }
        public string  direccion { get; set; }
        public string  telefono { get; set; }
        public string  correo { get; set; }
        [BsonElement("documentosescolares")]
        public List<DocumentosEscolares> documentosEscolares { get; set; } = new List<DocumentosEscolares>();
        [BsonElement("situacionescolar")]
        public string situacionEscolar { get; set; }
    }
    public class DocumentosEscolares
    {
        public string titulo { get; set; }
        public string url { get; set; }
    }
}
