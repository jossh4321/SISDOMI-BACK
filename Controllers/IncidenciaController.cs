using Microsoft.AspNetCore.Mvc;
using SISDOMI.DTOs;
using SISDOMI.Entities;
using SISDOMI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IncidenciaController
    {
        private readonly IncidenciaService _incidenciaService;

        public IncidenciaController(IncidenciaService incidenciaService)
        {
            _incidenciaService = incidenciaService;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<Incidencia>>> GetAllIncidencia()
        {
            return await _incidenciaService.GetAll();
        }

        [HttpGet("")]
        public async Task<ActionResult<Incidencia>> GetIncidencia([FromQuery] string id)
        {
            return await _incidenciaService.GetIncidencia(id);
        }

        [HttpPost("")]
        public async Task<ActionResult<IncidenciaDTO>> PostIncidencia(Incidencia incidencia)
        {
            return await _incidenciaService.PostIncidencia(incidencia);
        }

        [HttpPut("")]
        public async Task<ActionResult<IncidenciaDTO>> PutIncidencia(Incidencia incidencia)
        {
            return await _incidenciaService.PutIncidencia(incidencia);
        }
        [HttpGet("detalle/{id}")]
        public async Task<ActionResult<IncidenciaDTO>> GetDetalleIncidenteDTO(string id)
        {
            return await _incidenciaService.GetDetalleIncidencia(id);
        }
        [HttpGet("all/detalle")]
        public async Task<ActionResult<List<IncidenciaDTO>>> GetListDetalleIncidenciaDTO([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            return await _incidenciaService.GetListDetalleIncidencia(FromDate?.Date.ToString(), ToDate?.Date.ToString());
        }
    }
}
