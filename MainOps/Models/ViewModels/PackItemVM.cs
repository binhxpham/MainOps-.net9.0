using MainOps.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace MainOps.Models.ViewModels
{
    public class PackItemVM
    {
        public int TrackItemId { get; set; }
        public virtual TrackItem TrackItem { get; set; }
        [Display(Name = "Is Packed")]
        public bool isPacked { get; set; }
        [Display(Name = "Description")]
        public string LogText { get; set; }
        public DateTime TimeStamp { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}