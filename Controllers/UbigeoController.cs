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
    public class UbigeoController : ControllerBase
    {
        private readonly UbigeoService _ubigeoservice;
        private readonly IFileStorage _fileStorage;
        public UbigeoController(UbigeoService ubigeoservice, IFileStorage fileStorage)
        {
            _ubigeoservice = ubigeoservice;
            _fileStorage = fileStorage;
        }

        [HttpGet("allDepartamentos")]        
        public ActionResult<List<Ubigeo.Departamento>> GetAllDepartamentos()
        {
            return _ubigeoservice.GetAllDepartamentos();
        }
        [HttpGet("allProvincias")]
        public ActionResult<List<Ubigeo.Provincia>> GetAllProvincias()
        {
            return _ubigeoservice.GetAllProvincias();
        }
        [HttpGet("allDistritos")]
        public ActionResult<List<Ubigeo.Distrito>> GetAllDistritos()
        {
            return _ubigeoservice.GetAllDistritos();
        }

        [HttpGet("idDistrito")]
        public ActionResult<Ubigeo.Distrito> GetDistrito([FromQuery] string idDistrito)
        {
            return _ubigeoservice.GetDistrictById(idDistrito);
        }

        [HttpGet("idProvincia")]
        public ActionResult<Ubigeo.Provincia> GetProvincia([FromQuery] string idProvincia)
        {
            return _ubigeoservice.GetProvinceById(idProvincia);
        }

        [HttpGet("provincias/departamento/{idDepartamento}")]
        public async Task<ActionResult<List<Ubigeo.Provincia>>> GetProvincesByDepartment(String idDepartamento)
        {
            try
            {
                List<Ubigeo.Provincia> lstProvincias = await _ubigeoservice.GetProvincesByIdDepartment(idDepartamento);

                return lstProvincias;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet("distritos/provincia/{idProvincia}")]
        public async Task<ActionResult<List<Ubigeo.Distrito>>> GetDistrictsByProvince(String idProvincia)
        {
            try
            {
                List<Ubigeo.Distrito> lstDistritos = await _ubigeoservice.GetDistrictsByIdProvince(idProvincia);

                return lstDistritos;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

    }
}
