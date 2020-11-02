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
       typeof(InformeSocialInicial),
       typeof(InformeSocialEvolutivo),
       typeof(InformePsicologicoEvolutivo),
       typeof(PlanIntervencionIndividual))]
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
        public List<string> anexos { get; set; }
        public List<Firma> firmas { get; set; }
        public string idresidente { get; set; }
        public string codigodocumento { get; set; }
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
        public InstitucionEducativa iereinsersion { get; set; }
        public List<string> anexos { get; set; }
        public List<Firma> firmas { get; set; }
        public string idresidente { get; set; }
        public string codigodocumento { get; set; }
    }
    public class InformeEducativoEvolutivo : Documento
    {
        public ContenidoInformeEducativoEvolutivo contenido { get; set; } = new ContenidoInformeEducativoEvolutivo();
    }
    public class ContenidoInformeSocialInicial
    {
        public string antecedentes { get; set; }
        public List<Familiar> familiares { get; set; }
        public string situacionfamiliar { get; set; }
        public string situacionvivienda { get; set; }
        public string situacioneconomica { get; set; }
        public string situacionsalud { get; set; }
        public string educacion { get; set; }
        public string situacionactual { get; set; }
        public string diagnosticosocial { get; set; }
        public string recomendaciones { get; set; }
        public List<string> anexos { get; set; }
        public List<Firma> firmas { get; set; }
        public string idresidente { get; set; }
        public string codigodocumento { get; set; }
    }
    public class InformeSocialInicial : Documento
    {
        public ContenidoInformeSocialInicial contenido { get; set; } = new ContenidoInformeSocialInicial();
    }
    public class ContenidoInformeSocialEvolutivo
    {
        public string antecedentes { get; set; }
        public string situacionactual { get; set; }
        public string diagnosticosocial { get; set; }
        public string recomendaciones { get; set; }
        public List<AnexosDocumento> anexos { get; set; }
        public List<Firma> firmas { get; set; }
        public string idresidente { get; set; }
        public string codigodocumento { get; set; }
    }
    public class InformeSocialEvolutivo : Documento
    {
        public ContenidoInformeSocialEvolutivo contenido { get; set; } = new ContenidoInformeSocialEvolutivo();
    }
    public class ContenidoInformePsicologicoEvolutivo
    {
        public string motivoingreso { get; set; }
        public string observacionesgenerales { get; set; }
        public List<string> pruebasaplicadas { get; set; }
        public string interpretacionresultados { get; set; }
        public List<string> conclusiones { get; set; }
        public string diagnostico { get; set; }
        public List<string> recomendaciones { get; set; }
        public List<Firma> firmas { get; set; }
        public string idresidente { get; set; }
        public string codigodocumento { get; set; }
    }
    public class InformePsicologicoEvolutivo : Documento
    {
        public ContenidoInformePsicologicoEvolutivo contenido { get; set; } = new ContenidoInformePsicologicoEvolutivo();
    }

    public class ContenidoPlanIntervencionIndividual
    {
        public String car { get; set; }
        public String trimestre { get; set; }
        public String edad { get; set; }
        public String objetivogeneral { get; set; }
        public List<String> objetivoespecificos { get; set; }
        public List<String> aspectosintervencion { get; set; }
        public List<String> estrategias { get; set; }
        public List<String> indicadores { get; set; }
        public List<String> metas { get; set; }
        public List<Firma> firmas { get; set; }
        public String titulo { get; set; }
    }

    public class PlanIntervencionIndividual : Documento
    {
        public ContenidoPlanIntervencionIndividual contenido { get; set; } = new ContenidoPlanIntervencionIndividual();
    }
    public class AnexosDocumento
    {
        public string idanexo { get; set; }
        public string tipo { get; set; }
    }
}
