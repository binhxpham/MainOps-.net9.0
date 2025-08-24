using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ReportClasses
{
    public class Generator_Test
    {
        [Key]
        public int Id { get; set; }
        public string Test_name
        {
            get
            {
                return "Emergency Generator Test";
            }
        }
        public string DocId
        {
            get
            {
                return "CT-TP-28";
            }
        }
        public string VersionNr
        {
            get
            {
                return "0.0a";
            }
        }
        [Display(Name = "Equipment Name")]
        public string EquipmentName
        {
            get
            {
                return "Emergency Generator";
            }
        }
        [Display(Name = "Type kWa")]
        public string Type { get; set; }
        [Display(Name = "Manufacturer")]
        public string Manufacturer { get; set; }
        [Display(Name = "HD Equipment id")]
        public string HD_Equipment_Id { get; set; }
        [Display(Name = "Site Manager Name")]
        public string Site_Manager_Name { get; set; }
        [Display(Name = "Project")]
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        [Display(Name = "Location")]
        public string Location { get; set; }
        [Display(Name = "Oil Level checked?")]
        public bool Oil_level_check { get; set; }
        [Display(Name = "Fuel Level %")]
        [Range(0, 100, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public double Fuel_level { get; set; }
        [Display(Name = "Fuel Level checked?")]
        public bool Fuel_level_check { get; set; }
        [Display(Name = "Battery Level %")]
        [Range(0, 100, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public double Battery_level { get; set; }
        [Display(Name = "Battery checked?")]
        public bool Battery_status { get; set; }
        [Display(Name = "Heating checked?")]
        public bool Heating_check { get; set; }
        [Display(Name = "Cooling checked?")]
        public bool Cooling_check { get; set; }
        [Display(Name = "Cut power supply during test?")]
        public bool Cut_power_supply_during_test { get; set; }
        [Display(Name = "Generator running for 1 hour?")]
        public bool Generator_Run_1_Hour { get; set; }
        [Display(Name = "Load current fully overtaken ok?")]
        public bool Load_current_fully_overtaken { get; set; }
        [Display(Name = "Time delayed starting of pumps ok?")]
        public bool Time_delayed_starting_of_pumps { get; set; }
        [Display(Name = "Switching off generator ok?")]
        public bool Switching_off_generator_ok { get; set; }
        [Display(Name = "Received alarms?")]
        public bool received_alarm { get; set; }
        [Display(Name = "Hours of Operation")]
        public double Hours_of_Operation { get; set; }
        [Display(Name = "Special events")]
        public string Special_events { get; set; }
        [Display(Name = "Date")]
        public DateTime Date_Done { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
    }
}
