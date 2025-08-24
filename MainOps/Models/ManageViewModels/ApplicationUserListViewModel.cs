
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ManageViewModels
{
    public class ApplicationUserListViewModel
    {
        [Display(Name = "User ID")]
        public string UserId { get; set; }
        [Display(Name = "Username")]
        public string Username{get; set;}
        [Display(Name = "User Email Address")]
        public string Email { get; set; }
        [Display(Name = "User Roles")]
        public string Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Active { get; set; }
        public bool MemberShipConfirmed { get; set; }
        public bool EmailConfirmed { get; set; }
        public ApplicationUserListViewModel()
        {
        }
    }

}
