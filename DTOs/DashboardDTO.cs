using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.DTOs
{
    public class DashboardDTO
    {
        public List<Object> residentesMesAño { get; set; }
        public List<Object> residentesFase { get; set; }
        public List<Object> documentosPendientes { get; set; }
        public List<Object> documentosAtrazados { get; set; }
        public List<Object> documentosPendientesHoy { get; set; }
    }
}
