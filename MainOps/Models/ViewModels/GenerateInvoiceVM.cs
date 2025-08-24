using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ViewModels
{
    public class GenerateInvoiceVM
    {
        [Display(Name = "Start Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime starttime { get; set; }
        [Display(Name = "End Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime endtime { get; set; }
        public bool AddDocumentation { get; set; }
        public bool ShowDocumentation { get; set; }
        public bool ZipItAll { get; set; }
        public bool GenerateAkonto { get; set; }
        [Display(Name = "Project")]
        public int? ProjectId { get; set; }
        [Display(Name = "Sub Project")]
        public int? SubProjectId { get; set; }
        public bool SplitSubProjects { get; set; }
        public bool DownloadExcel { get; set; }
        public bool HideOldItems { get; set; }
        public bool Tax { get; set; }
        public bool PayDownTime { get; set; }
        public bool OnlyHours { get; set; }
        public bool NoMoneyNumbers { get; set; }
        public bool HidePhotos { get; set; }
        public bool AddDocumentationBackGround { get; set; }
        public bool GenerateAkontoBackGround { get; set; }
        public bool GenerateSnapShot { get; set; }
        public bool DownloadSimpleExcel { get; set; }

        public bool CheckInconsistensies { get; set; }
        public bool ShowLogs { get; set; }
        public GenerateInvoiceVM()
        {
            AddDocumentation = false;
            AddDocumentationBackGround = false;
            ShowDocumentation = false;
            GenerateAkonto = false;
            GenerateAkontoBackGround = false;
            SplitSubProjects = false;
            DownloadExcel = false;
            DownloadSimpleExcel = false;
            PayDownTime = false;
            OnlyHours = false;
            NoMoneyNumbers = false;
            Tax = true;
            HidePhotos = false;
            GenerateSnapShot = false;
        }
    }
}
