using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MainOps.Models.WTPClasses.ViewModels
{
    public class VirtualConc

    {
        [Display(Name = "Inbound Concentration")]
        public List<double> concentrations { get; set; }
        [Display(Name = "Outbound Concentration")]
        public List<double> concentrations_out { get; set; }
        [Display(Name = "Contamination")]
        public List<string> conts { get; set; }
        public Dictionary<string, double> cont_conc { get; set; }
        public Dictionary<string, double> cont_conc_out { get; set; }
        public VirtualConc(List<string> chosenCon)
        {
            cont_conc = new Dictionary<string, double>();
            cont_conc_out = new Dictionary<string, double>();
            concentrations = new List<double>();
            concentrations_out = new List<double>();
            conts = chosenCon;
            foreach (var s in conts)
            {
                concentrations.Add(0.0);
                concentrations_out.Add(0.0);
                cont_conc.Add(s, 0);
                cont_conc_out.Add(s, 0);
            }
        }
        public VirtualConc()
        {
            cont_conc = new Dictionary<string, double>();
            cont_conc_out = new Dictionary<string, double>();
            conts = new List<string>();
            concentrations = new List<double>();
            concentrations_out = new List<double>();
        }
    }
}
