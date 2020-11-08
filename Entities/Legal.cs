using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Entities
{
    public class Legal
    {
        public List<Penal> penales { get; set; } = new List<Penal>();
        public String apoyolocal { get; set; }
    }

    public class Penal
    {
        public String familiar { get; set; }
        public string motivo { get; set; }
    }
}
