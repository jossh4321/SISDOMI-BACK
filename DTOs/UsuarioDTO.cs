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
        public List<Rol> rolobj { get; set; }
    }
    public class UsuarioDTO_LK: Usuario
    {
        public RolDTO_uw rolobj { get; set; }
        public List<Permisos> permisos { get; set; }
    }
    public class UsuarioDTO_LK_UW : Usuario
    {
        public RolDTO_uw rolobj { get; set; }
        public Permisos permisos { get; set; }
    }

    public class Usuario_Group 
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string usuario { get; set; }
        public string clave { get; set; }
        public Datos datos { get; set; } = new Datos();
        public string estado { get; set; }
        public RolDto2 rol { get; set; }
        public List<Permisos> permisos { get; set; }
    }
    public class RolDto2
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public string area { get; set; }
        public string permisos { get; set; }
    }
    public class UsuarioDTO_UnwindRol: Usuario
    {
        public Rol rolobj { get; set; }
    }
    public class UsuarioDTO_UnwindPermiso : Usuario
    {
        public RolDTO_uw rolobj { get;set;}
    }

    public class UsuarioDTOR
    {
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string usuario { get; set; }
        public string clave { get; set; }
        public Datos datos { get; set; } = new Datos();
        public string estado { get; set; }
        public RolDTO rol { get; set; }
    }

    public class RolDTO
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public string area { get; set; }
        public List<Permisos> permisos { get; set; }
    }

    public class RolDTO_uw
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public string area { get; set; }
        public string permisos { get; set; }
    }
}
