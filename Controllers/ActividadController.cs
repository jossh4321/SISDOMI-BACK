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
    public class ActividadController : ControllerBase
    {
        private readonly ActividadService _actividadService;

        public ActividadController(ActividadService actividadService)
        {
            _actividadService = actividadService;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<ActividadDTOConsulta>>> GetAll()
        {
            return await _actividadService.GetAll();
        }

        [HttpPost()]
        public async Task<ActionResult<Actividades>> PostAnexo(Actividades actividad)
        {
            try
            {
                return await _actividadService.CreateActividad(actividad);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

        }
    }
}
