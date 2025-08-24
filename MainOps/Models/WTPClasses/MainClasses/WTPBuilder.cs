using MainOps.Models.WTPClasses.HelperClasses;
using MainOps.Models.WTPClasses.MainClasses;
using MainOps.Models.WTPClasses.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MainOps.Models.WTPClasses.MainClasses
{
    public class WTPBuilder
    {
        public IList<WTP_block> WTPblocks { get; set; }
        [Display(Name = "Contaminations")]
        public IList<Contamination> contams { get; set; }
        [Display(Name = "Filtermaterials")]
        public IList<FilterMaterial> filtermaterials { get; set; }
        [Display(Name = "Efforts")]
        public IList<Effort> efforts { get; set; }
        [Display(Name = "Contamination Concentrations")]
        public List<double> contams_concentrations { get; set; }
        [Display(Name = "Contamination Concentrations")]
        public List<double> contams_concentrations_out { get; set; }
        [Display(Name = "Pricelist")]
        public PriceList priceList { get; set; }
        [Display(Name = "Flow")]
        public double flow { get; set; }
        public int longestList { get; set; }
        [Display(Name = "Type of Dewatering")]
        public Water_type typeofdew { get; set; }
        [Display(Name = "Luxurity")]
        public Luxurity wtp_luxury { get; set; }
        [Display(Name = "Pipe Size")]
        public double pipe_size { get; set; }
        [Display(Name = "Pipe Type")]
        public string pipe_type { get; set; }
        [Display(Name = "Overcapacity")]
        public double overcapacity { get; set; }
        [Display(Name = "Duration Days")]
        public int duration_days { get; set; }
        [Display(Name = "Distance")]
        public double distance { get; set; }
        [Display(Name = "New WTP-Block")]
        public string newWTPblock { get; set; }
        public WTPBuilder()
        {
        }

        public WTPBuilder(List<Contamination> conta, double flow, Water_type typeofdew, int durationDays, Luxurity luxurity)
        {
            this.flow = flow;
            contams = conta;
            this.typeofdew = typeofdew;
            wtp_luxury = luxurity;
            duration_days = durationDays;
        }
    }
}
