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
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult<List<Usuario>> GetAll()
        {
            return _usuarioservice.GetAll();
        }

        [HttpGet("id")]
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

        [HttpPut("")]
        public async Task<ActionResult<Usuario>> ModificarUsuario([FromQuery] string tipo, [FromQuery] string modificado, Usuario usuario)
        {
            Usuario usuariobd = new Usuario();
            usuariobd = _usuarioservice.GetById(usuario.id);

            if (!string.IsNullOrWhiteSpace(usuario.datos.imagen) && tipo != "url")
            {
                var profileimg = Convert.FromBase64String(usuario.datos.imagen);
                usuario.datos.imagen = await _fileStorage.EditFile(profileimg, "jpg", "usuarios",usuariobd.datos.imagen);
            }
            Usuario objetousuario = _usuarioservice.ModifyUser(usuario);
            return objetousuario;
        }
        [HttpPut("estado")]
        public ActionResult<Usuario> ModificarEstadoUsuario([FromQuery] string id,[FromQuery] string nuevoestado)
        {
            Usuario objetousuario =  _usuarioservice.ModifyState(id, nuevoestado);
            return objetousuario;
        }

        [HttpPut("clave")]
        public ActionResult<Usuario> ModificarClaveUsuario([FromQuery] string id, [FromQuery] string nuevacontrasena)
        {
            Usuario objetousuario = _usuarioservice.ModifyPassword(id, nuevacontrasena);
            return objetousuario;
        }

        [HttpDelete("")]
        public async Task<ActionResult<String>> EliminarUsuario([FromQuery] string id)
        {
           await  _usuarioservice.DeleteUser(id);
            return $"se elimin el usuario {id}";
        }


        [HttpGet("rol/permiso")]
        [AllowAnonymous]
        public async Task<ActionResult<UsuarioDTOR>> ObtenerUsuarioRolPermiso([FromQuery]string id)
        {
           return await _usuarioservice.ObtenerUsuarioRolPermiso(id);
        }

        [HttpGet("roles/permisos")]
        [AllowAnonymous]
        public async Task<ActionResult<List<UsuarioDTOR>>> ObtenerUsuariosRolesPermisos()
        {
            return await _usuarioservice.ObtenerUsuariosRolesPermisos();
        }

        [HttpGet("rol")]
        [AllowAnonymous]
        public async Task<ActionResult<UsuarioDTO_UnwindRol>> ObtenerUsuarioRol([FromQuery]string id)
        {
            return await _usuarioservice.ObtenerUsuarioRol(id);
        }

        [HttpGet("roles")]
        [AllowAnonymous]
        public async Task<ActionResult<List<UsuarioDTO_UnwindRol>>> ObtenerUsuariosRoles()
        {
            return await _usuarioservice.ObtenerUsuariosRoles();
        }

        [HttpGet("sistema/rol")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Rol>>> ObtenerRolesSistema()
        {
            return await _usuarioservice.obtenerRolesSistema();
        }

        [HttpGet("idrol")]
        public ActionResult<List<Usuario>> GetUsuarioxRol([FromQuery] string idRol)
        {
            return _usuarioservice.GetByRol(idRol);
        }       
    }
}
