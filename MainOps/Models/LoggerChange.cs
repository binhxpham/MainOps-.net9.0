using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class LoggerChange
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Logger")]
        [Display(Name = "Logger ID")]
        public int LoggerId { get; set; }
        public virtual Logger Logger { get; set; }
        [ForeignKey("MeasPoint")]
        [Display(Name = "Measurement Point ID")]
        public int MeasPointId { get; set; }
        [Display(Name = "Measurement Point")]
        public virtual MeasPoint MeasPoint { get; set; }
        [Display(Name = "Logger Added")]
        public bool LoggerAdded { get; set; }
        [Display(Name = "Logger Removed")]
        public bool LoggerRemoved { get; set; }
        [Display(Name = "When")]
        public DateTime When { get; set; }
        public LoggerChange()
        {

        }
        public LoggerChange(int logId,int MeasPid,bool added,bool removed)
        {
            this.LoggerId = logId;
            this.MeasPointId = MeasPid;
            this.LoggerAdded = added;
            this.LoggerRemoved = removed;
            this.When = DateTime.Now;
        }
    }
}
