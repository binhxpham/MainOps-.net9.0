using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class Title
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Title")]
        public string TheTitle { get; set; }
        [Display(Name = "Linked BoQ")]
        public int? ItemTypeId { get; set; }
        public virtual ItemType ItemType { get; set; }
        [Display(Name = "Project")]
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public bool Worker { get; set; }
        public Title()
        {

        }
        public Title(Title tit,int projid,int itemtypeid)
        {
            this.ProjectId = projid;
            this.TheTitle = tit.TheTitle;
            this.ItemTypeId = itemtypeid;
            this.Worker = tit.Worker;
        }

    }
}
