using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Helpers
{
    public interface IDocument
    {
        String CreateCodeDocument(DateTime DateNow, String tipo, Int32 CountDocuments);
    }
}
