using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class Grouting
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Project")]
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
        [Display(Name = "SubProject")]
        public int? SubProjectId { get; set; }
        public virtual SubProject SubProject { get; set; }
        [Required]
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }
        [Required]
        [Display(Name = "End Time")]
        public DateTime EndTime { get; set; }
        [Display(Name = "Exclude from Invoice?")]
        public bool Exclude_From_Invoice { get; set; }
        public ICollection<GroutTestDataDevice> Data { get; set; }
        [NotMapped]
        public JsonResult Data_Json { get; set; }
        [Display(Name = "Photo BEFORE")]
        public ICollection<GroutBeforePhoto> Before_Photos { get; set; }
        [Display(Name = "Photo w. Grout finished")]
        public ICollection<GroutGroutPhoto> Grout_Photos { get; set; }
        [Display(Name = "Photo after plugging pipe")]
        public ICollection<GroutAfterPhoto> After_Photos { get; set; }
        [Display(Name = "Photo Water Meter before")]
        public ICollection<GroutWMBeforePhoto> WM_Before_Photos { get; set; }
        [Display(Name = "Photo Water Meter after")]
        public ICollection<GroutWMAfterPhoto> WM_After_Photos { get; set; }

        public double? Meter_Drain {
            get 
            {
                double LitersPerMeter = 5.0;
                return (this.WaterMeterEnd - this.WaterMeterStart) * 1000.0 / LitersPerMeter;
            } 
        }
        public bool HasData { get { 
            if(this.Data != null)
                {
                    if(this.Data.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            } }
        [Display(Name = "Latitude")]
        public double Latitude { get; set; }
        [Display(Name = "Longitude")]
        public double Longitude { get; set; }
        [Display(Name = "Location")]
        public string Location { get; set; }
        [Display(Name = "Total m3 Grout")]
        public double? Totalm3_override { get; set; }
        [Display(Name = "Comments")]
        public string Comments { get; set; }
        public string imagepath { get; set; }
        [Display(Name = "Done By")]
        public string DoneBy { get; set; }
        [Display(Name = "Water meter start")]
        public double? WaterMeterStart { get; set; }
        [Display(Name = "Water meter end")]
        public double? WaterMeterEnd { get; set; }
        public bool ReportChecked { get; set; }
        [Display(Name = "Total m3 Grout")]
        public double? Totalm3 { get
            {
                if(this.WaterMeterEnd == null || this.WaterMeterStart == null)
                {
                    if(this.Data != null)
                    {
                        return this.Data.Sum(x => x.FlowData) / 360.0; //1 value every 10 seconds
                    }
                    else
                    {
                        return null;
                    }
                    
                }
                else if(this.WaterMeterEnd != null && this.WaterMeterStart != null)
                {
                    //return this.Data.Sum(x => x.FlowData) / 360.0; //1 value every 10 seconds
                    return this.WaterMeterEnd - this.WaterMeterStart;
                }
                else { return null; }
            } 
        }

    }
    public class GroutTestDataDevice
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Grouting")]
        public int? GroutingId { get; set; }
        public virtual Grouting Grouting { get; set; }
        public DateTime TimeStamp { get; set; }
        public double? FlowData { get; set; }
    }
}
