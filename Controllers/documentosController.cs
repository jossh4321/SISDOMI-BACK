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

        [HttpPut("fichaingresosocial")]
        public ActionResult<FichaIngresoSocial> ModificarFichaIngresoSocial(FichaIngresoSocial fichaIngresoSocial)
        {
            FichaIngresoSocial objetofichaSocial = _fichaIngresoSocialService.ModifyFichaIngresoSocial(fichaIngresoSocial);
            return objetofichaSocial;
        }
        [HttpPut("fichaingresoeducativa")]
        public ActionResult<FichaIngresoEducativa> ModificarFichaIngresoEducativa(FichaIngresoEducativa fichaIngresoEducativa)
        {
            FichaIngresoEducativa objetofichaEducativa = _fichaIngresoEducativoService.ModifyFichaIngresoEducativa(fichaIngresoEducativa);
            return objetofichaEducativa;
        }
      
    }
}
