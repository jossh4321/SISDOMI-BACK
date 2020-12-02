using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Entities
{
    public class Tutor
    {
        public string nombre { get; set; }
        public string tipoDocumento { get; set; }
        public string numeroDocumento { get; set; }
        public string parentesco { get; set; }
        public ResidenteAux usuaria { get; set; }
    }

    public class ResidenteAux
    {
        public string id { get; set; }
        public string residente { get; set; }
        public string numeroDocumento { get; set; }

    }
}
