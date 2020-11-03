using SISDOMI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Models
{
    public class PlanResidente
    {
        public PlanIntervencionIndividual  planintervencionindividual {get; set;}
        public String idresidente { get; set; }
    }
}
