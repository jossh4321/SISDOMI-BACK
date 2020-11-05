using Microsoft.AspNetCore.Mvc;
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
    public class DocumentosController
    {
        private readonly FichaIngresoSocialService _fichaIngresoSocialService;
        private readonly FichaIngresoEducativoService _fichaIngresoEducativoService;

        public DocumentosController(FichaIngresoSocialService fichaIngresoSocialService, FichaIngresoEducativoService fichaIngresoEducativoService)
        {
            _fichaIngresoSocialService = fichaIngresoSocialService;
            _fichaIngresoEducativoService = fichaIngresoEducativoService;
        }
      
        [HttpGet("all")]

        public ActionResult<List<FichaIngresoSocial>> GetAll()
        {
            return _fichaIngresoSocialService.GetAll();
        }
        [HttpGet("FichaIngresoSocial")]
        public ActionResult<List<FichaIngresoEducativa>> GetAllEducativa()
       {
           return _fichaIngresoEducativoService.GetAll();
       }
        [HttpGet("FichaIngresoEducativa")]
        public ActionResult<FichaIngresoSocial> ModificarFichaIngresoSocial(FichaIngresoSocial fichaIngresoSocial)
        {
            FichaIngresoSocial objetofichaSocial = _fichaIngresoSocialService.ModifyFichaIngresoSocial(fichaIngresoSocial);
            return objetofichaSocial;
        }
        [HttpPut("ActualizarFichaIngresoSocial")]
        public ActionResult<FichaIngresoEducativa> ModificarFichaIngresoEducativa(FichaIngresoEducativa fichaIngresoEducativa)
        {
            FichaIngresoEducativa objetofichaEducativa = _fichaIngresoEducativoService.ModifyFichaIngresoEducativa(fichaIngresoEducativa);
            return objetofichaEducativa;
        }
      
    }
}
