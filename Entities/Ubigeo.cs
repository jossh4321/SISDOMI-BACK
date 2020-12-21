using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Entities
{
    public class Ubigeo
    {
        public class Departamento
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string id { get; set; }

            [BsonElement("id")]
            public string idDepartamento { get; set; }

            [BsonElement("name")]
            public string nombreDepartamento { get; set; }

        }

        public class Distrito
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string id { get; set; }

            [BsonElement("id")]
            public string idDistrito { get; set; }

            [BsonElement("name")]
            public string nombreDistrito { get; set; }

            [BsonElement("province_id")]
            public string idProvincia { get; set; }
            [BsonElement("department_id")]
            public string idDepartamento { get; set; }
        }

        public class Provincia
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string id { get; set; }

            [BsonElement("id")]
            public string idProvincia { get; set; }

            [BsonElement("name")]
            public string nombreProvincia { get; set; }

            [BsonElement("department_id")]
            public string idDepartamento { get; set; }
        }
    }
}
