using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using SISDOMI.Services;
using SISDOMI.DTOs;

namespace SISDOMI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExpedienteController: ControllerBase
    {
        private readonly ExpedienteService expedienteService;
        public ExpedienteController(ExpedienteService expedienteService)
        {
            this.expedienteService = expedienteService;
        }

        [HttpGet("")]
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
    }
}
