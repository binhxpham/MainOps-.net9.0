using MainOps.Models.ReportClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class ProjectUserOverview
    {
        public Division Division { get; set; }
        public List<Project> Projects { get; set; }
        public List<ApplicationUser> Employees { get; set; }
        public List<UserWithDailyReports> UsersWithDailyReports { get; set; }
        public int? ProjectFilterId { get; set; }
        public string EmployeeFilterId { get; set; }
        public ProjectUserOverview()
        {
            Projects = new List<Project>();
            Employees = new List<ApplicationUser>();
            UsersWithDailyReports = new List<UserWithDailyReports>();
        }

    }
    public class UserWithDailyReports
    {
        public ApplicationUser User { get; set; }
        public List<Daily_Report_2> DailyReports { get; set; }
        public UserWithDailyReports()
        {
            DailyReports = new List<Daily_Report_2>();
        }
    }
}
