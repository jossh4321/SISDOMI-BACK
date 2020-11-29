using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class DocumentoController: ControllerBase
    {
        private readonly FichaIngresoSocialService _fichaIngresoSocialService;
        private readonly FichaIngresoEducativoService _fichaIngresoEducativoService;
        private readonly FichaIngresoPsicologicaService _fichaIngresoPsicologicaService ;
        private readonly DocumentoService documentoService;

        public DocumentoController(FichaIngresoSocialService fichaIngresoSocialService, FichaIngresoEducativoService fichaIngresoEducativoService,FichaIngresoPsicologicaService  fichaIngresoPsicologicaService,
                                   DocumentoService documentoService)
        {
            _fichaIngresoSocialService = fichaIngresoSocialService;
            _fichaIngresoEducativoService = fichaIngresoEducativoService;
            _fichaIngresoPsicologicaService = fichaIngresoPsicologicaService;
            this.documentoService = documentoService;
        }

        [HttpGet("all/fichaingresosocial")]

        public ActionResult<List<FichaIngresoSocial>> GetAllFichaIngresoSocial()
        {
            return _fichaIngresoSocialService.GetAll();
        }
        [HttpGet("all/fichaingresoeducativa")]
        public ActionResult<List<FichaIngresoEducativa>> GetAllFichaIngresoEducativa()
        {
            return _fichaIngresoEducativoService.GetAll();
        }
        [HttpGet("all/fichaingresopsicologica")]
        public ActionResult<List<FichaIngresoPsicologica>> GetAllFichaIngresoPsicologica()
        {
            return _fichaIngresoPsicologicaService.GetAll();
        }
        [HttpPut("fichaingresosocial")]
        public ActionResult<FichaIngresoSocial> PutFichaIngresoSocial(FichaIngresoSocial  documento)
        {
            FichaIngresoSocial  objetofichaSocial= _fichaIngresoSocialService.ModifyFichaIngresoSocial(documento);
            return objetofichaSocial;
        }
        [HttpPut("fichaingresoeducativa")]
        public ActionResult<FichaIngresoEducativa> PutFichaIngresoEducativa(FichaIngresoEducativa documento)
        {

            FichaIngresoEducativa objetofichaEducativa = _fichaIngresoEducativoService.ModifyFichaIngresoEducativa(documento);
            return objetofichaEducativa;
        }
        [HttpPut("fichaingresopsicologica")]
        public ActionResult<FichaIngresoPsicologica> PutFichaIngresoPsicologica(FichaIngresoPsicologica  documento)
        {

            FichaIngresoPsicologica objetofichaPsicologica = _fichaIngresoPsicologicaService.ModifyFichaIngresoPsicologica(documento);
            return objetofichaPsicologica;
        }
        [HttpPost("all/fichaingresoeducativacrear")]
        public ActionResult<FichaIngresoEducativa> PostFichaIngresoEducativa(FichaIngresoEducativa  documento) {

           FichaIngresoEducativa  objetofichaEducativa = _fichaIngresoEducativoService.CreateFichaIngresoEducativo(documento);
            return objetofichaEducativa;
        }
        [HttpPost("all/fichaingresosocialcrear")]
        public ActionResult<FichaIngresoSocial> PostFichaIngresoSocial(FichaIngresoSocial documento)
        {
            FichaIngresoSocial objetofichaSocial = _fichaIngresoSocialService.CreateFichaIngresoSocial(documento);
            return objetofichaSocial;
             
        }

        [HttpPost("all/fichaingresopsicologicacrear")]
        public ActionResult<FichaIngresoPsicologica> PostFichaIngresoPsicologica(FichaIngresoPsicologica  documento)
        {
          FichaIngresoPsicologica  objetofichaPsicologica =_fichaIngresoPsicologicaService.CreateFichaIngresoPsicologica(documento);
            return objetofichaPsicologica;
        }
        [HttpGet("all/fichaingresoresidente")]
       public async Task<ActionResult<List<FichaIngresoDTO>>> GetFichaIngresoResidente()
           {
           return await _fichaIngresoSocialService.obtenerResidientesFichaIngreso();
           }

        [HttpGet("tipo/{tipo}/residente/{residenteid}")]
        [Authorize]
        public async Task<ActionResult<List<DocumentTypeResidentDTO>>> GetDocumentosByTypeAndResident(String tipo, String residenteid)
        {
            try
            {
                List<DocumentTypeResidentDTO> lstDocumentTypeResidentDTOs = await documentoService.ListDocumentsByTypeAndResident(tipo, residenteid);

                return lstDocumentTypeResidentDTOs;
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}
