using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<List<String>>> PostImage(List<IFormFile> files)
        {
            List<String> images;

            try
            {
                images = await mediaService.CrearListaFirmas(files);
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }

            return images;
        }
    }

}
