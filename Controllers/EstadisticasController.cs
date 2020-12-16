using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SISDOMI.DTOs;
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
    public class EstadisticasController: ControllerBase
    {
        private readonly EstadisticasService _estadisticasService;

        public EstadisticasController(EstadisticasService estadisticasService)
        {
            _estadisticasService = estadisticasService;
        }

        [HttpGet("residentes/fase")]
        public async Task<ActionResult<List<EstadisticaDTO>>> GetStadisticsResidentByFase()
        {
            try
            {
                List<EstadisticaDTO> lstEstadisticaDTOs =await _estadisticasService.GetStadisticsResidentByFase();

                return lstEstadisticaDTOs;
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet("residentes/edad")]
        public async Task<ActionResult<List<EstadisticaDTO>>> GetStadisticsResidentsByRangeAge()
        {
            try
            {
                List<EstadisticaDTO> lstEstadisticaDTOs = await _estadisticasService.GetStadisticsResidentsByRangeAge();

                return lstEstadisticaDTOs;
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet("residentes/modalidad/{tipo}")]
        public async Task<ActionResult<List<EstadisticaModalidadDTO>>> GetStadisticsResidentModalityByGrade(String tipo)
        {
            try
            {
                List<EstadisticaModalidadDTO> lstEstadisticaModalidadGradoDTOs = await _estadisticasService.GetStadisticsModalidadByGrade(tipo);

                return lstEstadisticaModalidadGradoDTOs;
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

    }
}
