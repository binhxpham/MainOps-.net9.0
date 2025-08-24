using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainOps.Models
{
    public class Log
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Photo path")]
        public string PhotoPath { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        [ForeignKey("TrackItem")]
        public int? TrackItemId { get; set; }
        public virtual TrackItem TrackItem { get; set; }
        public string TheUser { get; set; }
    }
    public class Log2
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Photo path")]
        public string PhotoPath { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        [ForeignKey("TrackItem")]
        public int? ItemTypeId { get; set; }
        public virtual ItemType ItemType { get; set; }
        public string TheUser { get; set; }
        public string otherinfo { get; set; }
    }
}