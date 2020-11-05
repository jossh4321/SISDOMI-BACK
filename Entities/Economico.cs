using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Entities
{
    public class Economico
    {
        public string condicionlaboral { get; set; }
        public string ocupacion { get; set; }
        List<String> ingresos { get; set; }
        List<String> egresos { get; set; }
        public string observacion { get; set; }

    }
}
