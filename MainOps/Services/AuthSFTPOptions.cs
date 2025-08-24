using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Services
{
    public class AuthSFTPOptions
    {
        public string Domain { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Key { get; set; }

    }
}
