using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MainOps.Models.ViewModels;

namespace MainOps.Models
{
    public class ExportDipListsViewModel
    {
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public int? SubProjectId { get; set; }
        public virtual SubProject SubProject { get; set; }
        [Display(Name = "Search station")]
        public string searchfield { get; set; }
        [Display(Name = "Include Pumpinug/Infiltration/Shaft monitoring")]
        public bool wholeSite { get; set; }
        public IList<DipListVM> DipList { get; set; }
        [Display(Name = "Add Flowmeters?")]
        public bool AddFlowMeters { get; set; }
        public ExportDipListsViewModel()
        {
            DipList = new List<DipListVM>();
        }
        
    }
}