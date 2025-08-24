using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class ProjectUser
    {
        [Display(Name = "ID")]
        public int Id { get; set; }
        [ForeignKey("Project")]
        [Display(Name = "Project ID")]
        public int projectId { get; set; }
        public virtual Project Project { get; set; }
        [ForeignKey("ApplicationUser")]
        [Display(Name = "User ID")]
        public string userId { get; set; }
    }
    public class ProjectUserVM
    {
        public int Id { get; set; }
        public string fullname { get; set; }
        public int Projectid { get; set; }
        public virtual Project Project { get; set; }
        public ProjectUserVM()
        {

        }
        public ProjectUserVM(ProjectUser PU,ApplicationUser user,Project p)
        {
            this.Id = PU.Id;
            this.Projectid = p.Id;
            this.Project = p;
            this.fullname = user.FirstName + " " + user.LastName;
            
        }
    }
}
