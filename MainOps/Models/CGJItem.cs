using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class CGJItem
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Equipment")]
        public string Name { get; set; }
        [Display(Name = "CGJ ID")]
        public string CGJId { get; set; }
        [Display(Name = "Weight [Kg]")]
        public double? weight { get; set; }

        [Display(Name = "Dpt.")]
        public string Department { get; set; }

        [Display(Name = "Contact person")]
        public string ContactPerson { get; set; }
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
        public string PathToPicture { get; set; }
        public string PathTo3DDrawing { get; set; }
        public string Comments { get; set; }
        [ForeignKey("Division")]
        public int? DivisionId { get; set; }
        public virtual Division Division { get; set; }
        [ForeignKey("CGJItemClass")]
        public int? CGJItemClassId { get; set; }
        [Display(Name = "Item Class")]
        public virtual CGJItemClass CGJItemClass { get; set; }
        [Display(Name = "Owned By")]
        public string Ownership { get; set; }
        [Display(Name = "GPS Track On")]
        public bool GPS_Tracker { get; set; }

       // [Display(Name = "Maintenance List")]
        //public ICollection<Maintenance> MaintenanceList { get; set; }
        public ICollection<CGJItem_Location> Locations { get; set; }
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
        public void SetCGJNumber_(int lastNum,string masterclassnumber,string itemclassnumber)
        {
            var newnum = lastNum + 1;

            if (Convert.ToInt32(masterclassnumber) < 10 && Convert.ToInt32(itemclassnumber) < 10 && newnum < 10)
            {
                this.CGJId = string.Format("0{0}-0{1}-00{2}", masterclassnumber, itemclassnumber, newnum);
            }
            else if (Convert.ToInt32(masterclassnumber) < 10 && Convert.ToInt32(itemclassnumber) < 10 && newnum < 100)
            {
                this.CGJId = string.Format("0{0}-0{1}-0{2}", masterclassnumber, itemclassnumber, newnum);
            }
            else if (Convert.ToInt32(masterclassnumber) < 10 && Convert.ToInt32(itemclassnumber) < 10 && newnum < 1000)
            {
                this.CGJId = string.Format("0{0}-0{1}-{2}", masterclassnumber, itemclassnumber, newnum);
            }
            else if (Convert.ToInt32(masterclassnumber) >= 10 && Convert.ToInt32(itemclassnumber) < 10 && newnum < 10)
            {
                this.CGJId = string.Format("{0}-0{1}-00{2}", masterclassnumber, itemclassnumber, newnum);
            }
            else if (Convert.ToInt32(masterclassnumber) >= 10 && Convert.ToInt32(itemclassnumber) < 10 && newnum < 100)
            {
                this.CGJId = string.Format("{0}-0{1}-0{2}", masterclassnumber, itemclassnumber, newnum);
            }
            else if (Convert.ToInt32(masterclassnumber) >= 10 && Convert.ToInt32(itemclassnumber) < 10 && newnum < 1000)
            {
                this.CGJId = string.Format("{0}-0{1}-{2}", masterclassnumber, itemclassnumber, newnum);
            }
            else if (Convert.ToInt32(masterclassnumber) < 10 && Convert.ToInt32(itemclassnumber) >= 10 && newnum < 10)
            {
                this.CGJId = string.Format("0{0}-{1}-00{2}", masterclassnumber, itemclassnumber, newnum);
            }
            else if (Convert.ToInt32(masterclassnumber) < 10 && Convert.ToInt32(itemclassnumber) >= 10 && newnum < 100)
            {
                this.CGJId = string.Format("0{0}-{1}-0{2}", masterclassnumber, itemclassnumber, newnum);
            }
            else if (Convert.ToInt32(masterclassnumber) < 10 && Convert.ToInt32(itemclassnumber) >= 10 && newnum < 1000)
            {
                this.CGJId = string.Format("0{0}-{1}-{2}", masterclassnumber, itemclassnumber, newnum);
            }
            else if (Convert.ToInt32(masterclassnumber) >= 10 && Convert.ToInt32(itemclassnumber) >= 10 && newnum < 10)
            {
                this.CGJId = string.Format("{0}-{1}-00{2}", masterclassnumber, itemclassnumber, newnum);
            }
            else if (Convert.ToInt32(masterclassnumber) >= 10 && Convert.ToInt32(itemclassnumber) >= 10 && newnum < 100)
            {
                this.CGJId = string.Format("{0}-{1}-0{2}", masterclassnumber, itemclassnumber, newnum);
            }
            else
            {
                this.CGJId = string.Format("{0}-{1}-{2}", masterclassnumber, itemclassnumber, newnum);
            }

        }

        public void SetCGJNumber(int lastNum, string masterclassnumber, string itemclassnumber)
        {
            int newnum = lastNum + 1;
            int master = Convert.ToInt32(masterclassnumber);
            int item = Convert.ToInt32(itemclassnumber);

            string formattedMaster = master < 10 ? $"0{master}" : master.ToString();
            string formattedItem = item < 10 ? $"0{item}" : item.ToString();
            string formattedNum = newnum < 10 ? $"00{newnum}" : newnum < 100 ? $"0{newnum}" : newnum.ToString();

            this.CGJId = $"{formattedMaster}-{formattedItem}-{formattedNum}";
        }

        public void SetCGJNumber_(int lastNumber, string classNumber)
        {
            int newNum = lastNumber + 1;
            int cNumber = Convert.ToInt32(classNumber);

            // masterclass padded to 6 digits
            string formattedNumber = cNumber.ToString("D6");

            // new number padded to 3 digits
            string formattedNum = newNum.ToString("D3");

            // Final CGJId: 40-000014-006
            this.CGJId = $"40-{formattedNumber}-{formattedNum}";
        }

        public void SetCGJNumber(int lastNumber, string classNumber)
        {
            int newNum = lastNumber + 1;

            // Extract the numeric part from classNumber
            string numericPart = classNumber.Contains("-")
                ? classNumber.Split('-')[1]
                : classNumber;

            int cNumber = Convert.ToInt32(numericPart);

            // masterclass padded to 6 digits
            string formattedNumber = cNumber.ToString("D6");

            // new number padded to 3 digits
            string formattedNum = newNum.ToString("D3");

            // Final CGJId: 40-000014-006
            this.CGJId = $"40-{formattedNumber}-{formattedNum}";
        }



    }
    public class CGJItemClasses
    {
        public List<CGJItemMasterClass> masters { get; set; }
        public List<CGJItemClass> subclasses { get; set; }
    }
    public class CGJItemMaintenaceVM
    {
        public int? CGJItemId { get; set; }
        public virtual CGJItem CGJItem { get; set; }
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
        public CGJItemMaintenaceVM()
        {

        }
        
        public CGJItemMaintenaceVM(CGJItem item)
        {
            this.CGJItemId = item.Id;
            this.CGJItem = item;
            this.Service_Maintenance_Freq = item.CGJItemClass.Service_Maintenance_Freq;
            this.Safety_Maintenance_Freq = item.CGJItemClass.Safety_Maintenance_Freq;
            this.Electrical_Maintenance_Freq = item.CGJItemClass.Electrical_Maintenance_Freq;
            //if(item.MaintenanceList.Count > 0) {
            //    var last_elec = item.MaintenanceList.Where(x => x.MaintenanceEntries.Where(y => y.MaintenanceSubTypeId.Equals(14) && y.MaintenanceSubTypeId.Equals(46)).Count() >= 1).LastOrDefault();
            //    //var last_elec = item.MaintenanceList.Where(x => x.MaintenanceEntriesMaintenanceTypeId.Equals(14) && x.MaintenanceSubTypeId.Equals(46)).LastOrDefault();
            //    if(last_elec != null)
            //    {
            //        this.Last_Electrical = last_elec.TimeStamp;
            //        this.Next_Electrical = Convert.ToDateTime(this.Last_Electrical).AddDays(Convert.ToInt32(this.Electrical_Maintenance_Freq));
            //    }
            //    var last_safety = item.MaintenanceList.Where(x => x.MaintenanceEntries.Where(y => y.MaintenanceSubTypeId.Equals(14) && y.MaintenanceSubTypeId.Equals(47)).Count() >= 1).LastOrDefault();
            //    //var last_safety = item.MaintenanceList.Where(x => x.MaintenanceTypeId.Equals(14) && x.MaintenanceSubTypeId.Equals(47)).LastOrDefault();
            //    if (last_safety != null)
            //    {
            //        this.Last_Safety = last_safety.TimeStamp;
            //        this.Next_Safety = Convert.ToDateTime(this.Last_Safety).AddDays(Convert.ToInt32(this.Safety_Maintenance_Freq));
            //    }
            //    var last_service = item.MaintenanceList.Where(x => x.MaintenanceEntries.Where(y => y.MaintenanceSubTypeId.Equals(14) && y.MaintenanceSubTypeId.Equals(45)).Count() >= 1).LastOrDefault();
            //    //var last_service = item.MaintenanceList.Where(x => x.MaintenanceTypeId.Equals(14) && x.MaintenanceSubTypeId.Equals(45)).LastOrDefault();
            //    if (last_service != null)
            //    {
            //        this.Last_Service = last_service.TimeStamp;
            //        this.Next_Service = Convert.ToDateTime(this.Last_Service).AddDays(Convert.ToInt32(this.Service_Maintenance_Freq));
            //    }
            //    if(last_elec != null && last_safety != null && last_service != null && this.Electrical_Maintenance_Freq > 0 && this.Service_Maintenance_Freq > 0 && this.Safety_Maintenance_Freq > 0)
            //    {
            //        if(this.Last_Electrical <= this.Last_Safety && this.Last_Electrical <= this.Last_Service)
            //        {
            //            this.Next_Check = Convert.ToDateTime(this.Next_Electrical);
            //        }
            //        else if (this.Last_Safety <= this.Last_Electrical && this.Last_Safety <= this.Last_Service)
            //        {
            //            this.Next_Check = Convert.ToDateTime(this.Next_Safety);
            //        }
            //        else
            //        {
            //            this.Next_Check = Convert.ToDateTime(this.Next_Service);
            //        }
            //    }
            //    else if (last_elec != null && last_safety != null && Last_Service == null)
            //    {
            //        if(this.Service_Maintenance_Freq > 0)
            //        {
            //            Next_Check = DateTime.Now;
            //        }
            //        else if (this.Last_Electrical <= this.Last_Safety)
            //        {
            //            this.Next_Check = Convert.ToDateTime(this.Next_Electrical);
            //        }
            //        else 
            //        {
            //            this.Next_Check = Convert.ToDateTime(this.Next_Safety);
            //        }
            //    }
            //    else if (last_elec != null && last_safety == null && Last_Service != null)
            //    {
            //        if (this.Safety_Maintenance_Freq > 0)
            //        {
            //            Next_Check = DateTime.Now;
            //        }
            //        else if (this.Last_Electrical <= this.Last_Service)
            //        {
            //            this.Next_Check = Convert.ToDateTime(this.Next_Electrical);
            //        }
            //        else
            //        {
            //            this.Next_Check = Convert.ToDateTime(this.Next_Service);
            //        }
            //    }
            //    else if (last_elec == null && last_safety != null && Last_Service != null)
            //    {
            //        if (this.Electrical_Maintenance_Freq > 0)
            //        {
            //            Next_Check = DateTime.Now;
            //        }
            //        else if (this.Last_Safety <= this.Last_Service)
            //        {
            //            this.Next_Check = Convert.ToDateTime(this.Next_Safety);
            //        }
            //        else
            //        {
            //            this.Next_Check = Convert.ToDateTime(this.Next_Service);
            //        }
            //    }
            //    else if(last_elec != null && last_safety == null && last_service == null)
            //    {
            //        if(this.Safety_Maintenance_Freq > 0 || this.Service_Maintenance_Freq > 0)
            //        {
            //            this.Next_Check = DateTime.Now;
            //        }
            //        else
            //        {
            //            this.Next_Check = Convert.ToDateTime(this.Next_Electrical);
            //        }
            //    }
            //    else if (last_elec == null && last_safety != null && last_service == null)
            //    {
            //        if (this.Electrical_Maintenance_Freq > 0 || this.Service_Maintenance_Freq > 0)
            //        {
            //            this.Next_Check = DateTime.Now;
            //        }
                    
            //        else
            //        {
            //            this.Next_Check = Convert.ToDateTime(this.Next_Safety);
            //        }
            //    }
            //    else if (last_elec == null && last_safety == null && last_service != null)
            //    {
            //        if (this.Electrical_Maintenance_Freq > 0 || this.Safety_Maintenance_Freq > 0)
            //        {
            //            this.Next_Check = DateTime.Now;
            //        }

            //        else
            //        {
            //            this.Next_Check = Convert.ToDateTime(this.Next_Service);
            //        }
            //    }
            //    else
            //    {
            //        if(this.Electrical_Maintenance_Freq > 0 || this.Safety_Maintenance_Freq > 0 || this.Service_Maintenance_Freq > 0)
            //        {
            //            this.Next_Check = DateTime.Now;
            //        }
            //    }
            //}
            //else
            //{
            //    this.Next_Check = DateTime.Now;
            //    this.Next_Service = DateTime.Now;
            //    this.Next_Electrical = DateTime.Now;
            //    this.Next_Safety = DateTime.Now;
            //}

        }
    }
    public class CGJItem_Locations_VM
    {
        public List<CGJItem_Location> Locations { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
    public class CGJItem_Location
    {
        [Key]
        public int Id { get; set; }
        public int? CGJItemId { get; set; }
        public virtual CGJItem CGJItem { get; set; }
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public int? SubProjectId { get; set; }
        public virtual SubProject SubProject { get; set; }
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
                            return ((end.Date - this.StartTime.Date).TotalDays + 1.0) * CGJItem.CGJItemClass.Internal_Rent;
                        }
                        else
                        {
                            return ((this.EndTime.Value.Date - this.StartTime.Date).TotalDays + 1.0) * CGJItem.CGJItemClass.Internal_Rent;
                        }
                    }
                    else
                    {
                        return ((end.Date - this.StartTime.Date).TotalDays + 1.0) * CGJItem.CGJItemClass.Internal_Rent;

                    }
                }
                else
                {
                    if(EndTime != null)
                    {
                        if(EndTime.Value.Date > end.Date)
                        {
                            return ((end.Date - start.Date).TotalDays + 1.0) * CGJItem.CGJItemClass.Internal_Rent;
                        }
                        else
                        {
                            return ((this.EndTime.Value.Date - start.Date).TotalDays + 1.0) * CGJItem.CGJItemClass.Internal_Rent;
                        }
                    }
                    else
                    {
                        return ((end.Date - start.Date).TotalDays +1.0) * CGJItem.CGJItemClass.Internal_Rent;

                    }
                }
            }
            else
            {
                return 0.0;
            }
        }
        public CGJItem_Location()
        {

        }      
           
    }
    
}
