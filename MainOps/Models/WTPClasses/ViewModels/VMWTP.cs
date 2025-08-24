using MainOps.Models.WTPClasses.MainClasses;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MainOps.Models.WTPClasses.ViewModels
{
    public class VMWTP
    {
        [Range(0.001, 1000)]
        [Display(Name = "Flow rate")]
        [Required]
        public double flow { get; set; }
        [Display(Name = "Contaminations")]
        public List<Contamination> contams { get; set; }
        [Display(Name = "Available Contaminations")]
        public List<SelectListItem> Select_Contas { get; set; }
        [Display(Name = "Active contaminants")]
        public List<SelectListItem> Chosen_Contas { get; set; }
        [Display(Name = "Active contaminants")]
        public List<String> Selected_Contas { get; set; }
        [Display(Name = "Available Contaminations")]
        public List<String> Available_Contas { get; set; }
        [Display(Name = "Contamination Group")]
        public List<SelectListGroup> Contaminations_groups { get; set; }
        [Display(Name = "Type of Dewatering")]
        [Required]
        public string typeofdew { get; set; }
        [Display(Name = "WTP Automation level")]
        [Required]
        public string wtp_luxury { get; set; }
        [Display(Name = "Pipe Size")]
        public double pipe_size { get; set; }
        [Display(Name = "Pipe Type")]
        public string pipe_type { get; set; }
        [Range(0, 10000)]
        [Display(Name = "Rental Days")]
        [Required]
        public int Duration_days { get; set; }
        [Display(Name = "Distance(km)")]
        [Range(0, 500)]
        [Required]
        public double distance { get; set; }
        public List<WTP_block> bbs { get; set; }
        public List<FilterMaterial> ffs { get; set; }
        public List<Contamination> ccs { get; set; }
        public List<double> Selected_Contas_vals { get; set; }
        public List<double> Selected_Contas_out_vals { get; set; }

        public VMWTP()
        {
        }
        public VMWTP(List<Contamination> _conts)
        {
            contams = _conts;
            Selected_Contas = new List<String>();
            Available_Contas = new List<String>();
            Contaminations_groups = new List<SelectListGroup>();
            Select_Contas = new List<SelectListItem>();
            Chosen_Contas = new List<SelectListItem>();
            Selected_Contas_vals = new List<double>();
            Selected_Contas_out_vals = new List<double>();
            bbs = new List<WTP_block>();
            ffs = new List<FilterMaterial>();
            ccs = new List<Contamination>();
            foreach (var c in _conts)
            {
                if (Contaminations_groups.Any(p => p.Name == c.contam_group))
                {
                    Select_Contas.Add(new SelectListItem { Value = c.Name, Text = c.Name, Group = Contaminations_groups.Where(p => p.Name == c.contam_group).First() });
                }
                else
                {
                    Contaminations_groups.Add(new SelectListGroup() { Name = c.contam_group });
                    Select_Contas.Add(new SelectListItem { Value = c.Name, Text = c.Name, Group = Contaminations_groups.Last() });
                }
            }
        }
        public VMWTP(List<WTP_block> bb, List<FilterMaterial> ff, List<Contamination> cc)
        {
            bbs = bb;
            ffs = ff;
            ccs = cc;
        }

    }
}
