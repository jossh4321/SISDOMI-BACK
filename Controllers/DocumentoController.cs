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
        private readonly FichaIngresoPsicologicaService _fichaIngresoPsicologicaService ;
        public DocumentoController(FichaIngresoSocialService fichaIngresoSocialService, FichaIngresoEducativoService fichaIngresoEducativoService,FichaIngresoPsicologicaService  fichaIngresoPsicologicaService)
        {
            _fichaIngresoSocialService = fichaIngresoSocialService;
            _fichaIngresoEducativoService = fichaIngresoEducativoService;
            _fichaIngresoPsicologicaService = fichaIngresoPsicologicaService;
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
        [HttpGet("all/fichaingresopsicologica")]
        public ActionResult<List<Documento>> GetAllFichaIngresoPsicologica()
        {
            return _fichaIngresoPsicologicaService.GetAll();
        }
        [HttpPut("fichaingresosocial")]
        public ActionResult<Documento> PutFichaIngresoSocial(Documento documento)
        {
            Documento objetofichaEducativa = _fichaIngresoSocialService.ModifyFichaIngresoSocial(documento);
            return objetofichaEducativa;
        }
        [HttpPut("fichaingresoeducativa")]
        public ActionResult<Documento> PutFichaIngresoEducativa(Documento documento)
        {

            Documento objetofichaEducativa = _fichaIngresoEducativoService.ModifyFichaIngresoEducativa(documento);
            return objetofichaEducativa;
        }
        [HttpPut("fichaingresopsicologica")]
        public ActionResult<Documento> PutFichaIngresoPsicologica(Documento documento)
        {

            Documento objetofichaPsicologica= _fichaIngresoPsicologicaService.ModifyFichaIngresoPsicologica(documento);
            return objetofichaPsicologica;
        }
        [HttpPost("all/fichaingresoeducativacrear")]
        public ActionResult<Documento> PostFichaIngresoEducativa(Documento documento) {

            Documento objetofichaEducativa = _fichaIngresoEducativoService.CreateFichaIngresoEducativo(documento);
            return objetofichaEducativa;
        }
        [HttpPost("all/fichaingresosocialcrear")]
        public ActionResult<Documento> PostFichaIngresoSocial(Documento documento)
        {

            Documento objetofichaSocial = _fichaIngresoSocialService.CreateFichaIngresoSocial(documento);
            return objetofichaSocial;
        }
        [HttpPost("all/fichaingresopsicologicacrear")]
        public ActionResult<Documento> PostFichaIngresoPsicologica(Documento documento)
        {

            Documento objetofichaPsicologica = _fichaIngresoPsicologicaService.CreateFichaIngresoPsicologica(documento);
            return objetofichaPsicologica;
        }
        [HttpGet("all/fichaingresoresidente")]
       public async Task<ActionResult<List<FichaIngresoDTO>>> GetFichaIngresoResidente()
           {
           return await _fichaIngresoSocialService.obtenerResidientesFichaIngreso();
           }
    }
}
