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
    public class SeguimientoEducativoController : ControllerBase
    {
        private readonly SeguimientoEducativoService _seguimientoeducativoservice;
        
        public SeguimientoEducativoController(SeguimientoEducativoService seguimientoeducativoservice)
        {
            _seguimientoeducativoservice = seguimientoeducativoservice;
        }

        [HttpGet("all")]
        public ActionResult<List<Documento>> GetAll()
        {
            return _seguimientoeducativoservice.GetAll();
        }
    }
}
