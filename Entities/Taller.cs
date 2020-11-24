﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Entities
{
    public class Taller
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public String id { get; set; }
        [BsonElement("fechacreacion")]
        public DateTime fechacreacion { get; set; }
        [BsonElement("titulo")]
        public String titulo { get; set; }
        [BsonElement("tipo")]
        public String tipo { get; set; }
        [BsonElement("descripcion")]
        public String descripcion { get; set; }
        [BsonElement("creadordocumento")]
        public String creadordocumento { get; set; }
        [BsonElement("area")]
        public String area { get; set; }
        [BsonElement("fase")]
        public String fase { get; set; }
        [BsonElement("contenido")]
        public Object contenido { get; set; }
    }
}
