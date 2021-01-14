using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SISDOMI.DTOs;
using SISDOMI.Entities;
using SISDOMI.Helpers;
using SISDOMI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Cors;

namespace SISDOMI.Controllers
{
    [Route("api/[controller]")]
    public class SesionesEducativasController : Controller
    {
        private readonly SesionesEducativasService _sesionducativaService;
        private readonly IFileStorage _fileStorage;

        public SesionesEducativasController(SesionesEducativasService sesionducativaService, IFileStorage fileStorage)
        {
            _sesionducativaService = sesionducativaService;
            _fileStorage = fileStorage;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<SesionEducativaDTOInicial>>> GetAll([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            return await _sesionducativaService.GetAll(FromDate?.Date.ToString("MM/dd/yyyy"), ToDate?.Date.ToString("MM/dd/yyyy"));
        }

        [HttpGet("allsesiondto/id")]
        public async Task<ActionResult<SesionEducativaDTO>> GetSesionEducativaDTO([FromQuery] string id)
        {
            return await _sesionducativaService.GetSesionEducativaDTO(id);
        }

        [HttpGet("id")]
        public ActionResult<SesionEducativa> Get([FromQuery] string id) //obtiene una sesion educativa segun su id
        {
            return _sesionducativaService.GetById(id);
        }

        [HttpPost("")]
        public async Task<ActionResult<SesionEducativa>> PostSesionesEducativas([FromBody] SesionEducativa sesioneseducativas)
        {
            foreach (var item in sesioneseducativas.contenido.participantes)
            {
                if (!string.IsNullOrWhiteSpace(item.firma))
                {
                    var imgfirma = Convert.FromBase64String(item.firma);
                    item.firma = await _fileStorage.SaveFile(imgfirma, "png", "sesiones");
                }
            }
            return await _sesionducativaService.CreateSesionEducativa(sesioneseducativas);
        }

        [HttpPut("")]
        public async Task<ActionResult<SesionEducativa>> PutSesionesEducativas([FromBody] SesionEducativa sesioneseducativas)
        {
            foreach (var item in sesioneseducativas.contenido.participantes)
            {
                if (!string.IsNullOrWhiteSpace(item.firma) && !item.firma.Contains("https"))
                {
                    var imgfirma = Convert.FromBase64String(item.firma);
                    item.firma = await _fileStorage.SaveFile(imgfirma, "png", "sesiones");
                }
            }
            return await _sesionducativaService.ModifySesionEducativa(sesioneseducativas);
        }
    }
}
