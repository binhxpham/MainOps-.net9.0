using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ViewModels
{
    public class ThreeStepTestVM
    {
        [Display(Name = "Project")]
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        [Display(Name = "Sub Project")]
        [ForeignKey("SubProject")]
        public int? SubProjectId { get; set; }
        [Display(Name = "Well")]
        public string Wellname { get; set; }
        [Display(Name = "Known Well")]
        [ForeignKey("MeasPoint")]
        public int? MeasPointId { get; set; }
        [Display(Name = "Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/M/yy}")]
        public DateTime Report_Date { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        [Display(Name = "Start Time")]
        public TimeSpan starttime { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        [Display(Name = "End Time")]
        public TimeSpan endtime { get; set; }
        [Display(Name = "Water Meter Before")]
        public double? Water_Meter_Before { get; set; }
        [Display(Name = "Water Meter After")]
        public double? Water_Meter_After { get; set; }
        [Display(Name = "Reference Level")]
        public double? Ref_Level { get; set; }
        [Display(Name = "Bottom Well")]
        public double? Bottom_well { get; set; }
        [Display(Name = "Water Level")]
        public double? Water_level { get; set; }
        [Display(Name = "Time Initial Dip")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        public TimeSpan? Init_Meas_Time { get; set; }
        public string imagepath { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        [Display(Name = "Test Type")]
        public string TestType { get; set; }
        [Display(Name = "Total m3")]
        public double Totalm3
        {
            get
            {
                if(this.Water_Meter_Before != null && this.Water_Meter_After != null)
                {
                    return Convert.ToDouble(this.Water_Meter_After) - Convert.ToDouble(this.Water_Meter_Before);
                }
                else if(this.Water_Meter_After != null)
                {
                    return Convert.ToDouble(this.Water_Meter_After);
                }
                else
                {
                    return 0.0;
                }
            }
        }
        public ThreeStepTestVM()
        {

        }
    }
    public class ClearPumpTestReport
    {
        public ClearPumpTest test { get; set; }
        public List<ClearPumpTestData> datas_dips { get; set; }
        public List<ClearPumpTestPhoto> Photos { get; set; }
        public List<ClearPumpTestDataDevice> datas_device { get; set; }
        public string imagepath { get; set; }
        public ClearPumpTestReport()
        {
            datas_dips = new List<ClearPumpTestData>();
            Photos = new List<ClearPumpTestPhoto>();
            datas_device = new List<ClearPumpTestDataDevice>();
        }
        public ClearPumpTestReport(ClearPumpTest test, List<ClearPumpTestData> datas_dips, List<ClearPumpTestDataDevice> datas_device, List<ClearPumpTestPhoto> fotos)
        {
            this.datas_dips = datas_dips;
            this.test = test;
            this.datas_device = datas_device;
            this.Photos = fotos;
        }
    }
    public class ThreeStepTestReport
    {
        public ThreeStepTest test { get; set; }
        public List<PumpTestData> datas_dips { get; set; }
        public List<PumpTestDataDevice> datas_device { get; set; }
        public List<PumpTestPhoto> Photos { get; set; }
        public double radius = 0.125;
        public string imagepath { get; set; }
        [Display(Name = "Q_max (m3/h)")]
        public double Q_max_m3_per_hour {
            get
            {
                try
                {
                    int index = this.datas_device.IndexOf(this.datas_device.OrderByDescending(x => x.TimeStamp).First(x => x.FlowData > 0));
                    DateTime timeofstop = this.datas_device[index].TimeStamp;
                    var theelement = this.datas_device.OrderByDescending(x => x.TimeStamp).Where(x => x.TimeStamp < timeofstop).FirstOrDefault();
                    var lastelements = this.datas_device.OrderByDescending(x => x.TimeStamp).Where(x => x.TimeStamp >= theelement.TimeStamp.AddSeconds(-60)).ToList();
                    return Convert.ToDouble(lastelements.Max(x => x.FlowData));
                    //return Convert.ToDouble(theelement.FlowData);
                }
                catch
                {
                    return 0.0;
                }
               
            }
        }
        [Display(Name = "Q_max (m3/s)")]
        public double Q_max_m3_per_second
        {
            get
            {
                try
                {
                    int index = this.datas_device.IndexOf(this.datas_device.OrderByDescending(x => x.TimeStamp).First(x => x.FlowData > 0));
                    DateTime timeofstop = this.datas_device[index].TimeStamp;
                    var theelement = this.datas_device.OrderByDescending(x => x.TimeStamp).Where(x => x.TimeStamp < timeofstop).First();
                    //var lastelements = this.datas_device.OrderByDescending(x => x.TimeStamp).Where(x => x.Id >= theelement.Id - 10).ToList();
                    var lastelements = this.datas_device.OrderByDescending(x => x.TimeStamp).Where(x => x.TimeStamp >= theelement.TimeStamp.AddSeconds(-60)).ToList();
                    return Convert.ToDouble(lastelements.Max(x => x.FlowData) / 3600.0);
                    //return Convert.ToDouble(theelement.FlowData) / 3600.0;
                }
                catch
                {
                    return 0.0;
                }
               
            }
        }
        [Display(Name = "Maximum Drawdown")]
        public double Max_Drawdown
        {
            get
            {
                try
                {
                    int index = this.datas_device.IndexOf(this.datas_device.OrderByDescending(x => x.TimeStamp).First(x => x.FlowData > 0)); //find first flow = 0
                    DateTime timeofstop = this.datas_device[index].TimeStamp;
                    var theelement = this.datas_device.OrderByDescending(x => x.TimeStamp).Where(x => x.TimeStamp < timeofstop).First();
                    var lastelements = this.datas_device.Where(x => x.TimeStamp >= theelement.TimeStamp.AddSeconds(-60)).ToList();
                    var elem = lastelements.OrderByDescending(x => x.FlowData).First();
                    return Convert.ToDouble(elem.PumpLevelData);
                }
                catch
                {
                    return 0.0;
                }

            }
        }
        [Display(Name = "Drawdown after 6 minutes stop")]
        public double m_Drawdown_6
        {
            get
            {
                try
                {
                    int index = this.datas_device.IndexOf(this.datas_device.OrderByDescending(x => x.TimeStamp).First(x => x.FlowData > 0));
                    DateTime timeofstop = this.datas_device[index].TimeStamp;
                    var theelement = this.datas_device.OrderBy(x => x.TimeStamp).Where(x => x.TimeStamp >= timeofstop.AddMinutes(6)).First();
                    return Convert.ToDouble(theelement.PumpLevelData);
                }
                catch{
                    return 0.0;
                }
               
            }
        }
        [Display(Name = "Drawdown after 60 minutes stop")]
        public double m_Drawdown_60
        {
            get
            {
                try { 
                int index = this.datas_device.IndexOf(this.datas_device.OrderByDescending(x => x.TimeStamp).First(x => x.FlowData > 0));
                DateTime timeofstop = this.datas_device[index].TimeStamp;
                var theelement = this.datas_device.OrderBy(x => x.TimeStamp).Where(x => x.TimeStamp >= timeofstop.AddHours(1)).FirstOrDefault();
                if (theelement == null)
                {
                    theelement = this.datas_device.OrderBy(x => x.TimeStamp).Where(x => x.TimeStamp >= timeofstop).Last();
                }
                return Convert.ToDouble(theelement.PumpLevelData);
                }
                catch
                {
                    return 0.0;
                }
            }
        }
        [Display(Name = "Difference max drawdown / drawdown after 60min")]
        public double s_60
        {
            get
            {
                try
                {
                    return this.m_Drawdown_60 - this.Max_Drawdown;
                }
                catch
                {
                    return 0.0;
                }
               
            }
        }
        [Display(Name = "s60 theoretical")]
        public double s_60_theo
        {
            get
            {
                try
                {
                    return 2.303 * this.Q_max_m3_per_second / (4.0 * Math.PI * this.Transmissivity) * Math.Log10(2.25 * this.Transmissivity * 3600.0 / (Math.Pow(this.radius, 2) * 0.0003));
                }
                catch
                {
                    return 0.0;
                }
            }
        }
        [Display(Name = "Specific Capacity")]
        public double specific_capacity
        {
            get
            {
                try
                {
                    return this.Q_max_m3_per_hour / this.s_60;
                }
                catch
                {
                    return 0.0;
                }

                
            }
        }
        [Display(Name = "Delta s (over decade)")]
        public double delta_s
        {
            get
            {
                try
                {
                    return this.m_Drawdown_60 - this.m_Drawdown_6;
                }
                catch
                {
                    return 0.0;
                }

                
            }
        }
        public double Transmissivity
        {
            get
            {
                try
                {
                    return 0.18303 * this.Q_max_m3_per_second / this.delta_s;
                }
                catch
                {
                    return 0.0;
                }
                    
                
            }
        }
        [Display(Name = "Efficiency/Virkningsgrad")]
        public double Efficiency
        {
            get
            {
                try {
                    return this.s_60_theo / s_60 * 100.0;
                //double lhs = this.Q_max_m3_per_second / s_60;
                //double rhs = 4 * Math.PI * this.Transmissivity
                //    / (2.30 * Math.Log(2.25 * this.Transmissivity * 3600
                //    / (Math.Pow(this.radius, 2) * 3 * Math.Pow(10, -4))));
                ////return 100.0 - ((lhs - rhs) / lhs * 100.0);
                //return lhs / rhs * 100;
                    //double s = 2.30 * this.Q_max_m3_per_second / (4 * Math.PI * this.Transmissivity) * Math.Log(2.25 * this.Transmissivity * 3600 / (Math.Pow(this.radius, 2) * 3 * Math.Pow(10, -4)));
                    //return s / this.s_60 * 100.0;
                }
                catch
                {
                    return 0.0;
                }
            }
        }

        [Display(Name = "Total m3")]
        public double Totalm3
        {
            get
            {
                if (this.test.Water_Meter_Before != null && this.test.Water_Meter_After != null)
                {
                    return Convert.ToDouble(this.test.Water_Meter_After) - Convert.ToDouble(this.test.Water_Meter_Before);
                }
                else if (this.test.Water_Meter_After != null)
                {
                    return Convert.ToDouble(this.test.Water_Meter_After);
                }
                else
                {
                    return 0.0;
                }
            }
        }
        public ThreeStepTestReport()
        {
            datas_dips = new List<PumpTestData>();
            datas_device = new List<PumpTestDataDevice>();
            Photos = new List<PumpTestPhoto>();
        }
        public ThreeStepTestReport(ThreeStepTest test,List<PumpTestData> datas_dips,List<PumpTestDataDevice> datas_device, List<PumpTestPhoto> fotos)
        {
            this.datas_dips = datas_dips;
            this.datas_device = datas_device;
            this.test = test;
            this.Photos = fotos;
        }
    }
}
