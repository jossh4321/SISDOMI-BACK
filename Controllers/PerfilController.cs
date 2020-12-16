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

    [ApiController]
    [Route("api/[controller]")]
    public class PerfilController : ControllerBase
    {

        private readonly PerfilService _Perfilservice;
        private readonly IFileStorage _fileStorage;
        public PerfilController(PerfilService perfilService, IFileStorage fileStorage)
        {
            _Perfilservice = perfilService;
            _fileStorage = fileStorage;
        }


        [HttpGet("id")]
        public ActionResult<Usuario> Get([FromQuery] string id)
        {
            return _Perfilservice.GetById(id);
        }



        [HttpPut("modificarperfil")]
        public async Task<ActionResult<Usuario>> ModificarUsuario(Usuario usuario)
        {
            if (!string.IsNullOrWhiteSpace(usuario.datos.imagen))
            {
                var profileimg = Convert.FromBase64String(usuario.datos.imagen);
                usuario.datos.imagen = await _fileStorage.SaveFile(profileimg, "jpg", "usuarios");
            }
            Usuario MPerfil = _Perfilservice.ModifyUser(usuario);
            return MPerfil;
        }


    }

}