using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Attributes;

namespace SISDOMI.Entities
{
    [BsonDiscriminator(RootClass = true)]
    [BsonKnownTypes(
       typeof(InformeEducativoInicial),
       typeof(InformeEducativoEvolutivo),
       typeof(InformeSocialInicial))]
    public class Documento
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        [BsonElement("tipo")]
        public string tipo { get; set; }
        [BsonElement("historialcontenido")]
        public List<string> historialcontenido { get; set; }
        [BsonElement("creadordocumento")]
        public string creadordocumento { get; set; }
        [BsonElement("fechacreacion")]
        public DateTime fechacreacion { get; set; }
        [BsonElement("area")]
        public string area { get; set; }
        [BsonElement("fase")]
        public string fase { get; set; }
    }
    public class ContenidoInformeEducativoInicial
    {
        public string situacionacademica { get; set; }
        public string analisisacademico { get; set; }
        public List<string> conclusiones { get; set; }
        public List<string> anexos  { get; set; }
        public List<Firma> firmas { get; set; }        
    }

    public class InformeEducativoInicial : Documento
    {
        public ContenidoInformeEducativoInicial contenido { get; set; } = new ContenidoInformeEducativoInicial();
    }

    public class ContenidoInformeEducativoEvolutivo
    {
        public string antecedentes { get; set; }
        public string situacionactual { get; set; }
        public string logroalcanzado { get; set; }
        public string recomendaciones { get; set; }
        public InstitucionEducativa iereinsercion { get; set; }
        public List<string> anexos { get; set; }
        public List<Firma> firmas { get; set; }
    }
    public class InformeEducativoEvolutivo : Documento
    {
        public ContenidoInformeEducativoEvolutivo contenido { get; set; } = new ContenidoInformeEducativoEvolutivo();
    }
    public class ContenidoInformeSocialInicial
    {
        public string antecedentes { get; set; }
        public string situacionactual { get; set; }
        public string logroalcanzado { get; set; }
        public string recomendaciones { get; set; }
        public List<string> anexos { get; set; }
        public List<Firma> firmas { get; set; }
    }
    public class InformeSocialInicial : Documento
    {
        public ContenidoInformeSocialInicial contenido { get; set; } = new ContenidoInformeSocialInicial();
    }

}
