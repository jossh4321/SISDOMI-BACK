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
    public class EvaluacionDiagnosticoEducativoController : ControllerBase
    {
        private readonly FichaEvaluacionDiagnosticoEducativoService _diagnosticoeducativoservice;
        private readonly IFileStorage _fileStorage;

        public EvaluacionDiagnosticoEducativoController(FichaEvaluacionDiagnosticoEducativoService diagnosticoeducativoservice, IFileStorage fileStorage)
        {
            _diagnosticoeducativoservice = diagnosticoeducativoservice;
            _fileStorage = fileStorage;
        }

        [HttpGet("all")]

        public async Task<ActionResult<List<FichaEvaluacionDiagnosticoEducativoDTO>>> GetAll()
        {
            return await _diagnosticoeducativoservice.GetAll();
        }
        [HttpGet("id")]
        public async Task<ActionResult<DocumentoDTO>> GetById([FromQuery] string id)
        {
            return await _diagnosticoeducativoservice.GetById(id);
        }
        //POST
        [HttpPost("fichaEvaluacionDE")]
        public async Task<ActionResult<FichaEvaluacionDiagnosticoEducativo>> CrearFichaEvaluacionDE(FichaEvaluacionDiagnosticoEducativo informe)
        {           
            foreach (var item in informe.contenido.firmas)
            {
                if (!string.IsNullOrWhiteSpace(item.urlfirma))
                {
                    var imgfirma = Convert.FromBase64String(item.urlfirma);
                    item.urlfirma = await _fileStorage.SaveFile(imgfirma, "png", "informes");
                }
            }
            return await _diagnosticoeducativoservice.RegistrarFichaEvaluacionDE(informe);
        }
        [HttpPut("fichaEvaluacionDE")]
        public async Task<ActionResult<FichaEvaluacionDiagnosticoEducativo>> ModificarFichaEvaluacionDE(FichaEvaluacionDiagnosticoEducativo informe)
        {
            foreach (var item in informe.contenido.firmas)
            {
                if (!string.IsNullOrWhiteSpace(item.urlfirma) && !item.urlfirma.Contains("http"))
                {
                    var imgfirma = Convert.FromBase64String(item.urlfirma);
                    item.urlfirma = await _fileStorage.SaveFile(imgfirma, "png", "informes");
                }
            }
            return await _diagnosticoeducativoservice.ModificarFichaEvaluacionDE(informe);
        }
    }
}
