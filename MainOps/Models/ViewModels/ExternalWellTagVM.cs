using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ViewModels
{
    public class ExternalWellTagVM
    {
        [Display(Name = "Project")]
        public int ProjectId { get; set; }
        [Display(Name = "SubProject")]
        public int? SubProjectId { get; set; }
        [Display(Name = "Filter Text")]
        public string searchtext { get; set; }
        [Display(Name = "Text for well tags")]
        public string TextForTag { get; set; }
        public IFormFile logo { get; set; }
        public List<MeasPoint> measpoints { get; set; }
        public string photopath { get; set; }
        public bool OnlyAsBuilt { get; set; }
        public bool PrintSecondaries { get; set; }
        public ExternalWellTagVM()
        {
            measpoints = new List<MeasPoint>();
        }
    }
}
