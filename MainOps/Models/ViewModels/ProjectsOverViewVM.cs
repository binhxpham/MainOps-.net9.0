using MainOps.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ViewModels
{
    public class ProjectsOverViewVM
    {
        public List<Project> Projects { get; set; }
        public List<SubProject> SubProjects { get; set; }
        public List<string> ProjectViews { get; set; }
        public List<string> SubProjectViews { get; set; }
        public ProjectsOverViewVM()
        {
            Projects = new List<Project>();
            SubProjects = new List<SubProject>();
            ProjectViews = new List<string>();
            SubProjectViews = new List<string>();
        }
        public ProjectsOverViewVM(List<Project> ps,List<SubProject> sps,LocService _localizer)
        {
            ProjectViews = new List<string>();
            SubProjectViews = new List<string>();
            this.Projects = ps;
            this.SubProjects = sps;
            foreach(Project p in ps) {
                string totalviewstring = String.Concat("<p>", _localizer.GetLocalizedHtmlString("Project Number").Value, ": ", p.ProjectNr, "</p>");
                totalviewstring += String.Concat("<p>", _localizer.GetLocalizedHtmlString("Project Name").Value, ": ", p.Name, "</p>");
                if(p.DepartmentId != null) { 
                    totalviewstring += String.Concat("<p>", _localizer.GetLocalizedHtmlString("Department").Value, ": ", p.Department.DepartmentName, "</p>");
                }
                if(p.Responsible_Person != "") { 
                    totalviewstring += String.Concat("<p>", _localizer.GetLocalizedHtmlString("Project Manager").Value, ": ", p.Responsible_Person, "</p>");
                }
                if (p.City != "")
                {
                    totalviewstring += String.Concat("<p>", _localizer.GetLocalizedHtmlString("City").Value, ": ", p.City, "</p>");
                }
                if (p.Client != "") { 
                    totalviewstring += String.Concat("<p>", _localizer.GetLocalizedHtmlString("Client").Value, ": ", p.Client, "</p>");
                }
                if (p.ClientContact != "")
                {
                    totalviewstring += String.Concat("<p>", _localizer.GetLocalizedHtmlString("Client Contact").Value, ": ", p.ClientContact, "</p>");
                }
                if (p.ClientPhone != "")
                {
                    totalviewstring += String.Concat("<p>", _localizer.GetLocalizedHtmlString("Client Phone").Value, ": ", p.ClientPhone, "</p>");
                }
                if (p.StartDate != null)
                {
                    totalviewstring += String.Concat("<p>", _localizer.GetLocalizedHtmlString("Start Date").Value, ": ", String.Format("{0:dd/MM/yyyy}", p.StartDate), "</p>");
                }
                if (p.EndDate != null)
                {
                    totalviewstring += String.Concat("<p>", _localizer.GetLocalizedHtmlString("End Date").Value, ": ", String.Format("{0:dd/MM/yyyy}", p.EndDate), "</p>");
                }
                //if (p.Address != null)
                //{
                //    totalviewstring += String.Concat("<p>", _localizer.GetLocalizedHtmlString("Address").Value, ": ", p.Address, "</p>");
                //}
                //if(p.AddressLine1 != "" && p.PostalCode != "" && p.City != "" && p.Country != "")
                //{
                //    totalviewstring += String.Concat("<p><table><tr><td>",_localizer.GetLocalizedHtmlString("Address").Value, ": ", "</td><td>", p.AddressLine1,"</td></tr>",
                //        "<tr><td>",_localizer.GetLocalizedHtmlString("PostalCode").Value, ": ", " </td><td> ", p.PostalCode," </td></tr>", 
                //        "<tr><td>", _localizer.GetLocalizedHtmlString("City").Value, ": ", "</td><td>", p.City, "</td></tr>",
                //        "<tr><td>", _localizer.GetLocalizedHtmlString("Country").Value, ": ", "</td><td>", p.Country, "</td></tr>","</table></p>");
                //}
                ProjectViews.Add(totalviewstring);
            }
            foreach(SubProject sp in sps)
            {
                string totalviewstring2 = String.Concat("<p>",_localizer.GetLocalizedHtmlString("Project").Value, ": ", sp.Project.ProjectNr, " : ", sp.Project.Name, "</p>");
                if(sp.SubProjectNr != "")
                {
                    totalviewstring2 += String.Concat("<p>", _localizer.GetLocalizedHtmlString("Sub Project").Value, ": ", sp.SubProjectNr," : ",sp.Name,"</p>");
                }
                else
                {
                    totalviewstring2 += String.Concat("<p>", _localizer.GetLocalizedHtmlString("Sub Project").Value, ": ", sp.Name, "</p>");
                }
                if (sp.Project.DepartmentId != null)
                {
                    totalviewstring2 += String.Concat("<p>", _localizer.GetLocalizedHtmlString("Department").Value, ": ", sp.Project.Department.DepartmentName, "</p>");
                }
                if (sp.Project.Responsible_Person != "")
                {
                    totalviewstring2 = String.Concat("<p>", _localizer.GetLocalizedHtmlString("Project Manager").Value, ": ", sp.Project.Responsible_Person, "</p>");
                }
                if (sp.Project.City != "")
                {
                    totalviewstring2 += String.Concat("<p>", _localizer.GetLocalizedHtmlString("City").Value, ": ", sp.Project.City, "</p>");
                }
                if (sp.Project.Client != "")
                {
                    totalviewstring2 += String.Concat("<p>", _localizer.GetLocalizedHtmlString("Client").Value, ": ", sp.Project.Client, "</p>");
                }
                if (sp.Project.ClientContact != "")
                {
                    totalviewstring2 += String.Concat("<p>", _localizer.GetLocalizedHtmlString("Client Contact").Value, ": ", sp.Project.ClientContact, "</p>");
                }
                if (sp.Project.ClientPhone != "")
                {
                    totalviewstring2 += String.Concat("<p>", _localizer.GetLocalizedHtmlString("Client Phone").Value, ": ", sp.Project.ClientPhone, "</p>");
                }
                if (sp.Project.StartDate != null)
                {
                    totalviewstring2 += String.Concat("<p>", _localizer.GetLocalizedHtmlString("Start Date").Value, ": ", String.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(sp.Project.StartDate).Date), "</p>");
                }
                if (sp.Project.EndDate != null)
                {
                    totalviewstring2 += String.Concat("<p>", _localizer.GetLocalizedHtmlString("End Date").Value,": ", String.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(sp.Project.EndDate).Date), "</p>");
                }
                SubProjectViews.Add(totalviewstring2);
            }
        }
    }
}
