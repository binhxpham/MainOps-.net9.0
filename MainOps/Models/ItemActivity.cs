using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class ItemActivity
    {
        [Key]
        public int Id { get; set; }
        public DateTime TheDate { get; set; }
        public bool WasActive { get; set; }
        public int? HJItemId { get; set; }
        public virtual HJItem HJItem { get; set; }
        public string UniqueID { get; set; }
        public int? ProjectId { get; set; }
        public int? SubProjectId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string ItemName { get; set; }
        public int? InstallId { get; set; }
        public virtual Install Install { get; set; }
        public int? ArrivalId { get; set; }
        public virtual Arrival Arrival { get; set; }
        public TimeSpan OperationTime { get; set; }
        public DateTime StartTime { get; set; }
    }
    public class NeedsOilChange
    {
        [Key]
        public int Id { get; set; }
        public double HoursOperated { get; set; }
        public string UniqueID { get; set; }
        public DateTime LastOilChange { get; set; }
    }
}
