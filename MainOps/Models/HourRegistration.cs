using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class HourRegistration
    {
        [Key]
        public int Id { get; set; }
        public string? userid { get; set; }
        public int? DivisionId { get; set; }
        public virtual Division? Division { get; set; }
        [Display(Name = "Week 1")]
        public int Week1 { get; set; }
        [Display(Name = "Week 2")]
        public int Week2 { get; set; }
        [Display(Name = "Full Name")]
        public string? FullName { get; set; }
        [Display(Name = "License Plate Nr Company Car")]
        public string? LicensePlate { get; set; }
        [Display(Name = "Payment Nr")]
        public string? PaymentNr { get; set; }
        [Display(Name = "Signature Employee")]
        public string? Signature_Worker { get; set; }
        [Display(Name ="Signature Supervisor")]
        public string? Signature_Supervisor { get; set; }
        [Display(Name = "Supervisor Name")]
        public string? Supervisor_Name { get; set; }
        [Display(Name = "Week-type")]
        public string? weektype { get; set; }
        [Display(Name = "Reg. hours 1201")]
        public double totalregularhours { get; set; }
        [Display(Name = "Overhours 50% 1202")]
        public double? totaloverhours50 { get; set; }
        [Display(Name = "Overhours 100% 1204")]
        public double? totaloverhours100 { get; set; }
        [Display(Name = "Total Addons")]
        public double? totaladdons { get; set; }
        public bool Edited { get; set; }
        public int? HourSheetYear { get; set; }
        public ICollection<RowHours>? Rows { get; set; }
        public HourRegistration()
        {

        }
        public HourRegistration(HourRegistrationVM mod)
        {
            this.Edited = true;
            this.userid = mod.HourRegistration.userid;
            this.DivisionId = mod.HourRegistration.DivisionId;
            this.FullName = mod.HourRegistration.FullName;
            this.PaymentNr = mod.HourRegistration.PaymentNr;
            this.LicensePlate = mod.HourRegistration.LicensePlate;
            this.Signature_Supervisor = mod.HourRegistration.Signature_Supervisor;
            this.Signature_Worker = mod.HourRegistration.Signature_Worker;
            this.totaloverhours100 = mod.HourRegistration.totaloverhours100;
            this.totaloverhours50 = mod.HourRegistration.totaloverhours50;
            this.totaladdons = mod.Rows.Sum(x => x.AddOns * x.AddOns_Amount);
            this.totalregularhours = mod.HourRegistration.totalregularhours;
            this.weektype = mod.HourRegistration.weektype;
            this.Week1 = mod.HourRegistration.Week1;
            this.Week2 = mod.HourRegistration.Week2;
            this.Supervisor_Name = mod.HourRegistration.Supervisor_Name;
            this.Rows = new List<RowHours>();
            this.HourSheetYear = mod.HourRegistration.HourSheetYear;
        }
    }
    public class HourRegistration_Ongoing
    {
        [Key]
        public int Id { get; set; }
        public int? DivisionId { get; set; }
        public virtual Division Division { get; set; }
        [Display(Name = "Week 1")]
        public int Week1 { get; set; }
        public string userid { get; set; }
        [Display(Name = "Week 2")]
        public int Week2 { get; set; }
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        [Display(Name = "License Plate Nr Company Car")]
        public string LicensePlate { get; set; }
        [Display(Name = "Payment Nr")]
        public string PaymentNr { get; set; }
        [Display(Name = "Signature Employee")]
        public string Signature_Worker { get; set; }
        [Display(Name = "Signature Supervisor")]
        public string Signature_Supervisor { get; set; }
        [Display(Name = "Supervisor Name")]
        public string Supervisor_Name { get; set; }
        [Display(Name = "Week-type")]
        public string weektype { get; set; }
        [Display(Name = "Reg. hours 1201")]
        public double totalregularhours { get; set; }
        [Display(Name = "Overhours 50% 1202")]
        public double? totaloverhours50 { get; set; }
        [Display(Name = "Overhours 100% 1204")]
        public double? totaloverhours100 { get; set; }
        [Display(Name = "Total Addons")]
        public double? totaladdons { get; set; }
        public bool Edited { get; set; }
        public int? HourSheetYear { get; set; }
        public ICollection<RowHours_Ongoing> Rows { get; set; }
        public HourRegistration_Ongoing()
        {

        }
        public HourRegistration_Ongoing(HourRegistrationVM mod)
        {
            this.FullName = mod.HourRegistration.FullName;
            this.userid = mod.HourRegistration.userid;
            this.DivisionId = mod.HourRegistration.DivisionId;
            this.PaymentNr = mod.HourRegistration.PaymentNr;
            this.LicensePlate = mod.HourRegistration.LicensePlate;
            this.Signature_Supervisor = mod.HourRegistration.Signature_Supervisor;
            this.Signature_Worker = mod.HourRegistration.Signature_Worker;
            this.totaloverhours100 = mod.HourRegistration.totaloverhours100;
            this.totaloverhours50 = mod.HourRegistration.totaloverhours50;
            this.totaladdons = mod.HourRegistration.Rows.Sum(x => x.AddOns * x.AddOns_Amount);
            this.totalregularhours = mod.HourRegistration.totalregularhours;
            this.weektype = mod.HourRegistration.weektype;
            this.Week1 = mod.HourRegistration.Week1;
            this.Week2 = mod.HourRegistration.Week2;
            this.Supervisor_Name = mod.HourRegistration.Supervisor_Name;
            this.HourSheetYear = mod.HourRegistration.HourSheetYear;
            this.Rows = new List<RowHours_Ongoing>();
        }
        public HourRegistration_Ongoing(HourRegistration mod,List<RowHours> Rows)
        {
            this.userid = mod.userid;
            this.FullName = mod.FullName;
            this.DivisionId = mod.DivisionId;
            this.PaymentNr = mod.PaymentNr;
            this.LicensePlate = mod.LicensePlate;
            this.Signature_Supervisor = mod.Signature_Supervisor;
            this.Signature_Worker = mod.Signature_Worker;
            this.totaloverhours100 = mod.totaloverhours100;
            this.totaloverhours50 = mod.totaloverhours50;
            this.totaladdons = Rows.Sum(x => x.AddOns * x.AddOns_Amount);
            this.totalregularhours = mod.totalregularhours;
            this.weektype = mod.weektype;
            this.Week1 = mod.Week1;
            this.Week2 = mod.Week2;
            this.HourSheetYear = mod.HourSheetYear;
            this.Supervisor_Name = mod.Supervisor_Name;
            this.Rows = new List<RowHours_Ongoing>();
        }
    }
    public class RowHours_Ongoing
    {
        public int Id { get; set; }
        public bool Edited { get; set; }
        [ForeignKey("HourRegistration_Ongoing")]
        public int? HourRegistration_OngoingId { get; set; }
        [Display(Name = "Mon")]
        [Range(0.0, 24.0)]
        public double? day1 { get; set; }
        [Display(Name = "Tue")]
        [Range(0.0, 24.0)]
        public double? day2 { get; set; }
        [Display(Name = "Wed")]
        [Range(0.0, 24.0)]
        public double? day3 { get; set; }
        [Display(Name = "Thu")]
        [Range(0.0, 24.0)]
        public double? day4 { get; set; }
        [Display(Name = "Fri")]
        [Range(0.0, 24.0)]
        public double? day5 { get; set; }
        [Display(Name = "Sat")]
        [Range(0.0, 24.0)]
        public double? day6 { get; set; }
        [Display(Name = "Sun")]
        [Range(0.0, 24.0)]
        public double? day7 { get; set; }
        [Display(Name = "Mon")]
        [Range(0.0, 24.0)]
        public double? day8 { get; set; }
        [Display(Name = "Tue")]
        [Range(0.0, 24.0)]
        public double? day9 { get; set; }
        [Display(Name = "Wed")]
        [Range(0.0, 24.0)]
        public double? day10 { get; set; }
        [Display(Name = "Thu")]
        [Range(0.0, 24.0)]
        public double? day11 { get; set; }
        [Display(Name = "Fri")]
        [Range(0.0, 24.0)]
        public double? day12 { get; set; }
        [Display(Name = "Sat")]
        [Range(0.0, 24.0)]
        public double? day13 { get; set; }
        [Display(Name = "Sun")]
        [Range(0.0, 24.0)]
        public double? day14 { get; set; }
        [Display(Name = "Mon Alarm")]
        [Range(4.0, 24.0)]
        public double? day1_Alarm { get; set; }
        [Display(Name = "Tue Alarm")]
        [Range(4.0, 24.0)]
        public double? day2_Alarm { get; set; }
        [Display(Name = "Wed Alarm")]
        [Range(4.0, 24.0)]
        public double? day3_Alarm { get; set; }
        [Display(Name = "Thur Alarm")]
        [Range(4.0, 24.0)]
        public double? day4_Alarm { get; set; }
        [Display(Name = "Fri Alarm")]
        [Range(4.0, 24.0)]
        public double? day5_Alarm { get; set; }
        [Display(Name = "Sat Alarm")]
        [Range(4.0, 24.0)]
        public double? day6_Alarm { get; set; }
        [Display(Name = "Sun Alarm")]
        [Range(4.0, 24.0)]
        public double? day7_Alarm { get; set; }
        [Display(Name = "Mon Alarm")]
        [Range(4.0, 24.0)]
        public double? day8_Alarm { get; set; }
        [Display(Name = "Tue Alarm")]
        [Range(4.0, 24.0)]
        public double? day9_Alarm { get; set; }
        [Display(Name = "Wed Alarm")]
        [Range(4.0, 24.0)]
        public double? day10_Alarm { get; set; }
        [Display(Name = "Thur Alarm")]
        [Range(4.0, 24.0)]
        public double? day11_Alarm { get; set; }
        [Display(Name = "Fri Alarm")]
        [Range(4.0, 24.0)]
        public double? day12_Alarm { get; set; }
        [Display(Name = "Sat Alarm")]
        [Range(4.0, 24.0)]
        public double? day13_Alarm { get; set; }
        [Range(4.0, 24.0)]
        [Display(Name = "Sun Alarm")]
        public double? day14_Alarm { get; set; }
        [DefaultValue("W")]
        public string day1_Type { get; set; }
        [DefaultValue("W")]
        public string day2_Type { get; set; }
        [DefaultValue("W")]
        public string day3_Type { get; set; }
        [DefaultValue("W")]
        public string day4_Type { get; set; }
        [DefaultValue("W")]
        public string day5_Type { get; set; }
        [DefaultValue("W")]
        public string day6_Type { get; set; }
        [DefaultValue("W")]
        public string day7_Type { get; set; }
        [DefaultValue("W")]
        public string day8_Type { get; set; }
        [DefaultValue("W")]
        public string day9_Type { get; set; }
        [DefaultValue("W")]
        public string day10_Type { get; set; }
        [DefaultValue("W")]
        public string day11_Type { get; set; }
        [DefaultValue("W")]
        public string day12_Type { get; set; }
        [DefaultValue("W")]
        public string day13_Type { get; set; }
        [DefaultValue("W")]
        public string day14_Type { get; set; }
        public double? day1314_Alarm { get; set; }
        [Display(Name = "Project")]
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        [Display(Name = "Others")]
        public string Others { get; set; }
        [Display(Name = "Over Hours 50%")]
        public double? OverHours_50 { get; set; }
        [Display(Name = "Over Hours 100%")]
        public double? OverHours_100 { get; set; }
        [Display(Name = "Addon Amount")]
        public double? AddOns { get; set; }
        [Display(Name = "Addon Rate")]
        public double? AddOns_Amount { get; set; }
        [Display(Name = "Allowance")]
        public double? Skur_penge { get; set; }
        [Display(Name = "Driving Days")]
        public int? Driving_Money_Days { get; set; }
        [Display(Name = "Mileage Rate")]
        public double? Driving_Money_Amount { get; set; }
        [Display(Name = "Hours")]
        public double Hours
        {
            get
            {
                return Convert.ToDouble(day1) +
                    Convert.ToDouble(day2) +
                    Convert.ToDouble(day3) +
                    Convert.ToDouble(day4) +
                    Convert.ToDouble(day5) +
                    Convert.ToDouble(day6) +
                    Convert.ToDouble(day7) +
                    Convert.ToDouble(day8) +
                    Convert.ToDouble(day9) +
                    Convert.ToDouble(day10) +
                    Convert.ToDouble(day11) +
                    Convert.ToDouble(day12) +
                    Convert.ToDouble(day13) +
                    Convert.ToDouble(day14);
            }
        }
        public RowHours_Ongoing()
        {

        }
        public RowHours_Ongoing(RowHours r)
        {
            this.AddOns = r.AddOns;
            this.AddOns_Amount = r.AddOns_Amount;
            this.day1 = r.day1;
            this.day2 = r.day2;
            this.day3 = r.day3;
            this.day4 = r.day4;
            this.day5 = r.day5;
            this.day6 = r.day6;
            this.day7 = r.day7;
            this.day8 = r.day8;
            this.day9 = r.day9;
            this.day10 = r.day10;
            this.day11 = r.day11;
            this.day12 = r.day12;
            this.day13 = r.day13;
            this.day14 = r.day14;
            this.day1_Alarm = r.day1_Alarm;
            this.day2_Alarm = r.day2_Alarm;
            this.day3_Alarm = r.day3_Alarm;
            this.day4_Alarm = r.day4_Alarm;
            this.day5_Alarm = r.day5_Alarm;
            this.day6_Alarm = r.day6_Alarm;
            this.day7_Alarm = r.day7_Alarm;
            this.day8_Alarm = r.day8_Alarm;
            this.day9_Alarm = r.day9_Alarm;
            this.day10_Alarm = r.day10_Alarm;
            this.day11_Alarm = r.day11_Alarm;
            this.day12_Alarm = r.day12_Alarm;
            this.day13_Alarm = r.day13_Alarm;
            this.day14_Alarm = r.day14_Alarm;
            this.day1_Type = r.day1_Type;
            this.day2_Type = r.day2_Type;
            this.day3_Type = r.day3_Type;
            this.day4_Type = r.day4_Type;
            this.day5_Type = r.day5_Type;
            this.day6_Type = r.day6_Type;
            this.day7_Type = r.day7_Type;
            this.day8_Type = r.day8_Type;
            this.day9_Type = r.day9_Type;
            this.day10_Type = r.day10_Type;
            this.day11_Type = r.day11_Type;
            this.day12_Type = r.day12_Type;
            this.day13_Type = r.day13_Type;
            this.day14_Type = r.day14_Type;
            this.Driving_Money_Amount = r.Driving_Money_Amount;
            this.Driving_Money_Days = r.Driving_Money_Days;
            this.Edited = r.Edited;
            this.Others = r.Others;
            this.OverHours_100 = r.OverHours_100;
            this.OverHours_50 = r.OverHours_50;
            this.Skur_penge = r.Skur_penge;
            this.ProjectId = r.ProjectId;
        }
    }
    public class RowHours
    {
        public int Id { get; set; }
        public bool Edited { get; set; }
        [ForeignKey("HourRegistration")]
        public int? HourRegistrationId { get; set; }
        [Display(Name = "Mon")]
        [Range(0.0, 24.0)]
        public double? day1 { get; set; }
        [Display(Name = "Tue")]
        [Range(0.0, 24.0)]
        public double? day2 { get; set; }
        [Display(Name = "Wed")]
        [Range(0.0, 24.0)]
        public double? day3 { get; set; }
        [Display(Name = "Thu")]
        [Range(0.0, 24.0)]
        public double? day4 { get; set; }
        [Display(Name = "Fri")]
        [Range(0.0, 24.0)]
        public double? day5 { get; set; }
        [Display(Name = "Sat")]
        [Range(0.0, 24.0)]
        public double? day6 { get; set; }
        [Display(Name = "Sun")]
        [Range(0.0, 24.0)]
        public double? day7 { get; set; }
        [Display(Name = "Mon")]
        [Range(0.0, 24.0)]
        public double? day8 { get; set; }
        [Display(Name = "Tue")]
        [Range(0.0, 24.0)]
        public double? day9 { get; set; }
        [Display(Name = "Wed")]
        [Range(0.0, 24.0)]
        public double? day10 { get; set; }
        [Display(Name = "Thu")]
        [Range(0.0, 24.0)]
        public double? day11 { get; set; }
        [Display(Name = "Fri")]
        [Range(0.0, 24.0)]
        public double? day12 { get; set; }
        [Display(Name = "Sat")]
        [Range(0.0, 24.0)]
        public double? day13 { get; set; }
        [Display(Name = "Sun")]
        [Range(0.0, 24.0)]
        public double? day14 { get; set; }
        [Display(Name = "Mon Alarm")]
        [Range(4.0, 24.0)]
        public double? day1_Alarm { get; set; }
        [Display(Name = "Tue Alarm")]
        [Range(4.0, 24.0)]
        public double? day2_Alarm { get; set; }
        [Display(Name = "Wed Alarm")]
        [Range(4.0, 24.0)]
        public double? day3_Alarm { get; set; }
        [Display(Name = "Thur Alarm")]
        [Range(4.0, 24.0)]
        public double? day4_Alarm { get; set; }
        [Display(Name = "Fri Alarm")]
        [Range(4.0, 24.0)]
        public double? day5_Alarm { get; set; }
        [Display(Name = "Sat Alarm")]
        [Range(4.0, 24.0)]
        public double? day6_Alarm { get; set; }
        [Display(Name = "Sun Alarm")]
        [Range(4.0, 24.0)]
        public double? day7_Alarm { get; set; }
        [Display(Name = "Mon Alarm")]
        [Range(4.0, 24.0)]
        public double? day8_Alarm { get; set; }
        [Display(Name = "Tue Alarm")]
        [Range(4.0, 24.0)]
        public double? day9_Alarm { get; set; }
        [Display(Name = "Wed Alarm")]
        [Range(4.0, 24.0)]
        public double? day10_Alarm { get; set; }
        [Display(Name = "Thur Alarm")]
        [Range(4.0, 24.0)]
        public double? day11_Alarm { get; set; }
        [Display(Name = "Fri Alarm")]
        [Range(4.0, 24.0)]
        public double? day12_Alarm { get; set; }
        [Display(Name = "Sat Alarm")]
        [Range(4.0, 24.0)]
        public double? day13_Alarm { get; set; }
        [Range(4.0, 24.0)]
        [Display(Name = "Sun Alarm")]
        public double? day14_Alarm { get; set; }
        [DefaultValue("W")]
        public string day1_Type { get; set; }
        [DefaultValue("W")]
        public string day2_Type { get; set; }
        [DefaultValue("W")]
        public string day3_Type { get; set; }
        [DefaultValue("W")]
        public string day4_Type { get; set; }
        [DefaultValue("W")]
        public string day5_Type { get; set; }
        [DefaultValue("W")]
        public string day6_Type { get; set; }
        [DefaultValue("W")]
        public string day7_Type { get; set; }
        [DefaultValue("W")]
        public string day8_Type { get; set; }
        [DefaultValue("W")]
        public string day9_Type { get; set; }
        [DefaultValue("W")]
        public string day10_Type { get; set; }
        [DefaultValue("W")]
        public string day11_Type { get; set; }
        [DefaultValue("W")]
        public string day12_Type { get; set; }
        [DefaultValue("W")]
        public string day13_Type { get; set; }
        [DefaultValue("W")]
        public string day14_Type { get; set; }
        public double? day1314_Alarm { get; set; }
        [Display(Name = "Project")]
        public int? ProjectId { get; set; }
        public virtual Project? Project { get; set; }
        [Display(Name = "Others")]
        public string Others { get; set; }
        [Display(Name = "Over Hours 50%")]
        public double? OverHours_50 { get; set; }
        [Display(Name = "Over Hours 100%")]
        public double? OverHours_100 { get; set; }
        [Display(Name = "Addon Amount")]
        public double? AddOns { get; set; }
        [Display(Name = "Addon Rate")]
        public double? AddOns_Amount { get; set; }
        [Display(Name = "Allowance")]
        public double? Skur_penge { get; set; }
        [Display(Name = "Driving Days")]
        public int? Driving_Money_Days { get; set; }
        [Display(Name = "Mileage Rate")]
        public double? Driving_Money_Amount { get; set; }       
        [Display(Name = "Hours")]
        public double Hours
        {
            get
            {
                return Convert.ToDouble(day1) +
                    Convert.ToDouble(day2) +
                    Convert.ToDouble(day3) +
                    Convert.ToDouble(day4) +
                    Convert.ToDouble(day5) +
                    Convert.ToDouble(day6) +
                    Convert.ToDouble(day7) +
                    Convert.ToDouble(day8) +
                    Convert.ToDouble(day9) +
                    Convert.ToDouble(day10) +
                    Convert.ToDouble(day11) +
                    Convert.ToDouble(day12) +
                    Convert.ToDouble(day13) +
                    Convert.ToDouble(day14);
            }
        }
        public double? Hours_Alarm
        {
            get
            {
                return Convert.ToDouble(day1_Alarm) +
                    Convert.ToDouble(day2_Alarm) +
                    Convert.ToDouble(day3_Alarm) +
                    Convert.ToDouble(day4_Alarm) +
                    Convert.ToDouble(day5_Alarm) +
                    Convert.ToDouble(day6_Alarm) +
                    Convert.ToDouble(day7_Alarm) +
                    Convert.ToDouble(day8_Alarm) +
                    Convert.ToDouble(day9_Alarm) +
                    Convert.ToDouble(day10_Alarm) +
                    Convert.ToDouble(day11_Alarm) +
                    Convert.ToDouble(day12_Alarm) +
                    Convert.ToDouble(day13_Alarm) +
                    Convert.ToDouble(day14_Alarm);
            }
        }
        public RowHours()
        {

        }
        public RowHours(RowHours mod, int newid)
        {
            this.AddOns = mod.AddOns;
            this.AddOns_Amount = mod.AddOns_Amount;
            this.day1 = mod.day1;
            this.day1_Alarm = mod.day1_Alarm;
            this.day1_Type = mod.day1_Type;
            this.day2 = mod.day2;
            this.day2_Alarm = mod.day2_Alarm;
            this.day2_Type = mod.day2_Type;
            this.day3 = mod.day3;
            this.day3_Alarm = mod.day3_Alarm;
            this.day3_Type = mod.day3_Type;
            this.day4 = mod.day4;
            this.day4_Alarm = mod.day4_Alarm;
            this.day4_Type = mod.day4_Type;
            this.day5 = mod.day5;
            this.day5_Alarm = mod.day5_Alarm;
            this.day5_Type = mod.day5_Type;
            this.day6 = mod.day6;
            this.day6_Alarm = mod.day6_Alarm;
            this.day6_Type = mod.day6_Type;
            this.day7 = mod.day7;
            this.day7_Alarm = mod.day7_Alarm;
            this.day7_Type = mod.day7_Type;
            this.day8 = mod.day8;
            this.day8_Alarm = mod.day8_Alarm;
            this.day8_Type = mod.day8_Type;
            this.day9 = mod.day9;
            this.day9_Alarm = mod.day9_Alarm;
            this.day9_Type = mod.day9_Type;
            this.day10 = mod.day10;
            this.day10_Alarm = mod.day10_Alarm;
            this.day10_Type = mod.day10_Type;
            this.day11 = mod.day11;
            this.day11_Alarm = mod.day11_Alarm;
            this.day11_Type = mod.day11_Type;
            this.day12 = mod.day12;
            this.day12_Alarm = mod.day12_Alarm;
            this.day12_Type = mod.day12_Type;
            this.day13 = mod.day13;
            this.day13_Alarm = mod.day13_Alarm;
            this.day13_Type = mod.day13_Type;
            this.day14 = mod.day14;
            this.day14_Alarm = mod.day14_Alarm;
            this.day14_Type = mod.day14_Type;
            this.Driving_Money_Amount = mod.Driving_Money_Amount;
            this.Driving_Money_Days = mod.Driving_Money_Days;
            this.OverHours_100 = mod.OverHours_100;
            this.OverHours_50 = mod.OverHours_50;
            this.ProjectId = mod.ProjectId;
            this.Skur_penge = mod.Skur_penge;
            this.Others = mod.Others;
            this.HourRegistrationId = newid;
            this.Edited = true;
        }
        public RowHours(RowHours_Ongoing mod)
        {
            this.AddOns = mod.AddOns;
            this.AddOns_Amount = mod.AddOns_Amount;
            this.day1 = mod.day1;
            this.day1_Alarm = mod.day1_Alarm;
            this.day1_Type = mod.day1_Type;
            this.day2 = mod.day2;
            this.day2_Alarm = mod.day2_Alarm;
            this.day2_Type = mod.day2_Type;
            this.day3 = mod.day3;
            this.day3_Alarm = mod.day3_Alarm;
            this.day3_Type = mod.day3_Type;
            this.day4 = mod.day4;
            this.day4_Alarm = mod.day4_Alarm;
            this.day4_Type = mod.day4_Type;
            this.day5 = mod.day5;
            this.day5_Alarm = mod.day5_Alarm;
            this.day5_Type = mod.day5_Type;
            this.day6 = mod.day6;
            this.day6_Alarm = mod.day6_Alarm;
            this.day6_Type = mod.day6_Type;
            this.day7 = mod.day7;
            this.day7_Alarm = mod.day7_Alarm;
            this.day7_Type = mod.day7_Type;
            this.day8 = mod.day8;
            this.day8_Alarm = mod.day8_Alarm;
            this.day8_Type = mod.day8_Type;
            this.day9 = mod.day9;
            this.day9_Alarm = mod.day9_Alarm;
            this.day9_Type = mod.day9_Type;
            this.day10 = mod.day10;
            this.day10_Alarm = mod.day10_Alarm;
            this.day10_Type = mod.day10_Type;
            this.day11 = mod.day11;
            this.day11_Alarm = mod.day11_Alarm;
            this.day11_Type = mod.day11_Type;
            this.day12 = mod.day12;
            this.day12_Alarm = mod.day12_Alarm;
            this.day12_Type = mod.day12_Type;
            this.day13 = mod.day13;
            this.day13_Alarm = mod.day13_Alarm;
            this.day13_Type = mod.day13_Type;
            this.day14 = mod.day14;
            this.day14_Alarm = mod.day14_Alarm;
            this.day14_Type = mod.day14_Type;
            this.Driving_Money_Amount = mod.Driving_Money_Amount;
            this.Driving_Money_Days = mod.Driving_Money_Days;
            this.OverHours_100 = mod.OverHours_100;
            this.OverHours_50 = mod.OverHours_50;
            this.ProjectId = mod.ProjectId;
            this.Skur_penge = mod.Skur_penge;
            this.Others = mod.Others;
            this.Edited = mod.Edited;
        }
    }
    public class HourRegistrationVM
    {
        public HourRegistration HourRegistration { get; set; }
        public List<RowHours> Rows { get; set; }
        public HourRegistrationVM()
        {
            Rows = new List<RowHours>(14);
            HourRegistration = new HourRegistration();
        }
        public HourRegistrationVM(ApplicationUser user)
        {
            Rows = new List<RowHours>(10);
            for(int i = 1; i<= 10; i++)
            {
                Rows.Add(new RowHours());
            }
            HourRegistration = new HourRegistration();
            HourRegistration.FullName = user.full_name();
            HourRegistration.DivisionId = user.DivisionId;
            HourRegistration.userid = user.Id;
        }
        public HourRegistrationVM(HourRegistration hr)
        {
            
            var addonamount = hr.Rows.Where(x => x.AddOns > 5 && x.AddOns_Amount < 1000).LastOrDefault();
            if(addonamount != null)
            {
                double theamount = Convert.ToDouble(addonamount.AddOns_Amount);
                foreach (RowHours r in hr.Rows.Where(x => x.Hours_Alarm > 0))
                {
                    hr.totaladdons += r.Hours_Alarm * theamount;
                }
            }
            Rows = hr.Rows.ToList();
            HourRegistration = hr;

        }
        public HourRegistrationVM(HourRegistration hr,bool addrows)
        {

            var addonamount = hr.Rows.Where(x => x.AddOns > 5 && x.AddOns_Amount < 1000).LastOrDefault();
            if (addonamount != null)
            {
                double theamount = Convert.ToDouble(addonamount.AddOns_Amount);
                foreach (RowHours r in hr.Rows.Where(x => x.Hours_Alarm > 0))
                {
                    hr.totaladdons += r.Hours_Alarm * theamount;
                }
            }
            this.Rows = new List<RowHours>(10);
            int i = 0;
            foreach(var r in hr.Rows)
            {
                this.Rows.Add(r);
                i += 1;
            }
            if(addrows == true)
            {
                for (int j = i; j <= 10; j++)
                {
                    this.Rows.Add(new RowHours());
                }
            }
            HourRegistration = hr;

        }
        public HourRegistrationVM(HourRegistration_Ongoing ongoing)
        {
            this.HourRegistration = new HourRegistration();
            this.Rows = new List<RowHours>();
            this.HourRegistration.userid = ongoing.userid;
            this.HourRegistration.DivisionId = ongoing.DivisionId;
            this.HourRegistration.LicensePlate = ongoing.LicensePlate;
            this.HourRegistration.PaymentNr = ongoing.PaymentNr;
            this.HourRegistration.Signature_Supervisor = ongoing.Signature_Supervisor;
            this.HourRegistration.Signature_Worker = ongoing.Signature_Worker;
            this.HourRegistration.Supervisor_Name = ongoing.Supervisor_Name;
            this.HourRegistration.totaladdons = ongoing.totaladdons;
            this.HourRegistration.totaloverhours100 = ongoing.totaloverhours100;
            this.HourRegistration.totaloverhours50 = ongoing.totaloverhours50;
            this.HourRegistration.totalregularhours = ongoing.totalregularhours;
            this.HourRegistration.Week1 = ongoing.Week1;
            this.HourRegistration.Week2 = ongoing.Week2;
            this.HourRegistration.weektype = ongoing.weektype;
            this.HourRegistration.FullName = ongoing.FullName;
            this.HourRegistration.Edited = ongoing.Edited;
            this.HourRegistration.HourSheetYear = ongoing.HourSheetYear;
            this.Rows = new List<RowHours>();
            foreach(RowHours_Ongoing r in ongoing.Rows)
            {
                RowHours r_new = new RowHours(r);
                this.Rows.Add(r_new);
            }
        }
    }

}
