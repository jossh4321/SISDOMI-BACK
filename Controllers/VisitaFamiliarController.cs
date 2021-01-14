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
    public class VisitaFamiliarController : ControllerBase
    {
        private readonly VisitaFamiliarService _visitaFamiliarservice;
        private readonly IFileStorage _fileStorage;
        private readonly FaseService faseService;

        public VisitaFamiliarController(VisitaFamiliarService visitafamiliarservice, IFileStorage fileStorage, FaseService faseService)
        {
            _visitaFamiliarservice = visitafamiliarservice;
            _fileStorage = fileStorage;
            this.faseService = faseService;
        }

        [HttpGet("all")]

        public async Task<ActionResult<List<VisitaFamiliarDTO>>> GetAll([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            return await _visitaFamiliarservice.GetAll(FromDate?.Date.ToString("MM/dd/yyyy"), ToDate?.Date.ToString("MM/dd/yyyy"));
        }
        [HttpGet("id")]
        public async Task<ActionResult<DocumentoDTO>> GetById([FromQuery] string id)
        {
            return await _visitaFamiliarservice.GetById(id);
        }
        //POST
        [HttpPost("informeVisita")]
        public async Task<ActionResult<VisitaFamiliar>> CrearInformeVisita(VisitaFamiliar informe)
        {
            /* foreach (var item in informe.contenido.firmas)
             {
                 if (!string.IsNullOrWhiteSpace(item.urlfirma))
                 {
                     var imgfirma = Convert.FromBase64String(item.urlfirma);
                     item.urlfirma = await _fileStorage.SaveFile(imgfirma, "png", "informes");
                 }
             }*/
            return await _visitaFamiliarservice.RegistrarVisitaFamiliar(informe);
        }
        [HttpPut("informeVisita")]
        public async Task<ActionResult<VisitaFamiliar>> ModificarInformeVisita(VisitaFamiliar informe)
        {
            /*foreach (var item in informe.contenido.firmas)
            {
                if (!string.IsNullOrWhiteSpace(item.urlfirma) && !item.urlfirma.Contains("http"))
                {
                    var imgfirma = Convert.FromBase64String(item.urlfirma);
                    item.urlfirma = await _fileStorage.SaveFile(imgfirma, "png", "informes");
                }
            }*/
            return await _visitaFamiliarservice.ModificarVisitaFamiliar(informe);
        }
    }
}
