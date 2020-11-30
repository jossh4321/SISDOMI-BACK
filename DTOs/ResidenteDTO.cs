using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using SISDOMI.Entities;

namespace SISDOMI.DTOs
{
    public class ResidenteDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public String id { get; set; }
        public String nombre { get; set; }
        public String apellido { get; set; }
        [BsonElement("tipodocumento")]
        public String tipoDocumento { get; set; }
        [BsonElement("numerodocumento")]
        public String numeroDocumento { get; set; }
        [BsonElement("lugarnacimiento")]
        public String lugarNacimiento { get; set; }
        public String ubigeo { get; set; }
        [BsonElement("juzgadoprocedencia")]
        public String juzgadoProcedencia { get; set; }
        [BsonElement("fechaingreso")]
        public DateTime fechaIngreso { get; set; }
        [BsonElement("motivoingreso")]
        public String motivoIngreso { get; set; }
        public List<Anexo> anexos { get; set; }
        [BsonElement("cantidaddocumentos")]
        public List<DocumentoAreaDTO> cantidadDocumentos { get; set; }

        public class DocumentoAreaDTO
        {
            public String area { get; set; }
            public Int32 cantidad { get; set; }
        } 
    }
}
