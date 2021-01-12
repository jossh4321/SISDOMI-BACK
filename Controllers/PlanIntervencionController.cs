using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SISDOMI.DTOs;
using SISDOMI.Entities;
using SISDOMI.Helpers;
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
    [Authorize]
    public class PlanIntervencionController: ControllerBase
    {
        private readonly PlanIntervencionIndividualService _planIntervencionService;
        private readonly IFileStorage _fileStorage;

        public PlanIntervencionController(PlanIntervencionIndividualService planIntervencionIndividualService, IFileStorage fileStorage)
        {
            _planIntervencionService = planIntervencionIndividualService;
            _fileStorage = fileStorage;
        }


        // Educativo

        [HttpPost("educativo")]
        public async Task<ActionResult<PlanIntervencionIndividualEducativo>> PostPlanEducativo([FromBody] PlanResidente planIntervencionIndividual)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(planIntervencionIndividual.planintervencionindividual.historialcontenido[0].url))
                {
                    var solicitudBytes2 = Convert.FromBase64String(planIntervencionIndividual.planintervencionindividual.historialcontenido[0].url);
                    planIntervencionIndividual.planintervencionindividual.historialcontenido[0].url = await _fileStorage.SaveDoc(solicitudBytes2, "pdf", "archivos");
                }
                planIntervencionIndividual.planintervencionindividual.historialcontenido[0].version = 1;
                planIntervencionIndividual.planintervencionindividual.historialcontenido[0].fechamodificacion = DateTime.UtcNow.AddHours(-5);
                return await _planIntervencionService.CreateIndividualInterventionPlan(planIntervencionIndividual);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
            
        }

        [HttpPut("educativo")]
        public async Task<ActionResult<PlanIntervencionIndividualEducativo>> Put([FromBody] PlanEducativoUpdate planIntervencion)
        {
            try
            {
                PlanIntervencionIndividualEducativo objetoplan = await _planIntervencionService.ModifyIndividualInterventionPlan(planIntervencion);
                return objetoplan;
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        // Plan Psicologico

        [HttpPost("psicologico")]
        public async Task<ActionResult<PlanIntervencionIndividualPsicologico>> PostPlanPsicologico([FromBody] PlanResidentePsicologico planResidentePsicologico)
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

        [HttpPut("psicologico")]
        public async Task<ActionResult<PlanIntervencionIndividualPsicologico>> Put([FromBody] PlanPsicologoUpdate planIntervencion)
        {
            PlanIntervencionIndividualPsicologico objetoplan =await _planIntervencionService.ModifyPsycologicalIndividualInterventionPlan(planIntervencion);
            return objetoplan;
        }

        // Plan Social

        [HttpPost("social")]
        public async Task<ActionResult<PlanIntervencionIndividualSocial>> PostPlanSocial([FromBody] PlanResidenteSocial planResidenteSocial)
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
        public async Task<ActionResult<PlanIntervencionIndividualSocial>> PutPlanSocial([FromBody] PlanSocialUpdate planIntervencion)
        {
            try
            {
                PlanIntervencionIndividualSocial objetoplan =await _planIntervencionService.ModifySocialIndividualInterventionPlan(planIntervencion);
                return objetoplan;
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
            
        }

        //General
        [HttpGet("all")]
        public async Task<ActionResult<List<PlanIntervencionDTO>>> GetAll()
        {
            try
            {
                return await _planIntervencionService.GetAll();
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
            
        }

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
        [HttpPut("state")]
        public async Task<ActionResult> UpdatePlanState([FromBody] PlanState planState)
        {
            try
            {
                await _planIntervencionService.UpdatePlanState(planState);

                return Ok();
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}
