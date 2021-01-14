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
    [ApiController]
    public class SeguimientoEducativoController : ControllerBase
    {
        private readonly SeguimientoEducativoService _seguimientoeducativoservice;
        private readonly IFileStorage _fileStorage;
        private readonly FaseService faseService;

        public SeguimientoEducativoController(SeguimientoEducativoService seguimientoeducativoservice, IFileStorage fileStorage, FaseService faseService)
        {
            _seguimientoeducativoservice = seguimientoeducativoservice;
            _fileStorage = fileStorage;
            this.faseService = faseService;
        }

        [HttpGet("all")]
        
        public async Task<ActionResult<List<SeguimientoDTO>>> GetAll([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            return await _seguimientoeducativoservice.GetAll(FromDate?.Date.ToString("MM/dd/yyyy"), ToDate?.Date.ToString("MM/dd/yyyy"));
        }
        [HttpGet("id")]
        public async Task<ActionResult<DocumentoDTO>> GetById([FromQuery] string id)
        {
            return await _seguimientoeducativoservice.GetById(id);
        }
        //POST
        [HttpPost("informese")]
        public async Task<ActionResult<InformeSeguimientoEducativo>> CrearInformeSE(InformeSeguimientoEducativo informe)
        {
           /* foreach (var item in informe.contenido.firmas)
            {
                if (!string.IsNullOrWhiteSpace(item.urlfirma))
                {
                    var imgfirma = Convert.FromBase64String(item.urlfirma);
                    item.urlfirma = await _fileStorage.SaveFile(imgfirma, "png", "informes");
                }
            }*/
            return await _seguimientoeducativoservice.RegistrarInformeSE(informe);
        }
        [HttpPut("informese")]
        public async Task <ActionResult<InformeSeguimientoEducativo>> ModificarInformeSE(InformeSeguimientoEducativo informe)
        {
            /*foreach (var item in informe.contenido.firmas)
            {
                if (!string.IsNullOrWhiteSpace(item.urlfirma) && !item.urlfirma.Contains("http"))
                {
                    var imgfirma = Convert.FromBase64String(item.urlfirma);
                    item.urlfirma = await _fileStorage.SaveFile(imgfirma, "png", "informes");
                }
            }*/
            return await _seguimientoeducativoservice.ModificarInformeSE(informe);
        }
    }
}
