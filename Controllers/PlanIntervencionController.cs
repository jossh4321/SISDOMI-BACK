using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SISDOMI.DTOs;
using SISDOMI.Entities;
using SISDOMI.Models;
using SISDOMI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlanIntervencionController: ControllerBase
    {
        private readonly PlanIntervencionIndividualService _planIntervencionService;

        public PlanIntervencionController(PlanIntervencionIndividualService planIntervencionIndividualService)
        {
            _planIntervencionService = planIntervencionIndividualService;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<PlanIntervencionDTO>>> GetAll()
        {
            return await _planIntervencionService.GetAll();
        }

        [HttpGet("educativobyid")]
        public ActionResult<PlanIntervencionIndividualEducativo> GetPlanEducativo([FromQuery] string id)
        {
            return _planIntervencionService.GetEducationalIndividualInterventionPlanById(id);

        }

        [HttpPost("educativo")]
        public async Task<ActionResult<PlanIntervencionIndividualEducativo>> PostPlanEducativo(PlanResidente planIntervencionIndividual)
        {
            try
            {
                return await _planIntervencionService.CreateIndividualInterventionPlan(planIntervencionIndividual);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
            
        }

        [HttpPut("educativo")]
        public ActionResult<PlanIntervencionIndividualEducativo> Put(PlanIntervencionIndividualEducativo planIntervencion)
        {
            PlanIntervencionIndividualEducativo objetoplan = _planIntervencionService.ModifyIndividualInterventionPlan(planIntervencion);
            return objetoplan;
        }

        // Plan Psicologico
        [HttpPost("psicologico")]
        public async Task<ActionResult<PlanIntervencionIndividualPsicologico>> PostPlanPsicologico(PlanResidentePsicologico planResidentePsicologico)
        {
            try
            {
                return await _planIntervencionService.CreatePsycologicalInterventionPlan(planResidentePsicologico);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }


        // Plan Social
        [HttpGet("socialbyid")]
        public ActionResult<PlanIntervencionIndividualSocial> GetPlanSocial([FromQuery] string id)
        {
            return _planIntervencionService.GetSocialIndividualInterventionPlanById(id);

        }

        [HttpPost("social")]
        public async Task<ActionResult<PlanIntervencionIndividualSocial>> PostPlanSocial(PlanResidenteSocial planResidenteSocial)
        {
            try
            {
                return await _planIntervencionService.CreateSocialInterventionPlan(planResidenteSocial);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPut("social")]
        public ActionResult<PlanIntervencionIndividualSocial> PutPlanSocial(PlanIntervencionIndividualSocial planIntervencion)
        {
            PlanIntervencionIndividualSocial objetoplan = _planIntervencionService.ModifySocialIndividualInterventionPlan(planIntervencion);
            return objetoplan;
        }

        //General
        [HttpGet("{id}")]
        public async Task<ActionResult<PlanIntervencionConsultaDTO>> GetPlanById(String id)
        {
            try
            {
                return await _planIntervencionService.GetPlanById(id);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}
