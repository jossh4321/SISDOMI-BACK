
using Microsoft.AspNetCore.Mvc;
using SISDOMI.DTOs;
using SISDOMI.Entities;
using SISDOMI.Helpers;
using SISDOMI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace SISDOMI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InformeController
    {
        private readonly InformeService _informeService;
        private readonly IFileStorage _fileStorage;

        public InformeController(InformeService informeService, IFileStorage fileStorage)
        {
            _informeService = informeService;
            _fileStorage = fileStorage;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<InformeDTO>>> GetAll()
        {
            return await _informeService.GetAllReportsDTO();
        }

        [HttpGet("id")]
        public async Task<ActionResult<DocumentoDTO>> GetById([FromQuery] string id)
        {
            return await _informeService.GetById(id);
        }

        //POST
        [HttpPost("informeei")]
        public async Task<ActionResult<InformeEducativoInicial>> CrearInformeEI(InformeEducativoInicial informe)        
        {
            if (!string.IsNullOrWhiteSpace(informe.historialcontenido[0].url))
            {
                var solicitudBytes2 = Convert.FromBase64String(informe.historialcontenido[0].url);
                informe.historialcontenido[0].url = await _fileStorage.SaveDoc(solicitudBytes2, "pdf", "archivos");
            }
            informe.historialcontenido[0].version = 1;
            informe.historialcontenido[0].fechamodificacion = DateTime.UtcNow.AddHours(-5);
            return await _informeService.RegistrarInformeEI(informe);
        }
        [HttpPost("informeee")]
        public async Task<ActionResult<InformeEducativoEvolutivo>> CrearInformeEE(InformeEducativoEvolutivo informe)
        {
            if (!string.IsNullOrWhiteSpace(informe.historialcontenido[0].url))
            {
                var solicitudBytes2 = Convert.FromBase64String(informe.historialcontenido[0].url);
                informe.historialcontenido[0].url = await _fileStorage.SaveDoc(solicitudBytes2, "pdf", "archivos");
            }
            informe.historialcontenido[0].version = 1;
            informe.historialcontenido[0].fechamodificacion = DateTime.UtcNow.AddHours(-5);
            return await _informeService.RegistrarInformeEE(informe);
        }
        [HttpPost("informesi")]
        public async Task<ActionResult<InformeSocialInicial>> CrearInformeSI(InformeSocialInicial informe)
        {            
            return await _informeService.RegistrarInformeSI(informe);
        }
        [HttpPost("informese")]
        public async Task<ActionResult<InformeSocialEvolutivo>> CrearInformeSE(InformeSocialEvolutivo informe)
        {            
            return await _informeService.RegistrarInformeSE(informe);
        }
        
        [HttpPost("informepi")]
        public async Task<ActionResult<InformePsicologicoInicial>> CrearInformePI(InformePsicologicoInicial informe)
        {            
            return await _informeService.RegistrarInformePI(informe);
        }

        [HttpPost("informepe")]
        public async Task<ActionResult<InformePsicologicoEvolutivo>> CrearInformePE(InformePsicologicoEvolutivo informe)
        {            
            return await _informeService.RegistrarInformePE(informe);
        }

        //PUT
        [HttpPut("informeei")]
        public async Task<ActionResult<InformeEducativoInicial>> ModificarInformeEI(InformeEducativoInicial informe)
        {         
            return await _informeService.ModificarInformeEI(informe);
        }
        [HttpPut("informeee")]
        public async Task<ActionResult<InformeEducativoEvolutivo>> ModificarInformeEE(InformeEducativoEvolutivo informe)
        {
            return await _informeService.ModificarInformeEE(informe);
        }
        [HttpPut("informesi")]
        public async Task<ActionResult<InformeSocialInicial>> ModificarInformeSI(InformeSocialInicial informe)
        {
            return await _informeService.ModificarInformeSI(informe);
        }

        [HttpPut("informese")]
        public async Task<ActionResult<InformeSocialEvolutivo>> ModificarInformeSE(InformeSocialEvolutivo informe)
        {
            return await _informeService.ModificarInformeSE(informe);
        }

        [HttpPut("informepi")]
        public async Task<ActionResult<InformePsicologicoInicial>> ModificarInformePI(InformePsicologicoInicial informe)
        {
            return await _informeService.ModificarInformePI(informe);
        }


        [HttpPut("informepe")]
        public async Task<ActionResult<InformePsicologicoEvolutivo>> ModificarInformePE(InformePsicologicoEvolutivo informe)
        {
            return await _informeService.ModificarInformePE(informe);
        }

        [HttpPost("compdocu")]
        public async Task<ActionResult<Boolean>> ComprobarDocumento(BuscarExpedienteDocumentoDTO documento)
        {
            return await _informeService.ComprobarDocumento(documento);
        }
        [HttpPost("avanceseguimiento")]
        public async Task<ActionResult<AvanceSeguimiento>> CrearAvanceSeguimiento(AvanceSeguimiento docavance)
        {
            return await _informeService.RegistrarAvanceSeguimiento(docavance);
        }
    }
}
