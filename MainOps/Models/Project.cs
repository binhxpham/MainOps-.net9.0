using MainOps.Models.ReportClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class Project
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Project Number")]
        public string? ProjectNr { get; set; }
        [Required]
        [StringLength(128, MinimumLength = 2)]
        [Display(Name = "Project")]
        public string? Name { get; set; }//public string? ColumnName { get; set; }  // Nullable string

        [Required]
        [StringLength(6, MinimumLength = 1)]
        [Display(Name = "Abbreviation")]
        public string? Abbreviation { get; set; }
        [Required]
        [StringLength(128, MinimumLength = 2)]
        [Display(Name = "Client")]
        public string? Client { get; set; }
        [Display(Name = "Client Contact")]
        public string? ClientContact { get; set; }
        [Display(Name = "Client Phone")]
        public string? ClientPhone { get; set; }

        [ForeignKey("CoordSystem")]
        [Display(Name = "Coordinate System ID")]
        public int? CoordSystemId { get; set; }
        [Display(Name = "Coordinate System")]
        public virtual CoordSystem? CoordSystem { get; set; }
        [ForeignKey("Division")]
        public int DivisionId { get; set; }
        public virtual Division? Division { get; set; }
        public ICollection<Document>? Documents { get; set; }
        public ICollection<Generator_Test>? Generator_TestsDocuments { get; set; }
        public ICollection<WTP_Test>? WTP_TestsDocuments { get; set; }
        public ICollection<Well_Installation>? WellInstallationsDocuments { get; set; }
        public ICollection<Well_Development>? WellDevelopmentsDocuments { get; set; }
        public ICollection<TrackItem>? TrackItems { get; set; }
        public ICollection<ItemType>? BoQList { get; set; }
        public ICollection<BoQHeadLine>? BoQHeadlines { get; set; }
        public bool Active { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public ICollection<SubProject>? SubProjects { get; set; }
        [EmailAddress]
        [Display(Name = "Client Email")]
        public string? ClientEmail { get; set; }
        public bool ImplementBreakTime { get; set; }
        [Display(Name = "Responible Person")]
        public string? Responsible_Person { get; set; }
        [Display(Name = "Address")]
        public string? Address { get; set; }
        public bool IsTemplate { get; set; }
        public string? Description { get; set; }
        public int? DepartmentId { get; set; }
        public virtual Department? Department { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? AddressLine1 { get; set; }
        public string? PostalCode { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public bool AlertCalls { get; set; }
        [Display(Name = "Client Contact During Alert")]
        public string? ClientContactAlert { get; set; }
        [Display(Name = "Client Alert Phone")]
        public string? ClientAlertPhone { get; set; }
        //public AddressData GoogleAddressData { get {
        //        AddressData data = new AddressData();
        //        if (this.AddressLine1 != "" && this.PostalCode != "" && this.City != "" && this.Country != "")
        //        {
        //            data.Address = this.AddressLine1;
        //            data.City = this.City;
        //            data.State = null;
        //            data.Country = this.Country;
        //            data.Zip = this.PostalCode;
        //        }
        //        else if (this.AddressLine1 != "" && this.PostalCode != "" && this.City != "")
        //        {
        //            data.Address = this.AddressLine1;
        //            data.City = this.City;
        //            data.State = null;
        //            data.Country = null;
        //            data.Zip = this.PostalCode;
        //        }
        //        else if (this.AddressLine1 != "" && this.PostalCode != "")
        //        {
        //            data.Address = this.AddressLine1;
        //            data.City = null;
        //            data.State = null;
        //            data.Country = null;
        //            data.Zip = this.PostalCode;
        //        }
        //        else if (this.AddressLine1 != "" && this.City != "")
        //        {
        //            data.Address = this.AddressLine1;
        //            data.City = this.City;
        //            data.State = null;
        //            data.Country = null;
        //            data.Zip = null;
        //        }
        //        else if (this.Address != "")
        //        {
        //            string[] parts = this.Address.Split(",");
        //            data.Address = parts[0];
        //            data.City = parts[1].Split(" ")[1];
        //            data.State = null;
        //            data.Country = null;
        //            data.Zip = parts[1].Split(" ")[0];
        //        }

        //        return data;
        //    } 
        //}
        public string? GoogleAddress { get
            {
                bool hasNumber = false;
                if(this.AddressLine1 != null) {
                    foreach(char c in this.AddressLine1)
                    {
                        if(char.IsNumber(c) == true)
                        {
                            hasNumber = true;
                        }
                    }
                    if(hasNumber == false)
                    {
                        this.AddressLine1 = String.Concat(this.AddressLine1," 1");
                    }
                }
                if (this.AddressLine1 != "" && this.PostalCode != "" && this.City != "" && this.Country != "" && this.AddressLine1 != null && this.PostalCode != null && this.City != null && this.Country != null)
                {
                    return String.Concat(this.AddressLine1, ", ", this.PostalCode, " ", this.City, ", ", this.Country);
                }
                else if(this.AddressLine1 != "" && this.PostalCode != "" && this.City != "" && this.AddressLine1 != null && this.PostalCode != null && this.City != null)
                {
                    return String.Concat(this.AddressLine1, ", ", this.PostalCode, " ", this.City);
                }
                else if (this.AddressLine1 != "" && this.PostalCode != "" && this.AddressLine1 != null && this.PostalCode != null)
                {
                    return String.Concat(this.AddressLine1, ", ", this.PostalCode);
                }
                else if (this.AddressLine1 != "" && this.City != "" && this.AddressLine1 != null && this.City != null)
                {
                    return String.Concat(this.AddressLine1, ", ", this.City);
                }
                else if(this.Address != "" && this.Address != null)
                {
                    return this.Address;
                }
                else
                {
                    return "";
                }
            } }
    }
    public class SubProject
    {
        public int Id { get; set; }
        public int? ProjectId { get; set; }
        public virtual Project? Project { get; set; }
        public string? Name { get; set; }
        public string? SubProjectNr { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool Active { get; set; }
        public string? ClientContact { get; set; }
        [EmailAddress]
        [Display(Name = "Client Email")]
        public string? ClientEmail { get; set; }
        public int? ProtokolId { get; set; }
        public string? Type { get; set; }
        [Display(Name = "Address")]
        public string? Address { get; set; }
        public ICollection<Document>? Documents { get; set; }
    }
}
