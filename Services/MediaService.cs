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

        public async Task<List<String>> CrearListaFirmas(List<IFormFile> mediaInfos)
        {
            List<String> imageUrls = new List<string>();

            foreach (var medio in mediaInfos)
            {
                using (var stream = new MemoryStream())
                {
                    await medio.CopyToAsync(stream);

                    String urlImage = await fileStorage.SaveFile(stream.ToArray(), "jpg", "planes");

                    imageUrls.Add(urlImage);
                }
            }

            return imageUrls;
        } 
    }
}
