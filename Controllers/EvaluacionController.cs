using Microsoft.AspNetCore.Mvc;
using SISDOMI.DTOs;
using SISDOMI.Entities;
using SISDOMI.Helpers;
using SISDOMI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace SISDOMI.Controllers{
    [ApiController]
    [Route("api/[controller]")]
    public class EvaluacionController{
        private readonly FichaEvaluacionDiagnosticoEducativoService evaluacionService;
        private readonly IFileStorage fileStorage;

        public EvaluacionController(FichaEvaluacionDiagnosticoEducativoService evaluacionService, IFileStorage fileStorage){
            this.evaluacionService = evaluacionService;
            this.fileStorage = fileStorage;
        }

        [HttpGet("all")]
        public ActionResult<List<FichaEvaluacionDiagnosticoEducativo>> GetAll(){
            return evaluacionService.GetAll();
        }

        [HttpGet("id")]
        public async Task<ActionResult<DocumentoDTO>> GetById([FromQuery] string id)
        {
            return await evaluacionService.GetById(id);
        }

        [HttpPost("evaluacion")]
        public async Task<ActionResult<FichaEvaluacionDiagnosticoEducativo>> CreateEvaluation(FichaEvaluacionDiagnosticoEducativo evaluacion)
        {
            foreach (var item in evaluacion.contenido.firmas){
                if (!string.IsNullOrWhiteSpace(item.urlfirma)){
                    var imgfirma = Convert.FromBase64String(item.urlfirma);
                    item.urlfirma = await this.fileStorage.SaveFile(imgfirma, "png", "informes");
                }
            }
            return await this.evaluacionService.CreateEvaluation(evaluacion);
        }

        [HttpPut("evaluacion")]
        public async Task<ActionResult<FichaEvaluacionDiagnosticoEducativo>> ModifyEvaluation(FichaEvaluacionDiagnosticoEducativo evaluacion)
        {
            foreach (var item in evaluacion.contenido.firmas)
            {
                if (!string.IsNullOrWhiteSpace(item.urlfirma) && !item.urlfirma.Contains("http"))
                {
                    var imgfirma = Convert.FromBase64String(item.urlfirma);
                    item.urlfirma = await this.fileStorage.SaveFile(imgfirma, "png", "informes");
                }
            }
            return this.evaluacionService.ModifyEvaluation(evaluacion);
        }

    }
}
