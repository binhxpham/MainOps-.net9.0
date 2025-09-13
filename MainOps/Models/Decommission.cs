using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class Decommission
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Project")]
        public int ProjectId { get; set; }
        [Display(Name = "Project")]
        public virtual Project? Project { get; set; }
        [Display(Name = "Sub Project")]
        public int? SubProjectId { get; set; }
        [Display(Name = "Sub Project")]
        public virtual SubProject? SubProject { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Report Done By")]
        public string? DoneBy { get; set; }
        [Display(Name = "Signature")]
        public string? Signature { get; set; }
        [Display(Name = "Entered into Database")]
        public DateTime? EnteredIntoDataBase { get; set; }
        [Display(Name = "General Comments")]
        public string? GeneralComments { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        [Display(Name = "Well (Type In)")]
        public string? Wellname { get; set; }
        [Display(Name = "Well (Choice)")]
        public int? MeasPointId { get; set; }
        public virtual MeasPoint? MeasPoint { get; set; }
        [Display(Name = "Photos befor taken?")]
        public bool PhotosBefore { get; set; }
        [Display(Name = "Photos after taken?")]
        public bool PhotosAfter { get; set; }
        [Display(Name = "Dip before")]
        public double? DipBefore { get; set; }
        [Display(Name = "Bottom Dip")]
        public double? BottomDip { get; set; }
        [Display(Name = "Was Airlifted before?")]
        public bool WasAirLiftedFirst { get; set; }
        [Display(Name = "Was sand used?")]
        public bool SandFilling { get; set; }
        [Display(Name = "Kg sand used")]
        public double? Kg_Sand { get; set; }
        [Display(Name = "Was bentonite used?")]
        public bool BentoniteFilling { get; set; }
        [Display(Name = "Kg bentonite used")]
        public double? Kg_Bentonite { get; set; }        
        [Display(Name = "Was the well casted?")]
        public bool Casted { get; set; }
        [Display(Name = "Kg casting material used")]
        public double? Kg_Casting { get; set; }
        [Display(Name = "L Water used")]
        public double? L_Water { get; set; }
        [Display(Name = "Pipe cut X meter under terrain")]
        public double? Pipe_Cut_X_meter_under { get; set; }
        [Display(Name = "Area re-established?")]
        public bool Area_Reestablished { get; set; }
        public ICollection<PhotoFileDecommission>? Photos { get; set; }
        public int? ItemTypeId { get; set; }
        public virtual ItemType? ItemType { get; set; }
        [Display(Name = "Casting Density")]
        public double Density
        {
            get
            {
                if (this.Kg_Casting != null && this.L_Water != null && this.Kg_Casting != 0 && this.L_Water != 0)
                {
                    double m3_Total = (Convert.ToDouble(this.Kg_Casting) / 4.0 + (Convert.ToDouble(this.L_Water)))/1000.0;
                    double Kg_Total = Convert.ToDouble(this.Kg_Casting) + +(Convert.ToDouble(this.L_Water));
                    return Kg_Total/m3_Total;
                }
                else
                {
                    return 0.0;
                }
            }
        }
        [Display(Name ="Casting Resolution")]
        public string Resolution
        {
            get
            {
                if (this.Kg_Casting != null && this.L_Water != null && this.Kg_Casting != 0 && this.L_Water != 0)
                {
                   
                    double resolution1 =  (Convert.ToDouble(this.Kg_Casting) /4.0) / (Convert.ToDouble(this.L_Water));
                    double first = 100.0 - resolution1*100;
                    return String.Format("{0} : {1}", first, resolution1*100);
                }
                else
                {
                    return "";
                }
            }
        }

        public Decommission() { }

    }
}
