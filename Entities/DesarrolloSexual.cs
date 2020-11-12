using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Entities
{
    public class DesarrolloSexual
    {
        public bool menstruacion { get; set; }
        public DateTime menarquia { get; set; }
        public Relaciones relaciones { get; set; }
    }
    public class Relaciones
    {
        public bool iniciorelaciones { get; set; }
        public int edadinicio { get; set; }
        public string motivo { get; set; }
        public string generopareja { get; set; }
        public bool relacionconsentida { get; set; }
        public bool its { get; set; }
        public bool tratamientoits { get; set; }
    }
}
