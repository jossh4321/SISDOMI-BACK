using Microsoft.AspNetCore.Mvc;
using SISDOMI.DTOs;
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
        public async Task<ActionResult<List<PlanIntervencionDTO>>> GetAll()
        {
            return  await _planIntervencionService.GetAll();
        }

        [HttpPost("")]
        public async Task<ActionResult<PlanIntervencionIndividualEducativo>> Post(PlanResidente planIntervencionIndividual)
        {
            return await _planIntervencionService.CreateIndividualInterventionPlan(planIntervencionIndividual);
        }

        [HttpGet("id")]
        public ActionResult<PlanIntervencionIndividual> Get([FromQuery] string id)
        {
            return _planIntervencionService.GetById(id);
               
        }


        [HttpPut("")]
        public ActionResult<PlanIntervencionIndividualEducativo> Put(PlanIntervencionIndividualEducativo planIntervencion)
        {
            PlanIntervencionIndividualEducativo objetoplan = _planIntervencionService.ModifyIndividualInterventionPlan(planIntervencion);
            return objetoplan;
        }
    }
}
