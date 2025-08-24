using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class AlarmReportReceiver
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Project")]
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public string UserId { get; set; }
        
    }
    public class AlarmReportReceiverVM
    {
        public int Id { get; set; }
        public string fullname { get; set; }
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public AlarmReportReceiverVM()
        {

        }
        public AlarmReportReceiverVM(AlarmReportReceiver PU, ApplicationUser user, Project p)
        {
            this.Id = PU.Id;
            this.ProjectId = p.Id;
            this.Project = p;
            this.fullname = user.FirstName + " " + user.LastName;

        }
    }
}
