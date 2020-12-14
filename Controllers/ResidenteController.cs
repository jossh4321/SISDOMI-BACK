using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

    public class ResidenteController : ControllerBase
    {
        private readonly ResidenteService _residenteservice;
        private readonly IFileStorage _fileStorage;

        public ResidenteController(ResidenteService residenteservice, IFileStorage fileStorage)
        {
            _residenteservice = residenteservice;
            _fileStorage = fileStorage;
        }

        [HttpGet("all")]
        public ActionResult<List<Residentes>> GetAll()
        {
            return _residenteservice.GetAll();
        }
    
        [HttpGet("id")]
        public ActionResult<Residentes> Get([FromQuery] string id) //obtiene un residente segun su id
        {
            return _residenteservice.GetById(id);
        }
        [HttpGet("idDoc")]
        public ActionResult<Documento> GetDocumento([FromQuery] string id) //obtiene un documento segun el id del residente

        {
            return _residenteservice.GetByIdDoc(id); 
        }
        [HttpPost("")]
        public async Task<ActionResult<Residentes>> PostResidente(ResidenteDTO2 residente) //CREAR RESIDENTE
        {
            Residentes objetoresidente = await _residenteservice.CreateUser(residente);
            return objetoresidente;
        }

        [HttpPut("")]
        public async Task<ActionResult<Residentes>> PutResidente(ResidenteFaseDTO residenteFase)  //MODIFICAR RESIDENTE
        {
            if(residenteFase.promocion == true)
            {
                if (!string.IsNullOrWhiteSpace(residenteFase.progresoFase.documentotransicion.firma.urlfirma))
                {
                    var imgfirma = Convert.FromBase64String(residenteFase.progresoFase.documentotransicion.firma.urlfirma);
                    residenteFase.progresoFase.documentotransicion.firma.urlfirma = await _fileStorage.SaveFile(imgfirma, "png", "sesiones");
                }
            }  
            Residentes objetoresidente = _residenteservice.ModifyUser(residenteFase);
            return objetoresidente;
        }

        [HttpGet("planes/area/{area}")]
        [Authorize]
        public async Task<ActionResult<List<Residentes>>> GetAllByAreaAndNotPlan(String area) 
        {
            try
            {
                List<Residentes> lstResidentes = await _residenteservice.ListResidentByAreaAndByNotPlan(area);

                return lstResidentes;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet("nombre/{nombre}")]
        [Authorize]
        public async Task<ActionResult<List<Residentes>>> GetAllByNombre(String nombre)
        {
            try
            {
                List<Residentes> listResidentes = await _residenteservice.GetResidenteByNombre(nombre);
                return listResidentes;
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet("{residenteId}/expediente")]
        [Authorize]
        public async Task<ActionResult<ResidenteDTO>> GetResidenteAndAnnexesAndDocuments(String residenteId)
        {
            try
            {

                ResidenteDTO residenteDTO = await _residenteservice.GetResidentAndAnnexesAndDocuments(residenteId);

                return residenteDTO;
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet("all/fase/{fase}")]
        public async Task<ActionResult<List<Residentes>>> GetAllByFase(String fase)
        {
            try
            {
                List<Residentes> lstResidentes = await _residenteservice.ListResidenteByFase(fase);

                return lstResidentes;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

    }
}