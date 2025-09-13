using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class MeasPoint
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }
        [Required]
        [StringLength(128, MinimumLength = 2)]
        public string? Name { get; set; }
        public int? MonitorTypeId { get; set; }
        public virtual MonitorType? MonitorType { get; set; }
        [Display(Name = "Top of Pipe / Initial value")]
        public double Offset { get; set; }
        [Display(Name = "X-Coordinate")]
        public double Coordx { get; set; }
        [Display(Name = "Y-Coordinate")]
        public double Coordy { get; set; }
        [Display(Name = "Z-Coordinate")]
        public double Coordz { get; set; }
        [ForeignKey("Project")]
        [Display(Name = "Project ID")]
        public int? ProjectId { get; set; }
        public virtual Project? Project { get; set; }
        [Display(Name = "Sub Project")]
        public int? SubProjectId { get; set; }
        public SubProject? SubProject { get; set; }
        [Display(Name = "Measurement Type")]
        public int? MeasTypeId { get; set; }
        [Display(Name = "Measurement Type")]
        public virtual MeasType? MeasType { get; set; }
        [Display(Name = "Labor ID")]
        public int LaborId { get; set; }
        [ForeignKey("Logger")]
        [Display(Name = "Logger ID")]
        public int? LoggerId { get; set; }
        [Display(Name = "Logger")]
        public virtual Logger? logger {get; set;}
        [Display(Name = "Scada Adress")]
        public int? ScadaAddress { get; set; }
        [Display(Name = "Logger Active")]
        public bool LoggerActive { get; set; }
        public ICollection<Document>? Documents { get; set; }
        public double? Lati { get; set; }
        public double? Longi { get; set; }
        [Display(Name = "Lower Limit")]
        public double? Lower_Lower_Limit { get; set; }
        [Display(Name = "Middle Lower Limit")]
        public double? Middle_Lower_Limit { get; set; }
        [Display(Name = "Middle Upper Limit")]
        public double? Middle_Upper_Limit { get; set; }
        [Display(Name = "Upper Limit")]
        public double? Upper_Upper_Limit { get; set; }
        [Display(Name = "Lower Limit Flowrate")]
        public double? Flow_Lower_Lower_Limit { get; set; }
        [Display(Name = "Middle Lower Limit Flowrate")]
        public double? Flow_Middle_Lower_Limit { get; set; }
        [Display(Name = "Middle Upper Limit Flowrate")]
        public double? Flow_Middle_Upper_Limit { get; set; }
        [Display(Name = "Upper Limit Flowrate")]
        public double? Flow_Upper_Upper_Limit { get; set; }
        [Display(Name = "To Be Hidden")]
        public bool ToBeHidden { get; set; }
        [Display(Name = "Description")]
        public string? Description { get; set; }
        [Display(Name = "Bottom Well Pipe")]
        public double? Bottom { get; set; }
        [Display(Name = "DGU nr")]
        public string? DGUnr { get; set; }
        public ICollection<Meas>? Measures { get; set; }
        public ICollection<Offset>? Offsets { get; set; }

        public bool Coordinates_Are_AsBuilt { get; set; }
        public double? ExternalCoordx { get; set; }
        public double? ExternalCoordy { get; set; }
        public double? ExternalCoordz { get; set; }
        public int numSpaces
        {
            get
            {
                return this.Name.Split(" ").Length;
            }
        }
        public string getBaseName
        {
            get
            { 
                if(this.Name != null)
                {
                    if (this.Name.Contains("_"))
                    {
                        if (this.Name.Contains(")"))
                        {
                            return this.Name.Split(")")[1].Split("_")[0];
                        }
                        else
                        {
                            string[] namesplit = this.Name.Split("_");
                            if (namesplit.Length > 2)
                            {
                                string returnedname = "";
                                for(int i = 0; i < namesplit.Length - 1; i++)
                                {
                                    if(i == 0)
                                    {
                                        returnedname += namesplit[i];
                                    }
                                    else
                                    {
                                        returnedname += String.Concat("_", namesplit[i]);
                                    }
                                    
                                }
                                return returnedname;
                            }
                            return namesplit[0];
                        }
                    }
                    else if(this.Name.Contains(" "))
                    {
                        return this.Name.Split(" ")[0];
                    }
                    else
                    {
                        return this.Name;
                    }
                }
                else
                {
                    return "";
                }
            }
        }
        public MeasPoint()
        {
        }
        
    }
}
