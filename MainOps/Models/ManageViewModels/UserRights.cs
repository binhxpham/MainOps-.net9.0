using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ManageViewModels
{
    public class UserRights
    {
        public string id { get; set; }
        public bool Guest { get; set; }
        public bool Member { get; set; }
        public bool ProjectMember { get; set; }
        public bool Admin { get; set; }
        public bool DivisionAdmin { get; set; }
        public bool MemberGuest { get; set; }
        public bool StorageManager { get; set; }
        public bool Supervisor { get; set; }
        public bool International { get; set; }
        public bool Manager { get; set; }
        public UserRights()
        {

        }
        public UserRights(ApplicationUser user,List<string> Roles)
        {
            id = user.Id;
            if(Roles.IndexOf("Guest") >= 0)
            {
                Guest = true;
            }
            if (Roles.IndexOf("MemberGuest") >= 0)
            {
                MemberGuest = true;
            }
            if (Roles.IndexOf("Member") >= 0)
            {
                Member = true;
            }
            if (Roles.IndexOf("ProjectMember") >= 0)
            {
                ProjectMember = true;
            }
            if (Roles.IndexOf("Manager") >= 0)
            {
                Manager = true;
            }
            if (Roles.IndexOf("StorageManager") >= 0)
            {
                StorageManager = true;
            }
            if (Roles.IndexOf("International") >= 0)
            {
                International = true;
            }
            if (Roles.IndexOf("Supervisor") >= 0)
            {
                Supervisor = true;
            }
            if (Roles.IndexOf("Admin") >= 0)
            {
                Admin = true;
            }
            if (Roles.IndexOf("DivisionAdmin") >= 0)
            {
                DivisionAdmin = true;
            }
        }
    }
}
