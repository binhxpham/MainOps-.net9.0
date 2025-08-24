using MainOps.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace MainOps.Models.ViewModels
{
    public class AddAlarmBoxVM
    {
        [Display(Name = "Box Navn")]
        public string Name { get; set; }
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
        [Display(Name = "Box Beskrivelse")]
        public string Description { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public AddAlarmBoxVM()
        {

        }
    }
}