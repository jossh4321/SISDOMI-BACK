using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SISDOMI.Models;
using SISDOMI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaController: ControllerBase
    {
        private readonly MediaService mediaService;

        public MediaController(MediaService mediaService)
        {
            this.mediaService = mediaService;
        }

        [HttpPost("")]
        public async Task<ActionResult<String>> PostImage(IFormFile file)
        {
            String imageUrl;

            try
            {
                imageUrl = await mediaService.CrearListaFirmas(file);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

            return imageUrl;
        }
        [HttpPut("{*urlfirma}")]
        public async Task<ActionResult<String>> PutImage(string urlfirma,IFormFile file)
        {
            String imageUrl;
            try
            {
                imageUrl = await mediaService.ModificarListaFirmas(file, urlfirma);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

            return imageUrl;
        }
        //talleres
        [HttpPost("talleres")]
        public async Task<ActionResult<String>> PostImageTalleres(IFormFile file)
        {
            String imageUrl;

            try
            {
                imageUrl = await mediaService.CrearFirmasTalleres(file);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

            return imageUrl;
        }
        [HttpPut("talleres/{*urlfirma}")]
        public async Task<ActionResult<String>> PutImageTalleres(string urlfirma, IFormFile file)
        {
            String imageUrl;
            try
            {
                imageUrl = await mediaService.ModificarFirmasTalleres(file, urlfirma);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

            return imageUrl;
        }

        [HttpPost("archivos/pdf")]
        public async Task<ActionResult<String>> PostFilePdf(IFormFile file)
        {
            String imageUrl;
            try
            {
                imageUrl = await mediaService.CrearListaArchivos(file);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

            return imageUrl;
        }


    }

}
