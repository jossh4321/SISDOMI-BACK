using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SISDOMI.DTOs;
using SISDOMI.Services;

namespace SISDOMI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TallerController : ControllerBase
    {
        private readonly TallerEscuelaPadresService _tallerEPSService;

        public TallerController(TallerEscuelaPadresService tallerEPSService)
        {
            _tallerEPSService = tallerEPSService;
        }

        [HttpGet("all/TallerEscuelaPadres")]
        public async Task<ActionResult<List<TallerEscuelaPadres>>> GetAll()
        {
            return await _tallerEPSService.GetAll();
        }
    }
}