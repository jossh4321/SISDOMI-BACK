using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISDOMI
{
    public interface ISysdomiDatabaseSettings
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string StorageConnectionString { get; set; }

    }
    public class SysdomiDatabaseSettings : ISysdomiDatabaseSettings
    {
        public string ConnectionString { get; set ; }
        public string DatabaseName { get; set; }
        public string StorageConnectionString { get; set; }
    }
}
