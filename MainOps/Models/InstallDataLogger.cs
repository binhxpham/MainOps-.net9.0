using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class DataLoggerInstall
    {
        [Key]
        public int Id { get; set;}
        [ForeignKey("Project")]
        [Display(Name = "Project")]
        public int ProjectId { get; set; }
        [Display(Name = "Project")]
        public virtual Project? Project { get; set; }
        [ForeignKey("SubProject")]
        [Display(Name = "SubProject")]
        public int? SubProjectId { get; set; }
        [Display(Name = "SubProject")]
        public virtual SubProject? SubProject { get; set; }
        [Display(Name = "Date and Time")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Data Logger No.")]
        public int DataLoggerNumber { get; set; }
        [ForeignKey("MeasPoint")]
        [Display(Name = "Measurement Point")]
        public int? MeasPointId { get; set; }
        [Display(Name = "Measurement Point")]
        public virtual MeasPoint? MeasPoint { get; set; }
        [Display(Name ="Well Name")]
        public string? WellName { get; set; }
        [Display(Name = "Hand Dip pipe 1")]
        public double? Dip_Pipe_1 { get; set; }
        [Display(Name = "Difference to top of pipe 1")]
        public double? Diff_GL_Top_Pipe_1 { get; set; }
        [Display(Name = "Raw Value SCADA pipe 1")]
        public double? Raw_Value_Pipe_1 { get; set; }
        [Display(Name = "Depth sensor pipe 1")]
        public double? Depth_Sensor_1 { get; set; }
        [Display(Name = "Type Sensor pipe 1")]
        public string? Type_Sensor_1 { get; set; }
        [Display(Name = "Hand Dip pipe 2")]
        public double? Dip_Pipe_2 { get; set; }
        [Display(Name = "Difference to top of pipe 2")]
        public double? Diff_GL_Top_Pipe_2 { get; set; }
        [Display(Name = "Raw Value SCADA pipe 2")]
        public double? Raw_Value_Pipe_2 { get; set; }
        [Display(Name = "Depth sensor pipe 2")]
        public double? Depth_Sensor_2 { get; set; }
        [Display(Name = "Type Sensor pipe 2")]
        public string? Type_Sensor_2 { get; set; }
        [Display(Name = "Installation Comments")]
        public string? Comments { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        [Display(Name = "Done by")]
        public string? DoneBy { get; set; }
        public string? Signature { get; set; }
        public ICollection<PhotoFileDataLoggerInstall>? Photos { get; set; }
    }
}
