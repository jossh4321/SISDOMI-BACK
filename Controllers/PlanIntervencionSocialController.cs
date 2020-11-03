using Microsoft.AspNetCore.Mvc;
using SISDOMI.Entities;
using SISDOMI.Models;
using SISDOMI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace SISDOMI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlanIntervencionSocialController: ControllerBase
    {
        private readonly PlanIntervencionIndividualService _planIntervencionService;

        public PlanIntervencionSocialController(PlanIntervencionIndividualService planIntervencionIndividualService)
        {
            _planIntervencionService = planIntervencionIndividualService;
        }

        [HttpGet("all")]
        public ActionResult<List<PlanIntervencionIndividual>> GetAll()
        {
            return _planIntervencionService.GetAll();
        }

        [HttpPost("")]
        public async Task<ActionResult<PlanIntervencionIndividual>> Post(PlanResidente planIntervencionIndividual)
        {
            return await _planIntervencionService.CreateIndividualInterventionPlan(planIntervencionIndividual);
        }


        [HttpPut("")]
        public ActionResult<PlanIntervencionIndividual> Put(PlanIntervencionIndividual planIntervencion)
        {
            PlanIntervencionIndividual objetoplan = _planIntervencionService.ModifyIndividualInterventionPlan(planIntervencion);
            return objetoplan;
        }
    }
}
