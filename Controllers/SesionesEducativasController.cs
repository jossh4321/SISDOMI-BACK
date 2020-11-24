﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
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
    [Route("api/[controller]")]
    public class SesionesEducativasController : Controller
    {
        private readonly SesionesEducativasService _sesionducativaService;
        private readonly IFileStorage _fileStorage;

        public SesionesEducativasController(SesionesEducativasService sesionducativaService, IFileStorage fileStorage)
        {
            _sesionducativaService = sesionducativaService;
            _fileStorage = fileStorage;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<SesionEducativa>>> GetAll()
        {
            return await _sesionducativaService.GetAll();
        }

        [HttpGet("id")]
        public ActionResult<SesionEducativa> Get([FromQuery] string id) //obtiene una sesion educativa segun su id
        {
            return _sesionducativaService.GetById(id);
        }
    }
}
