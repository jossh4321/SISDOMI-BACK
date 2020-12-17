using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SISDOMI.DTOs;
using SISDOMI.Entities;
using SISDOMI.Services;

namespace SISDOMI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TallerController : ControllerBase
    {
        private readonly TallerService _tallerService;

        public TallerController(TallerService tallerService)
        {
            _tallerService = tallerService;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<Taller>>> GetAll()
        {
            return await _tallerService.GetAll();
        }

        [HttpGet("all/TallerEP")]
        public async Task<ActionResult<List<TallerEscuelaPadres>>> GetAllTEP()
        {
            return await _tallerService.GetAllTEP();
        }

        [HttpGet("all/TallerE")]
        public async Task<ActionResult<List<TallerEducativo>>> GetAllTE()
        {
            return await _tallerService.GetAllTE();
        }

        [HttpGet("all/TallerFE")]
        public async Task<ActionResult<List<TallerFormativoEgreso>>> GetAllTFE()
        {
            return await _tallerService.GetAllTFE();
        }

        [HttpPost("crearTEP")]
        public async Task<ActionResult<TallerEscuelaPadres>> PostTEP([FromBody] TallerEscuelaPadres taller)
        {
            TallerEscuelaPadres mitaller = _tallerService.CreateTEP(taller);
            return mitaller;
        }
        [HttpPost("crearTE")]
        public async Task<ActionResult<TallerEducativo>> PostTE([FromBody] TallerEducativo taller)
        {
            TallerEducativo mitaller = _tallerService.CreateTE(taller);
            return mitaller;
        }
        [HttpPost("crearTFE")]
        public async Task<ActionResult<TallerFormativoEgreso>> PostTFE([FromBody] TallerFormativoEgreso taller)
        {
            TallerFormativoEgreso mitaller = _tallerService.CreateTFE(taller);
            return mitaller;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Taller>> GetById(string id)
        {
            return await _tallerService.GetById(id);
        }
        [HttpPut("actualizarTallerEP")]
        public async Task<ActionResult<Taller>> PutTEP([FromBody] TallerEscuelaPadres taller)
        {
            return await _tallerService.PutTallerEP(taller);
        }
        [HttpPut("actualizarTallerE")]
        public async Task<ActionResult<Taller>> PutTE([FromBody] TallerEducativo taller)
        {
            return await _tallerService.PutTallerE(taller);
        }
        [HttpPut("actualizarTallerFE")]
        public async Task<ActionResult<Taller>> PutTFE([FromBody] TallerFormativoEgreso taller)
        {
            return await _tallerService.PutTallerFE(taller);
        }
    }
}