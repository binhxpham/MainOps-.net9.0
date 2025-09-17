using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class MeasMoreVM
    {
        [Display(Name = "Level Measurement")]
        public double? TheMeasurement { get; set; }
        [Display(Name = "Flow Measurement")]
        public double? TheFlowMeasurement { get; set; }
        [Display(Name = "Water Meter Measurement")]
        public double? TheWMMeasurement { get; set; }
        [ForeignKey("Comment")]
        [Display(Name = "The Comment")]
        public int? CommentId { get; set; }

        [Display(Name = "General Comment")]
        public virtual Comment? TheComment { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime When { get; set; }
        [Display(Name = "Measurement Point")]
        public string? BaseName { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        [Display(Name = "Done By")]
        public string? DoneBy { get; set; }
        public int ProjectId { get; set; }

        [Display(Name = "Project")]
        public virtual Project? Project { get; set; }

        [Display(Name = "Measurement Point")]
        public virtual MeasPoint? MeasPoint { get; set; }
        public List<int> MeasPointLevelIds { get; set; }
        public List<int> MeasPointFlowIds { get; set; }
        public List<int> MeasPointWMIds { get; set; }
        public List<string> MeasPointNameLevelIds { get; set; }
        public List<string> MeasPointNameFlowIds { get; set; }
        public List<string> MeasPointNameWMIds { get; set; }
        [Display(Name = "Level Measurement")]
        public List<double?> MeasPointLevelMeasures { get; set; }
        [Display(Name = "Flow Measurement")]
        public List<double?> MeasPointFlowMeasures { get; set; }
        [Display(Name = "Water Meter Measurement")]
        public List<double?> MeasPointWMMeasures { get; set; }
        [Display(Name = "Main Reading Comment")]
        public List<string> MeasPointLevelComment { get; set; }
        [Display(Name = "Flow Comment")]
        public List<string> MeasPointFlowComment { get; set; }
        [Display(Name = "Water Meter Comment")]
        public List<string> MeasPointWMComment { get; set; }
        public MeasMoreVM()
        {
            MeasPointLevelIds = new List<int>();
            MeasPointFlowIds = new List<int>();
            MeasPointWMIds = new List<int>();
            MeasPointLevelMeasures = new List<double?>();
            MeasPointFlowMeasures = new List<double?>();
            MeasPointWMMeasures = new List<double?>();
            MeasPointNameLevelIds = new List<string>();
            MeasPointNameFlowIds = new List<string>();
            MeasPointNameWMIds = new List<string>();
            MeasPointLevelComment = new List<string>();
            MeasPointFlowComment = new List<string>();
            MeasPointWMComment = new List<string>();
        }
    }
    public class Meas
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }
        [Display(Name ="Measurement")]
        public double? TheMeasurement { get; set; }
        [ForeignKey("Comment")]
        [Display(Name = "The Comment")]
        public int? CommentId { get; set; }
        [Display(Name = "Comment")]
        public virtual Comment? TheComment {get; set;}
        [Display(Name = "New Comment")]
        public string? NewComment { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime When { get; set; }
        [Display(Name = "Measurement Point")]
        public int? MeasPointId { get; set; }
        [Display(Name = "Measurement Point")]
        public virtual MeasPoint? MeasPoint { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        [Display(Name = "Done By")]
        public string? DoneBy { get; set; }
        public ICollection<PhotoFileMeas>? MeasPhotos { get; set; }

        public Meas() { }
    }
    public class PipeLineMeas
    {
        public double Measurement { get; set; }
        public string? uniqueid { get; set; }
        public string? uniqueidtext { get; set; }
        public string? type { get; set; }
        public DateTime TimeStamp { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public PipeLineMeas()
        {

        }

    }
    public class PhotoFileMeas
    {
        [Key]
        public int Id { get; set; }
        public string? Path { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int MeasId { get; set; }
        public virtual Meas? Meas { get; set; }
    }
}
