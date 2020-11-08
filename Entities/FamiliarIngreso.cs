using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Entities
{
    public class FamiliarIngreso
    {
        public List<String> motivoingreso { get; set; } = new List<String>();
        public List<FamiliarDatos> familiares { get; set; } = new List<FamiliarDatos>();
        public string tipofamilia { get; set; }
        public string problematicafam { get; set; }

    }

    public class FamiliarDatos
    {
        public String nombrecompleto { get; set; }
        public String parentezco { get; set; }
        public Int32 edad { get; set; }
        public String estadocivil { get; set; }
        public String gradoinstruccion { get; set; }
        public String ocupacion { get; set; }

        public String observaciones { get; set; }
    }

}
