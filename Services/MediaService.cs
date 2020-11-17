using Microsoft.AspNetCore.Http;
using SISDOMI.Entities;
using SISDOMI.Helpers;
using SISDOMI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Services
{

    public class MediaService
    {
        private readonly IFileStorage fileStorage;

        public MediaService(IFileStorage fileStorage)
        {
            this.fileStorage = fileStorage;
        }

        public async Task<String> CrearListaFirmas(IFormFile mediaInfo)
        {
            String urlImage = "";

            using (var stream = new MemoryStream())
            {
                await mediaInfo.CopyToAsync(stream);

                urlImage = await fileStorage.SaveFile(stream.ToArray(), "jpg", "planes");

            }

            return urlImage;
        }

        public async Task<String> CrearListaArchivos(IFormFile mediaInfo)
        {
            String urlImage = "";

            using (var stream = new MemoryStream())
            {
                await mediaInfo.CopyToAsync(stream);

                urlImage = await fileStorage.SaveDoc(stream.ToArray(), "pdf", "archivos");

            }

            return urlImage;
        }
    }
}
