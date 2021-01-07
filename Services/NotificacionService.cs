using MongoDB.Driver;
using SISDOMI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using SISDOMI.DTOs;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;

namespace SISDOMI.Services
{
    public class NotificacionService
    {
        private readonly IMongoCollection<Notificacion> _notificacion;
        public NotificacionService(ISysdomiDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _notificacion = database.GetCollection<Notificacion>("notificaciones");

        }
        public List<Notificacion> GetAll()
        {
            List<Notificacion> notificacion = new List<Notificacion>();
            notificacion = _notificacion.Find(Notificacion => true).ToList();
            return notificacion;
        }

        public async Task<List<Notificacion>> GetNotificationById(string receptorId)
        {
            var match = new BsonDocument("$match",
              new BsonDocument("idreceptor", receptorId));

            List<Notificacion> notificaciones = new List<Notificacion>();

            notificaciones = await _notificacion.Aggregate()
                                .AppendStage<Notificacion>(match)
                                .ToListAsync();

            return notificaciones;
        }
        public Notificacion CreateNotification(Notificacion notificacion)
        {
            Notificacion not = new Notificacion();
            //not.id = notificacion.id;
            not.titulo = notificacion.titulo;
            not.cuerpo = notificacion.cuerpo;
            not.estado = notificacion.estado;
            not.idemisor = notificacion.idemisor;
            not.idreceptor = notificacion.idreceptor;
            not.fechaemision = notificacion.fechaemision;
            _notificacion.InsertOne(not);
            return not;
        }
    }
}
