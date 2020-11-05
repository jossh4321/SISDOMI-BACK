using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Entities
{
    public class Vivienda
    {
        public string ubicacion { get; set; }
        public string descripcionubicacion { get; set; }
        public List<String> habitantes { get; set; } = new List<String>();
        public string habitacionesdormir { get; set; }
        public string tipopropiedad { get; set; }
        public string tipo { get; set; }
        public string material { get; set; }
        public string tipopiso { get; set; }
        public string tipotecho { get; set; }
        public List<Servicio> servicios { get; set; } = new List<Servicio>();

    }

    public class Servicio
    {
        public String servicio { get; set; }
        public String tipo { get; set; }
    }
}
