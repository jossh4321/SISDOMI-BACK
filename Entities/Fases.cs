using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Entities
{
    public class Fase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }

        [BsonElement("idresidente")]
        public string idresidente { get; set; }

        [BsonElement("progreso")]
        public List<ProgresoFase> progreso { get; set; } = new List<ProgresoFase>();
    }
    public class ProgresoFase
    {
        public ContenidoFase educativa { get; set; } = new ContenidoFase();
        public ContenidoFase social { get; set; } = new ContenidoFase();
        public ContenidoFase psicologica { get; set; } = new ContenidoFase();
        public int fase { get; set; }
        public DocumentacionTransicion documentotransicion { get; set; } = new DocumentacionTransicion();
    }
    public class DocumentacionTransicion
    {
        public DateTime fecha { get; set; }
        public string idcreador { get; set; }
        public string observaciones { get; set; }
        public Firma firma { get; set; } = new Firma();
    }

    public class ContenidoFase
    {
        public List<Documentos> documentos { get; set; } = new List<Documentos>();
        public string estado { get; set; }
    }
    public class Documentos
    {
        public string tipo { get; set; }
        public string estado { get; set; }
        public DateTime fechaestimada { get; set; } = new DateTime();
    }
}
