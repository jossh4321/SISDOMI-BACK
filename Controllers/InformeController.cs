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
            foreach(var item in informe.contenido.firmas)
            {
                if (!string.IsNullOrWhiteSpace(item.urlfirma))
                {
                    var imgfirma = Convert.FromBase64String(item.urlfirma);
                    item.urlfirma = await _fileStorage.SaveFile(imgfirma, "png", "informes");
                }
            }            
            return await _informeService.RegistrarInformeEI(informe);
        }
        [HttpPost("informeee")]
        public async Task<ActionResult<InformeEducativoEvolutivo>> CrearInformeEE(InformeEducativoEvolutivo informe)
        {
            foreach (var item in informe.contenido.firmas)
            {
                if (!string.IsNullOrWhiteSpace(item.urlfirma))
                {
                    var imgfirma = Convert.FromBase64String(item.urlfirma);
                    item.urlfirma = await _fileStorage.SaveFile(imgfirma, "png", "informes");
                }
            }
            return await _informeService.RegistrarInformeEE(informe);
        }
        [HttpPost("informesi")]
        public async Task<ActionResult<InformeSocialInicial>> CrearInformeSI(InformeSocialInicial informe)
        {
            foreach (var item in informe.contenido.firmas)
            {
                if (!string.IsNullOrWhiteSpace(item.urlfirma))
                {
                    var imgfirma = Convert.FromBase64String(item.urlfirma);
                    item.urlfirma = await _fileStorage.SaveFile(imgfirma, "png", "informes");
                }
            }
            return await _informeService.RegistrarInformeSI(informe);
        }
        [HttpPost("informese")]
        public async Task<ActionResult<InformeSocialEvolutivo>> CrearInformeSE(InformeSocialEvolutivo informe)
        {
            foreach (var item in informe.contenido.firmas)
            {
                if (!string.IsNullOrWhiteSpace(item.urlfirma))
                {
                    var imgfirma = Convert.FromBase64String(item.urlfirma);
                    item.urlfirma = await _fileStorage.SaveFile(imgfirma, "png", "informes");
                }
            }
            return await _informeService.RegistrarInformeSE(informe);
        }
        //falta el psicologico inicial 0-0 -> ya no uwu
        [HttpPost("informepi")]
        public async Task<ActionResult<InformePsicologicoInicial>> CrearInformePI(InformePsicologicoInicial informe)
        {
            foreach (var item in informe.contenido.firmas)
            {
                if (!string.IsNullOrWhiteSpace(item.urlfirma))
                {
                    var imgfirma = Convert.FromBase64String(item.urlfirma);
                    item.urlfirma = await _fileStorage.SaveFile(imgfirma, "png", "informes");
                }
            }
            return await _informeService.RegistrarInformePI(informe);
        }

        [HttpPost("informepe")]
        public async Task<ActionResult<InformePsicologicoEvolutivo>> CrearInformePE(InformePsicologicoEvolutivo informe)
        {
            foreach (var item in informe.contenido.firmas)
            {
                if (!string.IsNullOrWhiteSpace(item.urlfirma))
                {
                    var imgfirma = Convert.FromBase64String(item.urlfirma);
                    item.urlfirma = await _fileStorage.SaveFile(imgfirma, "png", "informes");
                }
            }
            return await _informeService.RegistrarInformePE(informe);
        }

        //PUT
        [HttpPut("informeei")]
        public async Task<ActionResult<InformeEducativoInicial>> ModificarInformeEI(InformeEducativoInicial informe)
        {
            foreach (var item in informe.contenido.firmas)
            {
                if (!string.IsNullOrWhiteSpace(item.urlfirma) && !item.urlfirma.Contains("http"))
                {
                    var imgfirma = Convert.FromBase64String(item.urlfirma);
                    item.urlfirma = await _fileStorage.SaveFile(imgfirma, "png", "informes");
                }
            }
            return await _informeService.ModificarInformeEI(informe);
        }
        [HttpPut("informeee")]
        public async Task<ActionResult<InformeEducativoEvolutivo>> ModificarInformeEE(InformeEducativoEvolutivo informe)
        {
            foreach (var item in informe.contenido.firmas)
            {
                if (!string.IsNullOrWhiteSpace(item.urlfirma) && !item.urlfirma.Contains("http"))
                {
                    var imgfirma = Convert.FromBase64String(item.urlfirma);
                    item.urlfirma = await _fileStorage.SaveFile(imgfirma, "png", "informes");
                }
            }
            return await _informeService.ModificarInformeEE(informe);
        }
        [HttpPut("informesi")]
        public async Task<ActionResult<InformeSocialInicial>> ModificarInformeSI(InformeSocialInicial informe)
        {
            foreach (var item in informe.contenido.firmas)
            {
                if (!string.IsNullOrWhiteSpace(item.urlfirma) && !item.urlfirma.Contains("http"))
                {
                    var imgfirma = Convert.FromBase64String(item.urlfirma);
                    item.urlfirma = await _fileStorage.SaveFile(imgfirma, "png", "informes");
                }
            }

            return await _informeService.ModificarInformeSI(informe);
        }

        [HttpPut("informese")]
        public async Task<ActionResult<InformeSocialEvolutivo>> ModificarInformeSE(InformeSocialEvolutivo informe)
        {
            foreach (var item in informe.contenido.firmas)
            {
                if (!string.IsNullOrWhiteSpace(item.urlfirma) && !item.urlfirma.Contains("http"))
                {
                    var imgfirma = Convert.FromBase64String(item.urlfirma);
                    item.urlfirma = await _fileStorage.SaveFile(imgfirma, "png", "informes");
                }
            }
            return await _informeService.ModificarInformeSE(informe);
        }

        [HttpPut("informepi")]
        public async Task<ActionResult<InformePsicologicoInicial>> ModificarInformePI(InformePsicologicoInicial informe)
        {
            foreach (var item in informe.contenido.firmas)
            {
                if (!string.IsNullOrWhiteSpace(item.urlfirma) && !item.urlfirma.Contains("http"))
                {
                    var imgfirma = Convert.FromBase64String(item.urlfirma);
                    item.urlfirma = await _fileStorage.SaveFile(imgfirma, "png", "informes");
                }
            }

            return await _informeService.ModificarInformePI(informe);
        }


        [HttpPut("informepe")]
        public async Task<ActionResult<InformePsicologicoEvolutivo>> ModificarInformePE(InformePsicologicoEvolutivo informe)
        {
            foreach (var item in informe.contenido.firmas)
            {
                if (!string.IsNullOrWhiteSpace(item.urlfirma) && !item.urlfirma.Contains("http"))
                {
                    var imgfirma = Convert.FromBase64String(item.urlfirma);
                    item.urlfirma = await _fileStorage.SaveFile(imgfirma, "png", "informes");
                }
            }
            return await _informeService.ModificarInformePE(informe);
        }

        [HttpPost("compdocu")]
        public async Task<ActionResult<Boolean>> ComprobarDocumento(BuscarExpedienteDocumentoDTO documento)
        {
            return await _informeService.ComprobarDocumento(documento);
        }
    }
}
