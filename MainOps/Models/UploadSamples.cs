using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class UploadSamples
    {
        [Display(Name = "Project")]
        public int ProjectId { get; set; }
        public int WaterSamplePlaceId { get; set; }
        public DateTime Dato { get; set; }
        public DateTime? NextSample { get; set; }
    }
}
