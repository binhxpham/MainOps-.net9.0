using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class TruckDailyReport
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Date")]
        public DateTime Dato { get; set; }
        [Display(Name = "Sites")]
        public ICollection<TruckSite> Sites { get; set; }
        [Display(Name = "Done By")]
        public string DoneBy { get; set; }
        public int? HJItemId { get; set; }
        public virtual HJItem HJItem {get; set;}
        public TruckDailyReport()
        {

        }
    }
    public class TruckIndexVM
    {
        public DateTime Dato { get; set; }
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public int? SubProjectId { get; set; }
        public virtual SubProject SubProject { get; set; }
        public TimeSpan Hours { get; set; }
        public string DoneBy { get; set; }
        public int TruckDailyReportId { get; set; }
        public int TruckSiteId { get; set; }
        public string Description { get; set; }
        public double? KM_start { get; set; }
        [Display(Name = "KM end")]
        public double? KM_end { get; set; }
        public int? HJItemId { get; set; }
        public virtual HJItem HJItem { get; set; }
        public TruckIndexVM()
        {

        }
        public TruckIndexVM(TruckDailyReport dr,TruckSite site)
        {
            HJItemId = dr.HJItemId;
            HJItem = dr.HJItem;
            Dato = dr.Dato;
            ProjectId = Convert.ToInt32(site.ProjectId);
            Project = site.Project;
            SubProjectId = site.SubProjectId;
            SubProject = site.SubProject;
            Hours = site.Hours;
            DoneBy = dr.DoneBy;
            TruckDailyReportId = dr.Id;
            TruckSiteId = site.Id;
            Description = site.Description;
            KM_start = site.KM_start;
            KM_end = site.KM_end;
        }
    }
    public class TruckVM
    {
        public int? TruckDailyReportId { get; set; }
        [Display(Name = "Date")]
        public DateTime Dato { get; set; }
        public List<TruckSite> Sites { get; set; }
        public int? HJItemId { get; set; }
        public virtual HJItem HJItem { get; set; }
        public TruckVM()
        {
            Sites = new List<TruckSite>();
            Dato = DateTime.Now;
            for(int i = 0; i < 10; i++)
            {
                TruckSite newsite = new TruckSite();
                Sites.Add(newsite);
            }
        }
        public TruckVM(TruckDailyReport TDR)
        {
            TruckDailyReportId = TDR.Id;
            HJItemId = TDR.HJItemId;
            Sites = new List<TruckSite>();
            Dato = TDR.Dato;
            for(int i = 0; i < 10; i++)
            {
                if(i < TDR.Sites.Count())
                {
                    Sites.Add(TDR.Sites.ElementAt(i));
                }
                else
                {
                    Sites.Add(new TruckSite());
                }
            }
        }
    }
    public class TruckSite
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Project")]
        public int? ProjectId { get; set; }
        [Display(Name = "Project")]
        public virtual Project Project { get; set; }
        [Display(Name = "SubProject")]
        public int? SubProjectId { get; set; }
        [Display(Name = "SubProject")]
        public virtual SubProject SubProject { get; set; }
        [Display(Name = "Hours")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Between 00:00 to 23:59")]
        public TimeSpan Hours { get; set; }
        [Display(Name = "Truck Report ID")]
        public int? TruckDailyReportId { get; set; }
        public virtual TruckDailyReport TruckDailyReport { get; set; }
        public string Description { get; set; }
        [Display( Name = "KM start")]
        public double? KM_start { get; set; }
        [Display(Name = "KM end")]
        public double? KM_end { get; set; }
        public TruckSite()
        {

        }
    }
}
