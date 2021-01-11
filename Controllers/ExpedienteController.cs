using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using SISDOMI.Services;
using SISDOMI.DTOs;
using SISDOMI.Entities;

namespace SISDOMI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpedienteController: ControllerBase
    {
        private readonly ExpedienteService expedienteService;
        public ExpedienteController(ExpedienteService expedienteService)
        {
            this.expedienteService = expedienteService;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<ExpedienteDTO>>> GetAll()
        {
            try
            {
                List<ExpedienteDTO> lstExpedienteDTOs = await expedienteService.GetAll();

                return lstExpedienteDTOs;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
                
            }
        }
        [HttpGet("idresidente")]
        public async Task<Expediente> Get([FromQuery] string idresidente)
        {
            return await expedienteService.GetByResident(idresidente);
        }
    }
}
