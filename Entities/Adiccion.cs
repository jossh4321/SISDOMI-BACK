using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Entities
{
    public class Adiccion
    {
        public bool cosumo { get; set; }
        public DateTime  ultidiaconsumo { get; set; }
        public List<Spa> spa { get; set; }
    }
    public class Spa
    {
        public string tipospa { get; set; }
        public string tiempoconsumo { get; set; }
        public string frecuencia { get; set; }
    }
}
