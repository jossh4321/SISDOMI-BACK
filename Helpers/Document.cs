using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI.Helpers
{
    public class Document : IDocument
    {
        public string CreateCodeDocument(DateTime DateNow, string tipo, Int32 CountDocuments)
        {
            String UnixDate = Convert.ToInt64(DateNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString();

            String formatZeros = "000";
            String CountDocument = CountDocuments.ToString(formatZeros);

            return tipo + "-" + UnixDate + "-" + CountDocument;
        }
    }
}
