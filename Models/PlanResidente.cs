using SISDOMI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Models
{
    public class PlanResidente
    {//
        public PlanIntervencionIndividualEducativo  planintervencionindividual {get; set;}
        public String idresidente { get; set; }
    }

    public class PlanResidentePsicologico
    {//
        public PlanIntervencionIndividualPsicologico planIntervencionIndividualPsicologico { get; set; }
        public String idresidente { get; set; }
    }

    public class PlanResidenteSocial
    {//
        public PlanIntervencionIndividualSocial planIntervencionIndividualSocial { get; set; }
        public String idresidente { get; set; }
    }
}
