using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Entities
{
    public class ExplotacionSexual
    {
        public bool victimaexplotacion { get; set; }
        public int edadinicio { get; set; }
        public bool victimatrata { get; set; }
        public List<TrataSexual> tratasexual{ get; set; }
    }
    public class TrataSexual
    {
        public string modalidad { get; set; }
        public string lugar { get; set; }
        public List<String> personascontacto{ get; set; }
        public string duracion { get; set; }
        public string sentimientos { get; set; }
        public string referencias { get; set; }

    }
    
}
