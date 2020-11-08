using Microsoft.AspNetCore.Mvc;
using SISDOMI.DTOs;
using SISDOMI.Entities;
using SISDOMI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace SISDOMI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentoController
    {
        private readonly FichaIngresoSocialService _fichaIngresoSocialService;
        private readonly FichaIngresoEducativoService _fichaIngresoEducativoService;

        public DocumentoController(FichaIngresoSocialService fichaIngresoSocialService, FichaIngresoEducativoService fichaIngresoEducativoService)
        {
            _fichaIngresoSocialService = fichaIngresoSocialService;
            _fichaIngresoEducativoService = fichaIngresoEducativoService;
        }

        [HttpGet("all/fichaingresosocial")]

        public ActionResult<List<Documento>> GetAllFichaIngresoSocial()
        {
            return _fichaIngresoSocialService.GetAll();
        }
        [HttpGet("all/fichaingresoeducativa")]
        public ActionResult<List<Documento>> GetAllFichaIngresoEducativa()
        {
            return _fichaIngresoEducativoService.GetAll();
        }

        [HttpPut("fichaingresosocial")]
        public ActionResult<Documento> PutFichaIngresoSocial(Documento documento)
        {
            Documento objetofichaEducativa = _fichaIngresoSocialService.ModifyFichaIngresoSocial(documento);
            return objetofichaEducativa;
        }

        [HttpPost("all/fichaingresoeducativacrear")]
        public ActionResult<Documento> PostFichaIngresoEducativa(Documento documento) {

            Documento objetofichaEducativa = _fichaIngresoEducativoService.CreateFichaIngresoEducativo(documento);
            return objetofichaEducativa;
        }
        [HttpPut("fichaingresoeducativa")]
        public ActionResult<Documento> PutFichaIngresoEducativa(Documento documento)
        {

            Documento objetofichaEducativa = _fichaIngresoEducativoService.ModifyFichaIngresoEducativa(documento);
            return objetofichaEducativa;
        }
       [HttpGet("all/fichaingresoresidente")]
       public async Task<ActionResult<List<FichaIngresoDTO>>> GetFichaIngresoResidente()
           {
           return await _fichaIngresoSocialService.obtenerResidientesFichaIngreso();
           }
    }
}
