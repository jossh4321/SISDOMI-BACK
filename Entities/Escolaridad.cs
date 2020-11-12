using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Entities
{
    public class Escolaridad
    {
        public string niveleducativo { get; set; }
        public string educacionespecial { get; set; }
        public List<EdadGrado> edadgradoescolar { get; set; }
       
    }
    public class EdadGrado
    {
        public int edad { get; set; }
        public string gradoescolar { get; set; }
       

    }

}
