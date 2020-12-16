using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Models
{
    public class MediaBody
    {
        public string urlfirma { get; set; }
        public FormFileWrapper file { get; set; }
    }
    public class FormFileWrapper
    {
        public IFormFile File { get; set; }
    }
}
