using Microsoft.AspNetCore.Mvc;
using SISDOMI.DTOs;
using SISDOMI.Entities;
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

        public InformeController(InformeService informeService)
        {
            _informeService = informeService;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<InformeDTO>>> GetAll()
        {
            return await _informeService.GetAllReportsDTO();
        }

        [HttpGet("id")]
        public async Task<ActionResult<DocumentoDTO>> GetById(string id)
        {
            return await _informeService.GetById(id);
        }

        //POST
        [HttpPost("informeei")]
        public async Task<ActionResult<InformeEducativoInicial>> CrearInformeEI(InformeEducativoInicial informe)
        {
            return await _informeService.RegistrarInformeEI(informe);
        }
        [HttpPost("informeee")]
        public async Task<ActionResult<InformeEducativoEvolutivo>> CrearInformeEE(InformeEducativoEvolutivo informe)
        {
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
        //falta el psicologico inicial 0-0
        [HttpPost("informepe")]
        public async Task<ActionResult<InformePsicologicoEvolutivo>> CrearInformePE(InformePsicologicoEvolutivo informe)
        {
            return await _informeService.RegistrarInformePE(informe);
        }

        //PUT
        [HttpPut("informeei")]
        public ActionResult<InformeEducativoInicial> ModificarInformeEI(InformeEducativoInicial informe)
        {
            return _informeService.ModificarInformeEI(informe);
        }
        [HttpPut("informeee")]
        public ActionResult<InformeEducativoEvolutivo> ModificarInformeEE(InformeEducativoEvolutivo informe)
        {
            return _informeService.ModificarInformeEE(informe);
        }
        [HttpPut("informesi")]
        public ActionResult<InformeSocialInicial> ModificarInformeSI(InformeSocialInicial informe)
        {
            return _informeService.ModificarInformeSI(informe);
        }
        [HttpPut("informese")]
        public ActionResult<InformeSocialEvolutivo> ModificarInformeSE(InformeSocialEvolutivo informe)
        {
            return _informeService.ModificarInformeSE(informe);
        }
        [HttpPut("informepe")]
        public ActionResult<InformePsicologicoEvolutivo> ModificarInformePE(InformePsicologicoEvolutivo informe)
        {
            return _informeService.ModificarInformePE(informe);
        }
    }
}
