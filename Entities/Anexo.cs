using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SISDOMI.Entities
{
    //Comentario para probar    
    //Comentario para probar  x2
    //Comentario para probar x3
	//Comentario solo en la rama de desarrollo
    public class Anexo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        [BsonElement("titulo")]
        public string titulo { get; set; }
        [BsonElement("descripcion")]
        public string descripcion { get; set; }
        [BsonElement("idresidente")]
        public string idresidente { get; set; }
        [BsonElement("idcreador")]
        public string idcreador { get; set; }
        [BsonElement("fechacreacion")]
        public DateTime? fechacreacion { get; set; }
        [BsonElement("area")]
        public string area { get; set; }
        [BsonElement("enlaces")]
        public List<Enlace> enlaces { get; set; }

    }

    public class Enlace
    {
        public string link { get; set; }
        public string descripcion { get; set; }
    }
}
