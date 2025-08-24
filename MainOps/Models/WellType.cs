using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class WellType
    {
        [Key]
        public int Id { get; set; }
        public string Type { get; set; }
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
        [Display(Name = "BoQ Item")]
        public int? ItemTypeId { get; set; }
        [Display(Name = "BoQ Item")]
        public virtual ItemType ItemType { get; set; }
    }
}
