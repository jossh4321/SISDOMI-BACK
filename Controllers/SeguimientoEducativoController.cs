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
    [ApiController]
    [Route("api/[controller]")]
    public class SeguimientoEducativoController : ControllerBase
    {
        private readonly SeguimientoEducativoService _seguimientoeducativoservice;
        private readonly IFileStorage _fileStorage;

        public SeguimientoEducativoController(SeguimientoEducativoService seguimientoeducativoservice)
        {
            _seguimientoeducativoservice = seguimientoeducativoservice;
        }

        [HttpGet("all")]
        
        public async Task<ActionResult<List<InformeDTO>>> GetAll()
        {
            return await _seguimientoeducativoservice.GetAll();
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
            foreach (var item in informe.contenido.firmas)
            {
                if (!string.IsNullOrWhiteSpace(item.urlfirma))
                {
                    var imgfirma = Convert.FromBase64String(item.urlfirma);
                    item.urlfirma = await _fileStorage.SaveFile(imgfirma, "png", "informes");
                }
            }
            return await _seguimientoeducativoservice.RegistrarInformeSE(informe);
        }
        [HttpPut("informese")]
        public ActionResult<InformeSeguimientoEducativo> ModificarInformeSE(InformeSeguimientoEducativo informe)
        {
            return _seguimientoeducativoservice.ModificarInformeSE(informe);
        }
    }
}
