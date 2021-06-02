using SISDOMI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Models
{
    public class PlanResidente
    {//
        public PlanIntervencionIndividualEducativo planintervencionindividual { get; set; }
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

    public class PlanUpdate
    {
        public String id { get; set; }
        public String idresidente { get; set; }
        public List<HistorialContenido> historialcontenido { get; set; }
        public String tipo { get; set; }
    }

    public class PlanPsicologoUpdate : PlanUpdate
    {
        public ContenidoPlanIntervencionPsicologica contenido { get; set; }
    }

    public class PlanEducativoUpdate : PlanUpdate
    {
        public ContenidoPlanIntervencionIndividualEducativo contenido { get; set; }
    }

    public class PlanSocialUpdate: PlanUpdate
    {
        public ContenidoPlanIntervencionSocial contenido { get; set; }
    }


}
