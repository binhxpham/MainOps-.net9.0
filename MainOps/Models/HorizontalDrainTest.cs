using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class HorizontalDrainTest
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Project")]
        public int ProjectId { get; set; }
        [Display(Name = "Project")]
        public virtual Project Project { get; set; }
        [Display(Name = "SubProject")]
        public int? SubProjectId { get; set; }
        [Display(Name = "SubProject")]
        public virtual SubProject SubProject { get; set; }
        [Display(Name = "Linked Installation")]
        public int? InstallId { get; set; }
        [Display(Name = "Linked Installation")]
        public virtual Install Install { get; set; }
        [Display(Name = "Location/Stretch")]
        public string Location { get; set; }
        [Display(Name = "Distance of Pipe")]
        public double Distance { get; set; }
        [Display(Name = "Time of Test")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Duration Test")]
        public double Duration { get; set; }
        [Display(Name = "Test Log")]
        public string LogText { get; set; }
        [Display(Name = "Done by")]
        public string DoneBy { get; set; }
        public DateTime? EnteredIntoDataBase { get; set; }
        public DateTime? LastEditedInDataBase { get; set; }
        public string Signature { get; set; }
        public ICollection<PhotoFileHorizontalDrainTest> Photos { get; set; }
        [NotMapped]
        public List<Install> Installations { get; set; }
        public HorizontalDrainTest()
        {
            Installations = new List<Install>();
        }
    }
    public class PhotoFileHorizontalDrainTest
    {
        [Key]
        public int Id { get; set; }
        public string Path { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int HorizontalDrainTestId { get; set; }
        public virtual HorizontalDrainTest HorizontalDrainTest { get; set; }
    }
}
