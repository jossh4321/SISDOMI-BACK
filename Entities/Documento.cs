using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Attributes;
using SISDOMI.Services;

namespace SISDOMI.Entities
{
    [BsonDiscriminator(RootClass = true)]
    [BsonKnownTypes(
       typeof(InformeEducativoInicial),
       typeof(InformeEducativoEvolutivo),
       typeof(InformeSocialInicial),
       typeof(InformeSocialEvolutivo),
       typeof(FichaIngresoSocial),
       typeof(FichaIngresoEducativa),
       typeof(FichaIngresoPsicologica),
       typeof(InformePsicologicoInicial),
       typeof(InformePsicologicoEvolutivo),
       typeof(PlanIntervencionIndividualEducativo),
       typeof(PlanIntervencionIndividualPsicologico),
       typeof(PlanIntervencionIndividualSocial),
       typeof(ActaExternamiento),
       typeof(InformeSeguimientoEducativo))]
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
        [BsonElement("idresidente")]
        public string idresidente { get; set; }
        [BsonElement("estado")]
        public string estado { get; set; }
    }
    public class ContenidoInformeEducativoInicial
    {
        public string situacionacademica { get; set; }
        public string analisisacademico { get; set; }
        public List<string> conclusiones { get; set; }
        public List<AnexosDocumento> anexos { get; set; }
        public List<Firma> firmas { get; set; }        
        public string codigodocumento { get; set; }
        public string lugarevaluacion { get; set; }
        public string evaluador { get; set; }
    }

    public class InformeEducativoInicial : Documento
    {
        public ContenidoInformeEducativoInicial contenido { get; set; } = new ContenidoInformeEducativoInicial();
    }

    public class ContenidoInformeEducativoEvolutivo
    {
        public string antecedentes { get; set; }
        public string situacionactual { get; set; }
        public List<string> logroalcanzado { get; set; }
        public List<string> recomendaciones { get; set; }
        public InstitucionEducativa iereinsersion { get; set; }
        public List<AnexosDocumento> anexos { get; set; }
        public List<Firma> firmas { get; set; }
        
        public string codigodocumento { get; set; }
        public string evaluador { get; set; }
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
        public List<string> recomendaciones { get; set; }
        public List<AnexosDocumento> anexos { get; set; }
        public List<Firma> firmas { get; set; }        
        public string codigodocumento { get; set; }
        public string evaluador { get; set; }
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
        public List<string> recomendaciones { get; set; }
        public List<AnexosDocumento> anexos { get; set; }
        public List<Firma> firmas { get; set; }
        public string idresidente { get; set; }
        public string codigodocumento { get; set; }
        public string evaluador { get; set; }
    }
    public class InformeSocialEvolutivo : Documento
    {
        public ContenidoInformeSocialEvolutivo contenido { get; set; } = new ContenidoInformeSocialEvolutivo();
    }
    public class ContenidoInformePsicologicoInicial
    {
        public string antecedentes { get; set; }
        public string conextopsicologico { get; set; }
        public string analisisactual { get; set; }
        public List<string> transtornos { get; set; }
        public List<string> recomendaciones { get; set; }
        public List<string> conclusiones { get; set; }        
        public List<Firma> firmas { get; set; }
        public string codigodocumento { get; set; }
        public string evaluador { get; set; }
    }
    public class InformePsicologicoInicial : Documento
    {
        public ContenidoInformePsicologicoInicial contenido { get; set; } = new ContenidoInformePsicologicoInicial();
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
        public string evaluador { get; set; }
    }
    public class InformePsicologicoEvolutivo : Documento
    {
        public ContenidoInformePsicologicoEvolutivo contenido { get; set; } = new ContenidoInformePsicologicoEvolutivo();
    }

    public class ContenidoPlanIntervencionIndividualEducativo
    {
        [BsonElement("car")]
        public String car { get; set; }
        [BsonElement("trimestre")]
        public int trimestre { get; set; }
        [BsonElement("edad")]
        public int edad { get; set; }
        [BsonElement("objetivogeneral")]
        public String objetivoGeneral { get; set; }
        [BsonElement("objetivoespecificos")]
        public List<String> objetivoEspecificos { get; set; }
        [BsonElement("aspectosintervencion")]
        public List<String> aspectosIntervencion { get; set; }
        [BsonElement("estrategias")]
        public List<String> estrategias { get; set; }
        [BsonElement("indicadores")]
        public List<String> indicadores { get; set; }
        [BsonElement("metas")]
        public List<String> metas { get; set; }
        [BsonElement("firmas")]
        public List<Firma> firmas { get; set; }
        [BsonElement("titulo")]
        public String titulo { get; set; }
        [BsonElement("codigodocumento")]
        public String codigoDocumento { get; set; }
    }
    public class ContenidoFichaIngresoEducativo {
        public Procedencia ieprocedencia { get; set; } = new Procedencia();
        public String responsableturno { get; set; }
        public List<String> observaciones { get; set; } = new List<String>();
        public String codigodocumento { get; set; }
        public List<Firma> firmas { get; set; } = new List<Firma>();
        

    }
    public class ContenidoFichaIngresoSocial
    {
        public FamiliarIngreso familiar { get; set; } = new FamiliarIngreso();
        public Vivienda vivienda { get; set; } = new Vivienda();
        public Economico economico { get; set; } = new Economico();
        public String salud { get; set; }
        public Legal legal { get; set; } = new Legal();
        public String diagnosticosocial { get; set; }
        public String planintervencion { get; set; }
        public String codigodocumento { get; set; }
        public List<Firma> firmas { get; set; } = new List<Firma>();
    }
    public class ContenidoFichaIngresoPsicologica
    {
        public List<Firma> firmas { get; set; } = new List<Firma>();
        public String responsableturno { get; set; }
        public List<PadresFichaIngreso> padres { get; set; } = new List<PadresFichaIngreso>();
        public List<HermanosFichaIngreso> hermanos { get; set; } = new List<HermanosFichaIngreso>();
        public Escolaridad  escolaridad { get; set; } = new Escolaridad ();
        public Discapacidad discapacidad { get; set; } = new Discapacidad();
        public List<Maltrato> maltrato { get; set; } = new List<Maltrato>();
        public AbusoSexual abusosexual   { get; set; } = new AbusoSexual ();
        public Adiccion adicciones { get; set; } = new Adiccion();
        public List<conductaRiesgo> conductasriesgo  { get; set; } = new List<conductaRiesgo>();
        public List<ConductaEmocional> conductasemocionales { get; set; } = new List<ConductaEmocional>();
        public DesarrolloSexual  desarrollosexual  { get; set; } = new DesarrolloSexual ();
        public ExplotacionSexual  explotacionsexual { get; set; } = new ExplotacionSexual();
        public Actividades actividades  { get; set; } = new Actividades();
        public String codigodocumento { get; set; }
        public String observaciones { get; set; }


    }
    public class PlanIntervencionIndividualEducativo : Documento
    {
        public ContenidoPlanIntervencionIndividualEducativo contenido { get; set; } = new ContenidoPlanIntervencionIndividualEducativo();
    }
    public class FichaIngresoEducativa : Documento
    {
        public ContenidoFichaIngresoEducativo contenido { get; set; } = new ContenidoFichaIngresoEducativo();
    }
    public class FichaIngresoSocial : Documento
    {
        public ContenidoFichaIngresoSocial contenido { get; set; } = new ContenidoFichaIngresoSocial();
    }
    public class FichaIngresoPsicologica : Documento
    {
        public ContenidoFichaIngresoPsicologica contenido { get; set; } = new ContenidoFichaIngresoPsicologica();
    }

    public class ContenidoPlanIntervencionPsicologica
    {
        [BsonElement("descripcion")]
        public String descripcion { get; set; }
        [BsonElement("objetivoespecificos")]
        public List<String> objetivoEspecificos { get; set; }
        [BsonElement("tecnicas")]
        public List<String> tecnicas { get; set; }
        [BsonElement("metas")]
        public List<String> metas { get; set; }
        [BsonElement("indicadores")]
        public List<String> indicadores { get; set; }
        [BsonElement("frecuenciasesion")]
        public String frecuenciaSesion { get; set; }
        [BsonElement("numerosesion")]
        public Int32 numeroSesion { get; set; }
        [BsonElement("requerimientos")]
        public List<String> requerimientos { get; set; }
        [BsonElement("codigodocumento")]
        public String codigoDocumento { get; set; }
        [BsonElement("titulo")]
        public String titulo { get; set; }
        [BsonElement("firmas")]
        public List<Firma> firmas { get; set; }

    }

    public class PlanIntervencionIndividualPsicologico : Documento
    {
        public ContenidoPlanIntervencionPsicologica contenido { get; set; } = new ContenidoPlanIntervencionPsicologica();
    }

    public class ContenidoPlanIntervencionSocial
    {//
        [BsonElement("edad")]
        public int edad { get; set; }
        [BsonElement("diagnostico")]
        public String diagnostico { get; set; }
        [BsonElement("objetivos")]
        public List<String> objetivos { get; set; }
        [BsonElement("avances")]
        public List<String> avances { get; set; }
        [BsonElement("estrategias")]
        public List<String> estrategias { get; set; }
        [BsonElement("indicadores")]
        public List<String> indicadores { get; set; }
        [BsonElement("metas")]
        public List<String> metas { get; set; }
        [BsonElement("firmas")]
        public List<Firma> firmas { get; set; }
        [BsonElement("codigodocumento")]
        public String codigoDocumento { get; set; }
        [BsonElement("titulo")]
        public String titulo { get; set; }
    }

    public class PlanIntervencionIndividualSocial: Documento
    {
        public ContenidoPlanIntervencionSocial contenido { get; set; } = new ContenidoPlanIntervencionSocial();
    }

    public class AnexosDocumento
    {
        public string url { get; set; }
        public string titulo { get; set; }
    }

    //Contenido Seguimiento Educativo
    public class ContenidoInformeSeguimientoEducativo
    {

        [BsonElement("modalidad")]
        public String modalidad { get; set; }
        [BsonElement("nivel")]
        public String nivel { get; set; }
        [BsonElement("grado")]
        public String grado { get; set; }
        [BsonElement("añoescolar")]
        public String añoEscolar { get; set;}
        [BsonElement("trimestre")]
        public List<Trimestre> trimestre { get; set; }
        [BsonElement("firmas")]
        public List<Firmas> firmas { get; set; }
        [BsonElement("codigodocumento")]
        public String codigoDocumento { get; set; }
          
    }
    public class Trimestre
    {
        public String orden { get; set; }
        public List<Puntajes> puntajes { get; set; }
        public String analisiseducativo { get; set; }
        public String recomendaciones { get; set; }

    }
    public class Puntajes
    {
        public String area { get; set; }
        public String promedio { get; set; }
    }
    public class Firmas
    {
        public String urlfirma { get; set; }
        public String nombre { get; set; }
        public String cargo { get; set; }
    }
    public class InformeSeguimientoEducativo :Documento
    {
        public ContenidoInformeSeguimientoEducativo contenido { get; set; } = new ContenidoInformeSeguimientoEducativo();
    }
    ///FIN SEGUIMIENTO EDUCATIVO

    ///Acta De Externamiento
    public class ContenidoActaDeExternamiento
    {

        [BsonElement("responsable")]
        public String responsable { get; set; }
        [BsonElement("entidaddisposicion")]
        public String entidaddisposicion { get; set; }
        [BsonElement("numeroresolucion")]
        public String numeroresolucion { get; set; }
        [BsonElement("numerooficio")]
        public String numerooficio { get; set; }
        [BsonElement("observaciones")]
        public String observaciones { get; set; }
        [BsonElement("firmas")]
        public List<Firmas> firmas { get; set; }

    }

    public class ActaExternamiento : Documento
    {
        public ContenidoActaDeExternamiento contenido { get; set; } = new ContenidoActaDeExternamiento();
    }
    /// >:(

}
