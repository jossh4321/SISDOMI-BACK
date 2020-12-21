using MongoDB.Bson.Serialization.Attributes;
using SISDOMI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.DTOs
{
    public class EstadisticaDTO
    {
        public String tipo { get; set; }
        public Int32 cantidad { get; set; }
    }

    public class EstadisticaModalidadDTO: EstadisticaDTO
    {
        public List<Modalidad> modalidades { get; set; }
    }

    public class Modalidad
    {
        public String modalidad { get; set; }
        public Int32 cantidad { get; set; }
    }

    public class EstadisticaResidenteProgresoDTO
    {
        public List<Progreso> progreso { get; set; }
        public List<FaseDocumento> fases { get; set; }

        public class FaseDocumento
        {
            public Int32 fase { get; set; }
            public List<DocumentoFecha> documentos { get; set; }

            public class DocumentoFecha
            {
                public String tipo { get; set; }
                [BsonElement("fechacreacion")]
                public DateTime fechaCreacion { get; set; }
            }
        }
    } 

}
