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
    public class AnexoController : ControllerBase
    {
        private readonly AnexoService _anexoService;

        public AnexoController(AnexoService anexoService)
        {
            _anexoService = anexoService;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<AnexoDTO>>> GetAll()
        {
            return await _anexoService.GetAll();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AnexoDTO>> GetById(String id)
        {
            try
            {
                return await _anexoService.GetAnexoById(id);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }


    }
}
