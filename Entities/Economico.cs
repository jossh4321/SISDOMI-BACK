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
        public List<String> ingresos { get; set; } = new List<String>();
        public List<String> egresos { get; set; } = new List<String>();
        public string observacion { get; set; }

    }
}
