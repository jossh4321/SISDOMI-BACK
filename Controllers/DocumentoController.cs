using Microsoft.AspNetCore.Authorization;
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
        private readonly EntrevistaFamiliarService _entrevistaFamiliarService;
        private readonly DashBoardService _dashBoardService;
        private readonly DocumentoService documentoService;
        private readonly IFileStorage _fileStorage;

        public DocumentoController(IFileStorage fileStorage, FichaIngresoSocialService fichaIngresoSocialService, FichaIngresoEducativoService fichaIngresoEducativoService,FichaIngresoPsicologicaService  fichaIngresoPsicologicaService,
                                   DocumentoService documentoService,
                                   DashBoardService dashBoardService)
        {
            _entrevistaFamiliarService = entrevistaFamiliarService;
            _fichaIngresoSocialService = fichaIngresoSocialService;
            _fichaIngresoEducativoService = fichaIngresoEducativoService;
            _fichaIngresoPsicologicaService = fichaIngresoPsicologicaService;
            _dashBoardService = dashBoardService;
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
            /*if (!string.IsNullOrWhiteSpace(documento.contenido.firma.urlfirma))
            {
                if (!documento.contenido.firma.urlfirma.Contains("https://") && !documento.contenido.firma.urlfirma.Contains("http://"))
                {
                    var imgfirma = Convert.FromBase64String(documento.contenido.firma.urlfirma);
                    documento.contenido.firma.urlfirma = await _fileStorage.SaveFile(imgfirma, "jpg", "fichaingreso");
                }
            }*/
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
        public async Task<ActionResult<FichaIngresoDTO>> PutFichaIngresoPsicologica(FichaIngresoPsicologica  documento)
        {
            /*if (!string.IsNullOrWhiteSpace(documento.contenido.firma.urlfirma))
            {
                if (!documento.contenido.firma.urlfirma.Contains("https://") && !documento.contenido.firma.urlfirma.Contains("http://"))
                {
                    var imgfirma = Convert.FromBase64String(documento.contenido.firma.urlfirma);
                    documento.contenido.firma.urlfirma = await _fileStorage.SaveFile(imgfirma, "jpg", "fichaingreso");
                }
            }*/
            return await _fichaIngresoPsicologicaService.ModifyFichaIngresoPsicologica(documento);
        }
        [HttpPost("fichaeducativaingreso")]
        public async Task<ActionResult<FichaIngresoDTO>> PostFichaIngresoEducativa(FichaIngresoEducativa  documento)
        {
            if (!string.IsNullOrWhiteSpace(documento.historialcontenido[0].url))
            {
                var solicitudBytes2 = Convert.FromBase64String(documento.historialcontenido[0].url);
                documento.historialcontenido[0].url = await _fileStorage.SaveDoc(solicitudBytes2, "pdf", "archivos");
            }
            documento.historialcontenido[0].version = 1;
            documento.historialcontenido[0].fechamodificacion = DateTime.UtcNow.AddHours(-5);
            FichaIngresoDTO objetofichaEducativa = await _fichaIngresoEducativoService.CreateFichaIngresoEducativo(documento);
           return objetofichaEducativa;
        }
        [HttpPost("fichaingresosocialcrear")]
        public async Task<ActionResult<FichaIngresoDTO>> PostFichaIngresoSocial(FichaIngresoSocial documento)
        {         
           /* if (!string.IsNullOrWhiteSpace(documento.contenido.firma.urlfirma))
            {
                var imgfirma = Convert.FromBase64String(documento.contenido.firma.urlfirma);
                documento.contenido.firma.urlfirma = await _fileStorage.SaveFile(imgfirma, "jpg", "fichaingreso");
            }*/
            return await _fichaIngresoSocialService.CreateFichaIngresoSocial(documento);
        }

        [HttpPost("fichaingresopsicologicacrear")]
        public async Task<ActionResult<FichaIngresoDTO>> PostFichaIngresoPsicologica(FichaIngresoPsicologica  documento)
        {            
            /*if (!string.IsNullOrWhiteSpace(documento.contenido.firma.urlfirma))
            {
                var imgfirma = Convert.FromBase64String(documento.contenido.firma.urlfirma);
                documento.contenido.firma.urlfirma = await _fileStorage.SaveFile(imgfirma, "jpg", "fichaingreso");
            }*/
            return await _fichaIngresoPsicologicaService.CreateFichaIngresoPsicologica(documento);
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

        [HttpGet("{tipodoc}/residente/{residenteid}")]
        public async Task<ActionResult<List<DocumentoDTO>>> GetDocumentoByTypeAndResident(String tipodoc, String residenteid)
        {
            try
            {
                List<DocumentoDTO> documentos = await documentoService.GetByIdResidenteAndTipo(tipodoc, residenteid);

                return documentos;
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
        //ENTREVISTAS FAMILIARES
        [HttpGet("entrevistafamiliar/all")]
        public async Task<ActionResult<List<EntrevistaFamiliarDTO>>> GetAll()
        {
            return await _entrevistaFamiliarService.GetAll();
        }
        [HttpGet("entrevistafamiliar/iddoc/{id}")]
        public EntrevistaFamiliar getEntrevistaFamiliarPorId(string id)
        {
            return _entrevistaFamiliarService.getByIdEntrevistaFamiliar(id);
        }
        [HttpPost("entrevistafamiliar")]
        public async Task<ActionResult<EntrevistaFamiliar>> PostEntrevistaFamiliar(EntrevistaFamiliar documento)
        {
            EntrevistaFamiliar obj = await _entrevistaFamiliarService.CreateEntrevistaFamiliar(documento);
            return obj;
        }
        [HttpPut("entrevistafamiliar")]
        public ActionResult<EntrevistaFamiliar> PutEntrevistaFamiliar(EntrevistaFamiliar documento)
        {
            EntrevistaFamiliar objetofichaEducativa = _entrevistaFamiliarService.ModifyEntrevistaFamiliar(documento);
            return objetofichaEducativa;
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardDTO>> obtenerDashboard()
        {
            try
            {
                DashboardDTO dashboard = await _dashBoardService.obtenerDashBoard();
                return dashboard;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}
