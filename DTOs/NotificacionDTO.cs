using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using SISDOMI.Entities;

namespace SISDOMI.DTOs
{
    public class NotificacionDTO
    {
        public List<Notificacion> listaNotificaciones {get;set;}
        public int contadorNotificaciones { get; set; }
    }
}
