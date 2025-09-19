using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{

    public class HJItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Equipment")]
        public string Name { get; set; }
        [Display(Name = "HJ ID")]
        public string? HJId { get; set; }

        [Display(Name = "Weight [Kg]")]
        public double? weight { get; set; }
        [Display(Name = "Latitude")]
        public double? latitude { get; set; }
        [Display(Name = "Longitude")]
        public double? longitude { get; set; }
        [Display(Name = "Length")]
        public double? ItemLength { get; set; }

        [Display(Name = "Width")]
        public double? ItemWidth { get; set; }

        [Display(Name = "Height")]
        public double? ItemHeight { get; set; }

        public string? PathToPicture { get; set; }
        public string? PathTo3DDrawing { get; set; }
        public string? Comments { get; set; }

        [ForeignKey(nameof(Division))]
        public int? DivisionId { get; set; }
        public virtual Division? Division { get; set; }

        [ForeignKey(nameof(HJItemClass))]
        public int? HJItemClassId { get; set; }

        [Display(Name = "Item Class")]
        public virtual HJItemClass? HJItemClass { get; set; }

        [Display(Name = "Owned By")]
        public string? Ownership { get; set; }

        [Display(Name = "GPS Track On")]
        public bool GPS_Tracker { get; set; }

        [Display(Name = "Maintenance List")]
        public ICollection<Maintenance> MaintenanceList { get; set; }
        public ICollection<Item_Location> Locations { get; set; }
        [Display(Name = "Dimensions [m] L x W x H")]
        public string Measures
        {
            get
            {
                if(this.ItemLength != null && this.ItemHeight != null && this.ItemWidth != null)
                {

                
                return string.Format("{0:N2} x {1:N2} x {2:N2}", this.ItemLength, this.ItemWidth, this.ItemHeight);
                }
                else
                {
                    return "";
                }
            }
        }
        public void SetHJNumber(int lastNum,string masterclassnumber,string itemclassnumber)
        {
            var newnum = lastNum + 1;

            if (Convert.ToInt32(masterclassnumber) < 10 && Convert.ToInt32(itemclassnumber) < 10 && newnum < 10)
            {
                this.HJId = string.Format("0{0}-0{1}-00{2}", masterclassnumber, itemclassnumber, newnum);
            }
            else if (Convert.ToInt32(masterclassnumber) < 10 && Convert.ToInt32(itemclassnumber) < 10 && newnum < 100)
            {
                this.HJId = string.Format("0{0}-0{1}-0{2}", masterclassnumber, itemclassnumber, newnum);
            }
            else if (Convert.ToInt32(masterclassnumber) < 10 && Convert.ToInt32(itemclassnumber) < 10 && newnum < 1000)
            {
                this.HJId = string.Format("0{0}-0{1}-{2}", masterclassnumber, itemclassnumber, newnum);
            }
            else if (Convert.ToInt32(masterclassnumber) >= 10 && Convert.ToInt32(itemclassnumber) < 10 && newnum < 10)
            {
                this.HJId = string.Format("{0}-0{1}-00{2}", masterclassnumber, itemclassnumber, newnum);
            }
            else if (Convert.ToInt32(masterclassnumber) >= 10 && Convert.ToInt32(itemclassnumber) < 10 && newnum < 100)
            {
                this.HJId = string.Format("{0}-0{1}-0{2}", masterclassnumber, itemclassnumber, newnum);
            }
            else if (Convert.ToInt32(masterclassnumber) >= 10 && Convert.ToInt32(itemclassnumber) < 10 && newnum < 1000)
            {
                this.HJId = string.Format("{0}-0{1}-{2}", masterclassnumber, itemclassnumber, newnum);
            }
            else if (Convert.ToInt32(masterclassnumber) < 10 && Convert.ToInt32(itemclassnumber) >= 10 && newnum < 10)
            {
                this.HJId = string.Format("0{0}-{1}-00{2}", masterclassnumber, itemclassnumber, newnum);
            }
            else if (Convert.ToInt32(masterclassnumber) < 10 && Convert.ToInt32(itemclassnumber) >= 10 && newnum < 100)
            {
                this.HJId = string.Format("0{0}-{1}-0{2}", masterclassnumber, itemclassnumber, newnum);
            }
            else if (Convert.ToInt32(masterclassnumber) < 10 && Convert.ToInt32(itemclassnumber) >= 10 && newnum < 1000)
            {
                this.HJId = string.Format("0{0}-{1}-{2}", masterclassnumber, itemclassnumber, newnum);
            }
            else if (Convert.ToInt32(masterclassnumber) >= 10 && Convert.ToInt32(itemclassnumber) >= 10 && newnum < 10)
            {
                this.HJId = string.Format("{0}-{1}-00{2}", masterclassnumber, itemclassnumber, newnum);
            }
            else if (Convert.ToInt32(masterclassnumber) >= 10 && Convert.ToInt32(itemclassnumber) >= 10 && newnum < 100)
            {
                this.HJId = string.Format("{0}-{1}-0{2}", masterclassnumber, itemclassnumber, newnum);
            }
            else
            {
                this.HJId = string.Format("{0}-{1}-{2}", masterclassnumber, itemclassnumber, newnum);
            }

        }
    }

    public class HJItemClasses
    {
        public List<HJItemMasterClass> masters { get; set; }
        public List<HJItemClass> subclasses { get; set; }
    }
    public class HJItemMaintenaceVM
    {
        public int? HJItemId { get; set; }
        public virtual HJItem? HJItem { get; set; }
        [Display(Name = "Last Service Maintenance")]
        public DateTime? Last_Service { get; set; }
        [Display(Name = "Last Safety Check")]
        public DateTime? Last_Safety { get; set; }
        [Display(Name = "Last Electrical Check")]
        public DateTime? Last_Electrical { get; set; }
        [Display(Name = "Next Service Maintenance")]
        public DateTime? Next_Service { get; set; }
        [Display(Name = "Next Safety Check")]
        public DateTime? Next_Safety { get; set; }
        [Display(Name = "Next Electrical Check")]
        public DateTime? Next_Electrical { get; set; }
        [Display(Name = "Service Maintenance Frequency")]
        public int? Service_Maintenance_Freq { get; set; }
        [Display(Name = "Safety Check Frequency")]
        public int? Safety_Maintenance_Freq { get; set; }
        [Display(Name = "Electrical Check Frequency")]
        public int? Electrical_Maintenance_Freq { get; set; }
        [Display(Name = "The Next Check")]
        public DateTime Next_Check { get; set; }
        public HJItemMaintenaceVM()
        {

        }
        
        public HJItemMaintenaceVM(HJItem item)
        {
            this.HJItemId = item.Id;
            this.HJItem = item;
            this.Service_Maintenance_Freq = item.HJItemClass.Service_Maintenance_Freq;
            this.Safety_Maintenance_Freq = item.HJItemClass.Safety_Maintenance_Freq;
            this.Electrical_Maintenance_Freq = item.HJItemClass.Electrical_Maintenance_Freq;
            if(item.MaintenanceList.Count > 0) {
                var last_elec = item.MaintenanceList.Where(x => x.MaintenanceEntries.Where(y => y.MaintenanceSubTypeId.Equals(14) && y.MaintenanceSubTypeId.Equals(46)).Count() >= 1).LastOrDefault();
                //var last_elec = item.MaintenanceList.Where(x => x.MaintenanceEntriesMaintenanceTypeId.Equals(14) && x.MaintenanceSubTypeId.Equals(46)).LastOrDefault();
                if(last_elec != null)
                {
                    this.Last_Electrical = last_elec.TimeStamp;
                    this.Next_Electrical = Convert.ToDateTime(this.Last_Electrical).AddDays(Convert.ToInt32(this.Electrical_Maintenance_Freq));
                }
                var last_safety = item.MaintenanceList.Where(x => x.MaintenanceEntries.Where(y => y.MaintenanceSubTypeId.Equals(14) && y.MaintenanceSubTypeId.Equals(47)).Count() >= 1).LastOrDefault();
                //var last_safety = item.MaintenanceList.Where(x => x.MaintenanceTypeId.Equals(14) && x.MaintenanceSubTypeId.Equals(47)).LastOrDefault();
                if (last_safety != null)
                {
                    this.Last_Safety = last_safety.TimeStamp;
                    this.Next_Safety = Convert.ToDateTime(this.Last_Safety).AddDays(Convert.ToInt32(this.Safety_Maintenance_Freq));
                }
                var last_service = item.MaintenanceList.Where(x => x.MaintenanceEntries.Where(y => y.MaintenanceSubTypeId.Equals(14) && y.MaintenanceSubTypeId.Equals(45)).Count() >= 1).LastOrDefault();
                //var last_service = item.MaintenanceList.Where(x => x.MaintenanceTypeId.Equals(14) && x.MaintenanceSubTypeId.Equals(45)).LastOrDefault();
                if (last_service != null)
                {
                    this.Last_Service = last_service.TimeStamp;
                    this.Next_Service = Convert.ToDateTime(this.Last_Service).AddDays(Convert.ToInt32(this.Service_Maintenance_Freq));
                }
                if(last_elec != null && last_safety != null && last_service != null && this.Electrical_Maintenance_Freq > 0 && this.Service_Maintenance_Freq > 0 && this.Safety_Maintenance_Freq > 0)
                {
                    if(this.Last_Electrical <= this.Last_Safety && this.Last_Electrical <= this.Last_Service)
                    {
                        this.Next_Check = Convert.ToDateTime(this.Next_Electrical);
                    }
                    else if (this.Last_Safety <= this.Last_Electrical && this.Last_Safety <= this.Last_Service)
                    {
                        this.Next_Check = Convert.ToDateTime(this.Next_Safety);
                    }
                    else
                    {
                        this.Next_Check = Convert.ToDateTime(this.Next_Service);
                    }
                }
                else if (last_elec != null && last_safety != null && Last_Service == null)
                {
                    if(this.Service_Maintenance_Freq > 0)
                    {
                        Next_Check = DateTime.Now;
                    }
                    else if (this.Last_Electrical <= this.Last_Safety)
                    {
                        this.Next_Check = Convert.ToDateTime(this.Next_Electrical);
                    }
                    else 
                    {
                        this.Next_Check = Convert.ToDateTime(this.Next_Safety);
                    }
                }
                else if (last_elec != null && last_safety == null && Last_Service != null)
                {
                    if (this.Safety_Maintenance_Freq > 0)
                    {
                        Next_Check = DateTime.Now;
                    }
                    else if (this.Last_Electrical <= this.Last_Service)
                    {
                        this.Next_Check = Convert.ToDateTime(this.Next_Electrical);
                    }
                    else
                    {
                        this.Next_Check = Convert.ToDateTime(this.Next_Service);
                    }
                }
                else if (last_elec == null && last_safety != null && Last_Service != null)
                {
                    if (this.Electrical_Maintenance_Freq > 0)
                    {
                        Next_Check = DateTime.Now;
                    }
                    else if (this.Last_Safety <= this.Last_Service)
                    {
                        this.Next_Check = Convert.ToDateTime(this.Next_Safety);
                    }
                    else
                    {
                        this.Next_Check = Convert.ToDateTime(this.Next_Service);
                    }
                }
                else if(last_elec != null && last_safety == null && last_service == null)
                {
                    if(this.Safety_Maintenance_Freq > 0 || this.Service_Maintenance_Freq > 0)
                    {
                        this.Next_Check = DateTime.Now;
                    }
                    else
                    {
                        this.Next_Check = Convert.ToDateTime(this.Next_Electrical);
                    }
                }
                else if (last_elec == null && last_safety != null && last_service == null)
                {
                    if (this.Electrical_Maintenance_Freq > 0 || this.Service_Maintenance_Freq > 0)
                    {
                        this.Next_Check = DateTime.Now;
                    }
                    
                    else
                    {
                        this.Next_Check = Convert.ToDateTime(this.Next_Safety);
                    }
                }
                else if (last_elec == null && last_safety == null && last_service != null)
                {
                    if (this.Electrical_Maintenance_Freq > 0 || this.Safety_Maintenance_Freq > 0)
                    {
                        this.Next_Check = DateTime.Now;
                    }

                    else
                    {
                        this.Next_Check = Convert.ToDateTime(this.Next_Service);
                    }
                }
                else
                {
                    if(this.Electrical_Maintenance_Freq > 0 || this.Safety_Maintenance_Freq > 0 || this.Service_Maintenance_Freq > 0)
                    {
                        this.Next_Check = DateTime.Now;
                    }
                }
            }
            else
            {
                this.Next_Check = DateTime.Now;
                this.Next_Service = DateTime.Now;
                this.Next_Electrical = DateTime.Now;
                this.Next_Safety = DateTime.Now;
            }
        }



    }
    public class Item_Locations_VM
    {
        public List<Item_Location>? Locations { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
    public class Item_Location
    {
        [Key]
        public int Id { get; set; }
        public int? HJItemId { get; set; }
        public virtual HJItem? HJItem { get; set; }
        public int ProjectId { get; set; }
        public virtual Project? Project { get; set; }
        public int? SubProjectId { get; set; }
        public virtual SubProject? SubProject { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public double CalculatedRent(DateTime start,DateTime end)
        {
            if(StartTime.Date <= end.Date) { 
                if(StartTime.Date >= start.Date)
                {
                    if (EndTime != null)
                    {
                        if (EndTime.Value.Date > end.Date)
                        {
                            return ((end.Date - this.StartTime.Date).TotalDays + 1.0) * HJItem.HJItemClass.Internal_Rent;
                        }
                        else
                        {
                            return ((this.EndTime.Value.Date - this.StartTime.Date).TotalDays + 1.0) * HJItem.HJItemClass.Internal_Rent;
                        }
                    }
                    else
                    {
                        return ((end.Date - this.StartTime.Date).TotalDays + 1.0) * HJItem.HJItemClass.Internal_Rent;

                    }
                }
                else
                {
                    if(EndTime != null)
                    {
                        if(EndTime.Value.Date > end.Date)
                        {
                            return ((end.Date - start.Date).TotalDays + 1.0) * HJItem.HJItemClass.Internal_Rent;
                        }
                        else
                        {
                            return ((this.EndTime.Value.Date - start.Date).TotalDays + 1.0) * HJItem.HJItemClass.Internal_Rent;
                        }
                    }
                    else
                    {
                        return ((end.Date - start.Date).TotalDays +1.0) * HJItem.HJItemClass.Internal_Rent;

                    }
                }
            }
            else
            {
                return 0.0;
            }
        }
        public Item_Location()
        {

        }      
           
    }
    
}
