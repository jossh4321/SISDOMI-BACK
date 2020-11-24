using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace SISDOMI.Models
{
    public class PlanState
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public String idDocumento { get; set; }
        public String estado { get; set; }
    }
}
