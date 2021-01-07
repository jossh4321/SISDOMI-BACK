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
    public class NotificacionController
    {
        private readonly NotificacionService _notificacionservice;
        public NotificacionController(NotificacionService notificacionservice)
        {

            _notificacionservice = notificacionservice;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NotificacionDTO>> Get(string id) //obtiene un residente segun su id
        {
            NotificacionDTO notificacion = new NotificacionDTO();
            var listaNotificaciones = await _notificacionservice.GetNotificationById(id);
            notificacion.listaNotificaciones = listaNotificaciones;
            notificacion.contadorNotificaciones = listaNotificaciones.Count();

            return notificacion;
        }

        [HttpPost("")]
        public ActionResult<Notificacion> PostSesionesEducativas([FromBody] Notificacion notificacion)
        {
            return _notificacionservice.CreateNotification(notificacion);
        }
    }
}
