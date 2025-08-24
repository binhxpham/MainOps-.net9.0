using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class DecommissionableItem
    {
        [Key]
        public int? Id { get; set; }
        [Display(Name = "Installed Item to find")]
        public int? InstalledItemTypeId { get; set; }
        [Display(Name = "Installed Item to find")]
        public virtual ItemType InstallItemType { get; set; }
        [Display(Name = "BoQ Item to find")]
        public int? BoQItemTypeId { get; set; }
        [Display(Name = "BoQ Item to find")]
        public virtual ItemType BoQItemType { get; set; }
    }
}
