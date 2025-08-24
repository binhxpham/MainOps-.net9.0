using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Resources
{
    public class MyAppSecretConfig
    {
        public string emailUser { get; set; }
        public string emailPass { get; set; }
        public string myConnectionString { get; set; }
        public string emailAdmin { get; set; }
    }
}
