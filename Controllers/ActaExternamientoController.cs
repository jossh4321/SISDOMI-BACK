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

    public class ActaExternamientoController : ControllerBase
    {
        private readonly ActaDeExternamientoService _ActaExternamientoService;


        public ActaExternamientoController(ActaDeExternamientoService actaDeExternamientoService)
        {
            _ActaExternamientoService = actaDeExternamientoService;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<ActaExternamientoDTO>>> GetAll()
        {
            return await _ActaExternamientoService.GetAll();
        }

        [HttpGet("id")]
        public async Task<ActionResult<Documento>> GetID(String id)
        {
            try
            {
                return await _ActaExternamientoService.GetById(id);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
                
        }
    }
}