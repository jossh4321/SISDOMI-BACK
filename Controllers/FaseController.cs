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
    public class FaseController
    {
        private readonly FaseService _faseservice;
        public FaseController(FaseService faseservice)
        {

            _faseservice = faseservice;
        }

        [HttpGet("id")]
        public ActionResult<Fase> Get([FromQuery] string id) //obtiene un residente segun su id
        {
            return _faseservice.GetByIdResidente(id);
        }

        [HttpPut("")]
        public ActionResult<Fase> PutResidente(Fase documentofase)
        {
            /*
            foreach (var item in documentofase.progreso)
            {
                if (!string.IsNullOrWhiteSpace(item.firma) && !item.firma.Contains("https"))
                {
                    var imgfirma = Convert.FromBase64String(item.firma);
                    item.firma = await _fileStorage.SaveFile(imgfirma, "png", "sesiones");
                }
            }*/
            Fase objetofase = _faseservice.ModifyFase(documentofase);
            return objetofase;
        }
    }
}
