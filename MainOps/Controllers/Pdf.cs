using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Controllers
{
    [Route("pdf")]
    public class Pdf : Controller
    {
        [Route("website")]
        public async Task<IActionResult> WebsiteAsync()
        {
            return null;
        }
    }
}
