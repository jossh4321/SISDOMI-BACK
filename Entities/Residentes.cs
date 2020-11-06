using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SISDOMI.Entities
{
    public class Residentes
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        [BsonElement("nombre")]
        public string nombre { get; set; }
        [BsonElement("apellido")]
        public string apellido { get; set; }
        [BsonElement("tipodocumento")]
        public string tipoDocumento  { get; set; }
        [BsonElement("numerodocumento")]
        public string numeroDocumento { get; set; }
        [BsonElement("lugarnacimiento")]
        public string lugarNacimiento  { get; set; }
        [BsonElement("ubigeo")]
        public string ubigeo { get; set; }
        [BsonElement("juzgadoprocedencia")]
        public string juzgadoProcedencia  { get; set; }
        [BsonElement("fechanacimiento")]
        public DateTime fechaNacimiento { get; set; }
        [BsonElement("sexo")]
        public string sexo { get; set; }
        [BsonElement("telefonosreferencias")]
        public List<Telefono>telefonosReferencia { get; set; }
        [BsonElement("fechaingreso")]
        public DateTime fechaIngreso { get; set; }
        [BsonElement("motivoingreso")]
        public string motivoIngreso { get; set; }
        [BsonElement("progreso")]
        public List<Progreso> progreso { get; set; }
        [BsonElement("estado")]
        public string estado { get; set; }

    }

    public class Telefono
    {
        public string numero { get; set; }
        public string referenteFamiliar { get; set; }
    }
}

