using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SISDOMI.Entities;
using SISDOMI.Helpers;
using SISDOMI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController: ControllerBase
    {
        private readonly UsuarioService _usuarioservice;
        private readonly IFileStorage _fileStorage;
        public UsuarioController( UsuarioService usuarioservice, IFileStorage fileStorage)
        {
            _usuarioservice = usuarioservice;
            _fileStorage = fileStorage;
        }

        [HttpGet("all")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult<List<Usuario>> GetAll()
        {
            return _usuarioservice.GetAll();
        }

        [HttpGet("byid")]
        public ActionResult<Usuario> Get([FromQuery] string id)
        {
            return _usuarioservice.GetById(id);
        }

        [HttpPost("")]
        public async Task<ActionResult<Usuario>> CrearUsuario(Usuario usuario)
        {
            if (!string.IsNullOrWhiteSpace(usuario.datos.imagen))
            {
                var profileimg = Convert.FromBase64String(usuario.datos.imagen);
                usuario.datos.imagen = await _fileStorage.SaveFile(profileimg, "jpg", "usuarios");
            }
            Usuario objetousuario = _usuarioservice.CreateUser(usuario);
            return objetousuario;
        }

    }
}
