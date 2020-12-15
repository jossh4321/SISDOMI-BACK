using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Entities
{
    public class PadresFichaIngreso
    {
        public string parentesco { get; set; }
        public string nombre { get; set; }
        public string apellidos { get; set; }
        public int edad { get; set; }
        public string instruccion { get; set; }
        public string estadosalud { get; set; }
        public string relacion { get; set; }
    }
}
