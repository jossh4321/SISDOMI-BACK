using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SISDOMI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.DTOs
{
    public class IncidenciaDTO
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public Usuario autor { get; set; }
        public DateTime fechaRegistro { get; set; }
        public DateTime fecha { get; set; }
        public string titulo { get; set; }
        public string descripcion { get; set; }
        public List<string> observaciones { get; set; } = new List<string>();
        public List<string> incidencias { get; set; } = new List<string>();
        public List<Residentes> residentes { get; set; } = new List<Residentes>();
        public Firma firma { get; set; }
    }
}
