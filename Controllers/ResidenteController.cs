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

        public ResidenteController(ResidenteService residenteservice)
        {

            _residenteservice = residenteservice;
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
        public async Task<ActionResult<Residentes>> PostResidente(Residentes residente) //CREAR RESIDENTE
        {
            Residentes objetoresidente = await _residenteservice.CreateUser(residente);
            return objetoresidente;
        }

        [HttpPut("")]
        public ActionResult<Residentes> PutResidente(Residentes residente)  //MODIFICAR RESIDENTE
        {
            Residentes objetoresidente = _residenteservice.ModifyUser(residente);
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

        [HttpGet("all/fase/{fase}")]
        public async Task<ActionResult<List<Residentes>>> ListResidenteByFase(String fase)
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