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

}
