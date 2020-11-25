﻿using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class PlanIntervencionController: ControllerBase
    {
        private readonly PlanIntervencionIndividualService _planIntervencionService;

        public PlanIntervencionController(PlanIntervencionIndividualService planIntervencionIndividualService)
        {
            _planIntervencionService = planIntervencionIndividualService;
        }


        // Educativo

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

        [HttpPut("psicologico")]
        public ActionResult<PlanIntervencionIndividualPsicologico> Put(PlanIntervencionIndividualPsicologico planIntervencion)
        {
            PlanIntervencionIndividualPsicologico objetoplan = _planIntervencionService.ModifyPsycologicalIndividualInterventionPlan(planIntervencion);
            return objetoplan;
        }

        // Plan Social

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
        public async Task<ActionResult> UpdatePlanState(PlanState planState)
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
