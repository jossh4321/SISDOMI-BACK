using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SISDOMI.Services;
using SISDOMI.DTOs;

namespace SISDOMI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HojaProductividadController : ControllerBase
    {
        private readonly HojaProductividadService _hojaService;
        public HojaProductividadController(HojaProductividadService hojaService)
        {
            this._hojaService = hojaService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<EstadisticaDTO>>> GetById(String id)
        {
            try
            {
                return await _hojaService.GetHojaProductivdadByIdResidente(id);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}
