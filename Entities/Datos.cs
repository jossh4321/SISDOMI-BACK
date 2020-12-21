using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Entities
{
    public class Datos
    {
        public string nombre { get; set; }
        public string apellido { get; set; }
        public DateTime fechanacimiento { get; set; }
        public string tipodocumento { get; set; }
        public string numerodocumento { get; set; }
        public string direccion { get; set; }
        public string email { get; set; }
        public string imagen { get; set; }
        public string firma{ get; set; }
    }
}
