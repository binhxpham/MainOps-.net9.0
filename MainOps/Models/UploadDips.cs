using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class UploadDips
    {
        [Display(Name = "MeasPoint")]
        public int MeasPointId { get; set; }
    }
}
