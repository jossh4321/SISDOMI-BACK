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
        [HttpGet("id")]

        [HttpPost("idDoc")]

        public ActionResult<Documento> GetDocumento([FromQuery] string id) //obtiene un documento segun el id del residente

        {
            return _residenteservice.GetByIdDoc(id);
        }    
        public ActionResult<Residentes> PostResidente(Residentes residente) //CREAR RESIDENTE
        {
            Residentes objetoresidente = _residenteservice.CreateUser(residente);
            return objetoresidente;
        }

        [HttpPut("")]
        public ActionResult<Residentes> PutResidente(Residentes residente)  //MODIFICAR RESIDENTE
        {
            Residentes objetoresidente = _residenteservice.ModifyUser(residente);
            return objetoresidente;
        }


    }
}