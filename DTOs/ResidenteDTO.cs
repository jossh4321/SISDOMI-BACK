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
        [BsonElement("sexo")]
        public String sexo { get; set; }
        [BsonElement("fechanacimiento")]
        public DateTime fechaNacimiento { get; set; }
        [BsonElement("telefonosreferencias")]
        public List<Telefono> telefonosReferencia { get; set; }
        public List<Anexo> anexos { get; set; }
        [BsonElement("cantidaddocumentos")]
        public List<DocumentoAreaDTO> cantidadDocumentos { get; set; }

        public class DocumentoAreaDTO
        {
            public String area { get; set; }
            public Int32 cantidad { get; set; }
        } 
    }

    public class ResidenteAnnexDocumentoDTO: Residentes
    {
        public List<Anexo> anexos { get; set; } = new List<Anexo>();
        public List<DocumentoDTO> documentos { get; set; } = new List<DocumentoDTO>();
    }


    public class ResidenteFaseDTO
    {
        public Residentes residente { get; set; }
        public ProgresoFase? progresoFase { get; set; }
        public bool promocion { get; set; }
        public int faseAnterior { get; set; }
    }

    public class ResidenteFaseDocumentoDTO
    {
        public string fase { get; set; }
        public string fasedocumentoanterior { get; set; }
        public string area { get; set; }
        public string documentoanterior { get; set; }
        public string documentoactual { get; set; }
        public string estadodocumentoactual { get; set; }
        public string estadodocumentoanterior { get; set; }
        //public List<string> documentos { get; set; }
    }

    public class ResidenteDTO2 : Residentes
    {
        //DTO para registrar datos del usuario creador en el documento de la coleccion Fases
        public string idcreador { get; set; }
        public string observaciones { get; set; }
        public Firma firma { get; set; }

    }

    public class ResidenteFasesDocumentosDTO
    {
        public List<Int32> fases { get; set; }
        public String area { get; set; }
        public List<DocumentoEstado> documentoEstadosAnteriores { get; set; }
        public List<DocumentoEstado> documentoEstadosActuales { get; set; }
        
        public class DocumentoEstado
        {
            public String tipo { get; set; }
            public String estado { get; set; }
        } 
    }

    public class ResidenteProgresoDTO : Residentes
    {
        public List<Fase> fases { get; set; } = new List<Fase>();
        public Expediente expediente { get; set; }
    }
}
