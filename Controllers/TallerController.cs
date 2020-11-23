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

        [HttpGet("all/Taller")]
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
        public async Task<ActionResult<TallerEscuelaPadres>> CreateTEP([FromBody] TallerEscuelaPadres taller)
        {
            TallerEscuelaPadres mitaller = _tallerService.CreateTEP(taller);
            return mitaller;
        }
        [HttpPost("crearTE")]
        public async Task<ActionResult<TallerEducativo>> CreateTE([FromBody] TallerEducativo taller)
        {
            TallerEducativo mitaller = _tallerService.CreateTE(taller);
            return mitaller;
        }
        [HttpPost("crearTFE")]
        public async Task<ActionResult<TallerFormativoEgreso>> CreateTFE([FromBody] TallerFormativoEgreso taller)
        {
            TallerFormativoEgreso mitaller = _tallerService.CreateTFE(taller);
            return mitaller;
        }
        [HttpGet("id")]
        public async Task<ActionResult<Taller>> GetById([FromQuery] string id)
        {
            return await _tallerService.GetById(id);
        }
    }
}