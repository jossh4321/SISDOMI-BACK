using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Entities
{
    public class Discapacidad
    {
        public string intelectual{ get; set; }
        public string fisico{ get; set; }
        public string sensorial { get; set; }
        public List<String> enfermedad { get; set; }
    }
    
}
