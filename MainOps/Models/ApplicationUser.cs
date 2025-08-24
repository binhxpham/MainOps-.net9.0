using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainOps.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(128, MinimumLength = 2)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(128, MinimumLength = 2)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        public bool MemberShipConfirmed { get; set; }
        [Display(Name = "Full Name")]
        public string full_name()
        {
            return string.Format("{0} {1}", this.FirstName, this.LastName);
        }
        [ForeignKey("Division")]
        public int DivisionId { get; set; }
        public virtual Division Division { get; set; }
        public string? PicturePath { get; set; }
        public string? UserLog { get; set; }
        public bool Active { get; set; } = false;
        public ICollection<PersonalFile> PersonalFiles { get; set; }
        //public IList<WorkTask> WorkTasks { get; set; }
    }
    
}
