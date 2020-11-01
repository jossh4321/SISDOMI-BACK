using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Entities
{
    public class Progreso
    {
        public int fase { get; set; }
        public DateTime fechaingreso { get; set; }
        public DateTime fechafinalizacion { get; set; }
        public string estado { get; set; }        
    }
}
