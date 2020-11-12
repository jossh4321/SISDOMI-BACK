﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SISDOMI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.DTOs
{
    public class DocumentoDTO { 
    
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string tipo { get; set; }
        public List<string> historialcontenido { get; set; }
        public string creadordocumento { get; set; }
        public DateTime fechacreacion { get; set; }
        public string area { get; set; }
        public string fase { get; set; }
        public Object contenido { get; set; }
    }
}