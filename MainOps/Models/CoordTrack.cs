using MainOps.ExtensionMethods;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainOps.Models
{
    public class CoordTrack
    {
        [Key]
        public int Id { get; set; }
        public string TypeCoord { get; set; }
        [Display(Name = "Latitude")]
        public double? Latitude { get; set; }
        [Display(Name = "Longitude")]
        public double? Longitude { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Item")]
        [ForeignKey("TrackItem")]
        public int TrackItemId { get; set; }
        public virtual TrackItem TrackItem { get; set; }
        public bool Within_Coords(double latitude, double longitude)
        {
            double distance = DistanceAlgorithm.DistanceBetweenPlaces(longitude, latitude, Convert.ToDouble(this.Longitude), Convert.ToDouble(this.Latitude));
            if (distance < 0.01)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public class CoordTrack2
    {
        [Key]
        public int Id { get; set; }
        public string TypeCoord { get; set; }
        [Display(Name = "Latitude")]
        public double? Latitude { get; set; }
        [Display(Name = "Longitude")]
        public double? Longitude { get; set; }
        [Display(Name = "Accuracy")]
        public double? Accuracy { get; set; }
        [Display(Name = "Latitude")]
        public double? Latitude_backup { get; set; }
        [Display(Name = "Longitude")]
        public double? Longitude_backup { get; set; }
        [Display(Name = "Accuracy")]
        public double? Accuracy_backup { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Item")]
        [ForeignKey("Install")]
        public int? InstallId { get; set; }
        public virtual Install Install { get; set; }
        [ForeignKey("Arrival")]
        public int? ArrivalId { get; set; }
        public virtual Arrival Arrival { get; set; }
        [ForeignKey("Mobilize")]
        public int? MobilizeId { get; set; }
        public virtual Mobilize Mobilize { get; set; }
        [ForeignKey("MeasPoint")]
        public int? MeasPointId { get; set; }
        public virtual MeasPoint MeasPoint { get; set; }
        public bool Within_Coords(double latitude,double longitude)
        {
            double distance = DistanceAlgorithm.DistanceBetweenPlaces(longitude, latitude, Convert.ToDouble(this.Longitude), Convert.ToDouble(this.Latitude));
            if(distance < 0.01)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public class CoordTrack3
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Latitude")]
        public double? Latitude { get; set; }
        [Display(Name = "Longitude")]
        public double? Longitude { get; set; }
        [Display(Name = "Accuracy")]
        public double? Accuracy { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Item")]
        [ForeignKey("HJItem")]
        public int HJItemId { get; set; }
        public virtual HJItem HJItem { get; set; }
        public bool Within_Coords(double latitude, double longitude)
        {
            double distance = DistanceAlgorithm.DistanceBetweenPlaces(longitude, latitude, Convert.ToDouble(this.Longitude), Convert.ToDouble(this.Latitude));
            if (distance < 0.01)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}