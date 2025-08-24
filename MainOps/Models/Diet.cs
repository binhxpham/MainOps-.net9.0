using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class Diet
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Full Name")]
        [Required]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "Must be filled out")]
        public string? FullName { get; set; }
        [Display(Name = "Employee Number")]
        [Required]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "Must be filled out")]
        public string? EmployeeNumber { get; set; }
        [Display(Name = "Pay Period")]
        [Required]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "Must be filled out")]
        public string? PayPeriod { get; set; }
        public bool HourlyPaid { get; set; }
        [Display(Name = "Project Number")]
        public int ProjectId { get; set; }
        public virtual Project? Project { get; set; }
        public string? UserId { get; set; }
        public DateTime? Day1_start { get; set; }
        public DateTime? Day2_start { get; set; }
        public DateTime? Day3_start { get; set; }
        public DateTime? Day4_start { get; set; }
        public DateTime? Day5_start { get; set; }
        public DateTime? Day6_start { get; set; }
        public DateTime? Day1_end { get; set; }
        public DateTime? Day2_end { get; set; }
        public DateTime? Day3_end { get; set; }
        public DateTime? Day4_end { get; set; }
        public DateTime? Day5_end { get; set; }
        public DateTime? Day6_end { get; set; }
        public string? WorkPlaceName1 { get; set; }
        public string? WorkPlaceName2 { get; set; }
        public string? WorkPlaceName3 { get; set; }
        public string? WorkPlaceName4 { get; set; }
        public string? WorkPlaceName5 { get; set; }
        public string? WorkPlaceName6 { get; set; }
        [Display(Name = "I,my self, account for all expenses")]
        public decimal? SelfContainedExpenses { get; set; }
        [Display(Name = "I live in a trailer on site")]
        public decimal? LivingInCamperWagon { get; set; }
        [Display(Name = "Calculation of diets and small necesseties")]
        public decimal? CalculationOfDietsAndSmallNecessities { get; set; }
        [Display(Name = "Additional hours")]
        public decimal? HourAddon { get; set; }
        [Display(Name = "- Breakfast 15%")]
        public decimal? DeductionBreakFast { get; set; }
        [Display(Name = "- Lunch 30%")]
        public decimal? DeductionLunch { get; set; }
        [Display(Name = "- Dinner 30%")]
        public decimal? DeductionDinner { get; set; }
        public decimal? SelfContainedExpensesRate { get { return (decimal)238.00; } }
        public decimal? LivingInCamperWagonRate { get { return (decimal)173.00; } }
        public decimal? CalculationOfDietsAndSmallNecessitiesRate { get { return (decimal)555.00; } }
        public decimal? HourAddonRate { get { return (decimal)23.13; } }
        public decimal? DeductionBreakFastRate { get { return (decimal)83.85; } }
        public decimal? DeductionLunchRate { get { return (decimal)166.50; } }
        public decimal? DeductionDinnerRate { get { return (decimal)166.50; } }
        public decimal? SelfContainedExpensesSubTotal { get; set; }
        public decimal? LivingInCamperWagonSubTotal { get; set; }
        public decimal? CalculationOfDietsAndSmallNecessitiesSubTotal { get; set; }
        public decimal? HourAddonSubTotal { get; set; }

        public decimal? DeductionBreakFastSubTotal { get; set; }
        public decimal? DeductionLunchSubTotal { get; set; }
        public decimal? DeductionDinnerSubTotal { get; set; }
        [Display(Name = "Allowances Total")]
        public decimal? Total { get; set; }
        [Display(Name = "Signature employee")]
        public string? SignatureEmployee { get; set; }
        [Display(Name = "Signature supervisor")]
        public string? SignatureSupervisor { get; set; }

        public Diet()
        {
        }
    }    
}
