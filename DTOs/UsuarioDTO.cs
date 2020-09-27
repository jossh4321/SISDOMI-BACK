using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SISDOMI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.DTOs
{
    public class UsuarioDTO : Usuario
    {
        public Rol rolobj { get; set; }
    }

    public class UsuarioDTOR
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string usuario { get; set; }
        public string clave { get; set; }
        public Datos datos { get; set; } = new Datos();
        public string estado { get; set; }
        public Rol rol { get; set; }
    }
}
