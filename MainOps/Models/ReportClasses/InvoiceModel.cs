using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ReportClasses
{
    public class InvoiceModel
    {
        public List<InvoiceItem> items { get; set; }
        public List<InvoiceItem> allItems { get; set; }
        public List<InvoiceItem> inconsistentItems { get; set; }
        public List<InvoiceItem> allInconsistentItems { get; set; }
        public List<BoQHeadLine> headlines { get; set; }
        public List<Log2> Logs { get; set; }
        public decimal? installcosts { get; set; }
        public decimal? arrivalcosts { get; set; }
        public decimal? mobcosts { get; set; }
        public decimal? hourcosts { get; set; }
        public decimal? extracosts { get; set; }
        public List<Payment> Payments { get; set; }
        public List<Payment> AllPayments { get; set; }
        public List<Invoice> Invoices { get; set; }
        public List<Invoice> AllInvoices { get; set; }
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }
        public int ProjectId { get; set; }
        public virtual Project Project {get; set;}
        public int? SubProjectId { get; set; }
        public virtual SubProject SubProject { get; set; }
        [Display(Name = "Date Printed")]
        public DateTime ReportDate { get; set; }
        public bool HideOldItems { get; set; }
        public bool NoMoneyNumbers { get; set; }
        public bool AllInvoicesSent { get; set; }
        public DateTime GenerationTime { get; set; }
        public InvoiceModel()
        {

        }
        public InvoiceModel(InvoiceModel model)
        {
            this.items = model.items;
            this.Logs = model.Logs;
            this.inconsistentItems = model.inconsistentItems;
            this.allInconsistentItems = model.allInconsistentItems;
            this.allItems = model.allItems;
            this.starttime = model.starttime;
            this.endtime = model.endtime;
            this.ProjectId = model.ProjectId;
            this.Payments = model.Payments;
            this.AllPayments = model.AllPayments;
            this.headlines = model.headlines;
            this.SubProjectId = model.SubProjectId;
            this.HideOldItems = model.HideOldItems;
            this.NoMoneyNumbers = model.NoMoneyNumbers;
            this.Invoices = model.Invoices;
            this.AllInvoices = model.AllInvoices;
            this.Payments = model.Payments;
            this.AllPayments = model.AllPayments;
            this.GenerationTime = model.GenerationTime;
            if(model.Project != null)
            {
                this.Project = model.Project;
            }
            if(model.SubProject != null)
            {
                this.SubProject = model.SubProject;
            }
            this.ReportDate = model.ReportDate;
        }
        public InvoiceModel(InvoiceModel model,SubProject sb)
        {
            this.items = model.items.Where(x => x.SubProjectId.Equals(sb.Id)).ToList();
            this.allItems = model.allItems.Where(x => x.SubProjectId.Equals(sb.Id)).ToList();
            this.Logs = model.Logs;
            this.inconsistentItems = model.inconsistentItems.Where(x => x.SubProjectId.Equals(sb.Id)).ToList();
            this.allInconsistentItems = model.allInconsistentItems.Where(x => x.SubProjectId.Equals(sb.Id)).ToList();
            this.starttime = model.starttime;
            this.endtime = model.endtime;
            this.ProjectId = model.ProjectId;
            this.Payments = model.Payments.Where(x => x.SubProjectId.Equals(sb.Id)).ToList();
            this.AllPayments = model.AllPayments.Where(x => x.SubProjectId.Equals(sb.Id)).ToList();
            this.Invoices = model.Invoices;
            this.AllInvoices = model.AllInvoices;
            this.headlines = model.headlines;
            this.SubProjectId = sb.Id;
            this.HideOldItems = model.HideOldItems;
            this.NoMoneyNumbers = model.NoMoneyNumbers;
            this.GenerationTime = model.GenerationTime;
            if(model.Project != null)
            {
                this.Project = model.Project;
            }
            this.SubProject = sb;           
            this.ReportDate = model.ReportDate;
        }
}
}
