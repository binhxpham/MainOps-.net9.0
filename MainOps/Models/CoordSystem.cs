using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    [Display(Name = "Coordinate System")]
    public class CoordSystem
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "System")]
        public string system { get; set; }
    }
}
