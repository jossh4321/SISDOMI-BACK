﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SISDOMI.DTOs;
using SISDOMI.Entities;
using SISDOMI.Helpers;
using SISDOMI.Models;
using SISDOMI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentoController : ControllerBase
    {
        private readonly FichaIngresoSocialService _fichaIngresoSocialService;
        private readonly FichaIngresoEducativoService _fichaIngresoEducativoService;
        private readonly FichaIngresoPsicologicaService _fichaIngresoPsicologicaService ;
        private readonly DocumentoService documentoService;
        private readonly IFileStorage _fileStorage;

        public DocumentoController(IFileStorage fileStorage, FichaIngresoSocialService fichaIngresoSocialService, FichaIngresoEducativoService fichaIngresoEducativoService,FichaIngresoPsicologicaService  fichaIngresoPsicologicaService,
                                   DocumentoService documentoService)
        {
            _fichaIngresoSocialService = fichaIngresoSocialService;
            _fichaIngresoEducativoService = fichaIngresoEducativoService;
            _fichaIngresoPsicologicaService = fichaIngresoPsicologicaService;
            _fileStorage = fileStorage;
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
        public async Task<ActionResult<FichaIngresoDTO>> PutFichaIngresoSocial(FichaIngresoSocial  documento)
        {
            FichaIngresoDTO objetofichaSocial = await _fichaIngresoSocialService.ModifyFichaIngresoSocial(documento);
            return objetofichaSocial;
        }
        [HttpPut("fichaingresoeducativa")]
        public async Task<ActionResult<FichaIngresoDTO>> PutFichaIngresoEducativa(FichaIngresoEducativa documento)
        {

            FichaIngresoDTO objetofichaEducativa = await  _fichaIngresoEducativoService.ModifyFichaIngresoEducativa(documento);
            return objetofichaEducativa;
        }
        [HttpPut("fichaingresopsicologica")]
        public ActionResult<FichaIngresoPsicologica> PutFichaIngresoPsicologica(FichaIngresoPsicologica  documento)
        {

            FichaIngresoPsicologica objetofichaPsicologica = _fichaIngresoPsicologicaService.ModifyFichaIngresoPsicologica(documento);
            return objetofichaPsicologica;
        }
        [HttpPost("fichaeducativaingreso")]
        public async Task<ActionResult<FichaIngresoDTO>> PostFichaIngresoEducativa(FichaIngresoEducativa  documento) {

           FichaIngresoDTO objetofichaEducativa = await _fichaIngresoEducativoService.CreateFichaIngresoEducativo(documento);
           return objetofichaEducativa;
        }
        [HttpPost("fichaingresosocialcrear")]
        public async Task<ActionResult<FichaIngresoDTO>> PostFichaIngresoSocial(FichaIngresoSocial documento)
        {
            if (!string.IsNullOrWhiteSpace(documento.contenido.firma.urlfirma))
            {
                var imgfirma = Convert.FromBase64String(documento.contenido.firma.urlfirma);
                documento.contenido.firma.urlfirma = await _fileStorage.SaveFile(imgfirma, "jpg", "fichaingreso");
            }
            return await _fichaIngresoSocialService.CreateFichaIngresoSocial(documento);
        }

        [HttpPost("fichaingresopsicologicacrear")]
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
        [HttpGet("fichaingreso/detalle/{id}")]
        public async Task<ActionResult<FichaIngresoDetalleDTO>> GetFichaIngresoResidenteDetalle(string id)
        {
            return await documentoService.getFichaIngresoDetalleDtoById(id);
        }

        [HttpGet("fichaingreso/idficha/{id}")]
        public Object getFichaIngresoGenericaPorId(string id)
        {
            
            return _fichaIngresoEducativoService.GetFichaIngresoDTO2PorId(id);
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

        [HttpGet("{id}")]
        public async Task<ActionResult<DocumentoExpedienteDTO>> GetById(String id)
        {
            try
            {
                DocumentoExpedienteDTO documentoExpedienteDTO;

                documentoExpedienteDTO = await documentoService.GetById(id);

                return documentoExpedienteDTO;
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet("fichaingresoeducativa/idresidente/{idresidente}")]
        public ActionResult<FichaIngresoEducativa> GetFichaIngresoSocialByIdResidente(String idresidente)
        {
            FichaIngresoEducativa fichaIngresoEducativa = _fichaIngresoEducativoService.GetByResidenteId(idresidente);
            return fichaIngresoEducativa;
        }
    }
}
