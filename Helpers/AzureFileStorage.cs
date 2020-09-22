using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Helpers
{
    public class AzureFileStorage : IFileStorage
    {
        private readonly string connectionString;
        public AzureFileStorage(ISysdomiDatabaseSettings settings)
        {
            connectionString = settings.StorageConnectionString;
        }
        public async Task DeleteFile(string ruta, string nombreContenedor)
        {
            var account = CloudStorageAccount.Parse(connectionString);
            var clientService = account.CreateCloudBlobClient();
            var contenedor = clientService.GetContainerReference(nombreContenedor);

            var blobName = Path.GetFileName(ruta);
            var blob = contenedor.GetBlobReference(blobName);
            await blob.DeleteIfExistsAsync();
        }

        public async Task<string> EditFile(byte[] contenido, string extension, string nombreContenedor, string rutaArchivo)
        {
            if (!string.IsNullOrWhiteSpace(rutaArchivo))
            {
                await DeleteFile(rutaArchivo, nombreContenedor);
            }
            return await SaveFile(contenido, extension, nombreContenedor);
        }

        public async Task<string> SaveDoc(byte[] contenido, string extension, string nombreContenedor)
        {
            var account = CloudStorageAccount.Parse(connectionString);
            var clientService = account.CreateCloudBlobClient();
            var contenedor = clientService.GetContainerReference(nombreContenedor);
            await contenedor.CreateIfNotExistsAsync();
            await contenedor.SetPermissionsAsync(new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Container
            });
            var filename = $"{Guid.NewGuid()}.{extension}";
            var blob = contenedor.GetBlockBlobReference(filename);
            await blob.UploadFromByteArrayAsync(contenido, 0, contenido.Length);
            blob.Properties.ContentType = "application/pdf";
            await blob.SetPropertiesAsync();
            return blob.Uri.ToString();
        }

        public async Task<string> SaveFile(byte[] contenido,
            string extension, 
            string nombreContenedor)
        {
            var account = CloudStorageAccount.Parse(connectionString);
            var clientService = account.CreateCloudBlobClient();
            var contenedor = clientService.GetContainerReference(nombreContenedor);
            await contenedor.CreateIfNotExistsAsync();
            await contenedor.SetPermissionsAsync(new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });
            var filename = $"{Guid.NewGuid()}.{extension}";
            var blob = contenedor.GetBlockBlobReference(filename);
            await blob.UploadFromByteArrayAsync(contenido, 0, contenido.Length);
            blob.Properties.ContentType = "image/jpg";
            await blob.SetPropertiesAsync();
            return blob.Uri.ToString();
        }
    }
}
