using MainOps.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class WaterSampleMeas
    {
        public int Id { get; set; }
        [Display(Name = "Water Sample Type")]
        public int? WaterSampleTypeId { get; set; }
        [Display(Name = "Water Sample Type")]
        public virtual WaterSampleType WaterSampleType { get; set; }
        [Display(Name = "Project")]
        public int? ProjectId { get; set; }
        [Display(Name = "Project")]
        public virtual Project Project { get; set; }
        [Display(Name = "Value")]
        public double value { get; set; }
        [Display(Name = "Method")]
        public string method { get; set; }
        [Display(Name = "Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Dato { get; set; }
        [Display(Name = "Water Sample Place")]
        public int WaterSamplePlaceId { get; set; }
        [Display(Name = "Water Sample Place")]
        public virtual WaterSamplePlace WaterSamplePlace {get; set;}
        public string WaterSampleNumber { get; set; }
        public string SampleTakerName { get; set; }
        public DateTime DateLabReceived { get; set; }
        public DateTime? DateReporting { get; set; }
        public DateTime? DateNextSample { get; set; }
        public int WeekNumber { get
            {
                return StringExtensions.GetIso8601WeekOfYear(this.Dato);
            } 
        }
        }
    public class WaterSampleMeasVM
    {
        public int Id { get; set; }
        [Display(Name = "Water Sample Type")]
        public int? WaterSampleTypeId { get; set; }
        [Display(Name = "Water Sample Type")]
        public virtual WaterSampleType WaterSampleType { get; set; }
        [Display(Name = "Project")]
        public int? ProjectId { get; set; }
        [Display(Name = "Project")]
        public virtual Project Project { get; set; }
        [Display(Name = "Value")]
        public double value { get; set; }
        [Display(Name = "Method")]
        public string method { get; set; }
        [Display(Name = "Sampling Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Dato { get; set; }
        [Display(Name = "Water Sample Place")]
        public int WaterSamplePlaceId { get; set; }
        [Display(Name = "Water Sample Place")]
        public virtual WaterSamplePlace WaterSamplePlace { get; set; }
        public int? holdslimit { get; set; }
        public double? theLimit { get; set; }
        public double? theMeanLimit { get; set; }
        public string WaterSampleNumber { get; set; }
        [Display(Name = "Initials Sample Taker")]
        public string SampleTakerName { get; set; }
        [Display(Name = "Date Received Laboratory")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DateLabReceived { get; set; }
        [Display(Name = "Date Reported")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DateReporting { get; set; }
        [Display(Name = "Date Next Sample")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DateNextSample { get; set; }

        public int WeekNumber { get; set; }

        public WaterSampleMeasVM()
        {

        }
        public WaterSampleMeasVM(WaterSampleMeas m)
        {
            this.Id = m.Id;
            this.method = m.method;
            this.ProjectId = m.ProjectId;
            this.Project = m.Project;
            this.value = m.value;
            this.WaterSamplePlaceId = m.WaterSamplePlaceId;
            this.WaterSamplePlace = m.WaterSamplePlace;
            this.WaterSampleTypeId = m.WaterSampleTypeId;
            this.WaterSampleType = m.WaterSampleType;
            this.WaterSampleNumber = m.WaterSampleNumber;
            this.Dato = m.Dato;
            this.WeekNumber = m.WeekNumber;
            this.SampleTakerName = m.SampleTakerName;
            this.DateLabReceived = m.DateLabReceived;
            this.DateReporting = m.DateReporting;
            this.DateNextSample = m.DateNextSample;
        }
    }
}
