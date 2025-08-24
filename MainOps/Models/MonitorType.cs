using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainOps.Models
{
    public class MonitorType
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Monitor Type")]
        public string MonitorTypeName { get; set; }
        [Display(Name = "Monitoring frequency")]
        public int frequency { get; set; }
        [Display(Name = "Alert time")]
        public int alertfrequency { get; set; }
        [ForeignKey("Divisions")]
        public int DivisionId { get; set; }
        public virtual Division Division { get; set; }
        public MonitorType()
        {

        }
        public MonitorType(MonitorType mt)
        {
            this.MonitorTypeName = mt.MonitorTypeName;
            this.frequency = mt.frequency;
        }
        public MonitorType(MonitorType mt,Division div)
        {
            this.MonitorTypeName = mt.MonitorTypeName;
            this.frequency = mt.frequency;
            this.DivisionId = div.Id;
        }
    }
}