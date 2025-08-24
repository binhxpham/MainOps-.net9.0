using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class ProjectStatus
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }
        [Display(Name = "Division")]
        public int? DivisionId { get; set; }
        [Display(Name = "Division")]
        public virtual Division Division { get; set; }
        [Display(Name = "Status")]
        public int? StatusDescriptionId { get; set; }
        [Display(Name = "Status")]
        public virtual StatusDescription StatusDescription { get; set; }
        [Display(Name = "Offer Number")]
        public string QuoteNumber { get; set; }
        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }
        [Display(Name = "Tender Manager")]
        public string TenderManager { get; set; }
        [Display(Name = "Tender Date")]
        public DateTime? TenderDate { get; set; }
        [Display(Name = "Client Tender Date")]
        public DateTime? ClientTenderDate { get; set; }
        [Display(Name = "Date Submitted")]
        public DateTime? DateSubmitted { get; set; }
        [Display(Name = "Project Description")]
        public string ProjectDescription { get; set; }
        [Display(Name = "Project Category")]
        public ICollection<ProjectStatusProjectCategory> ProjectStatusProjectCategories { get; set; }
        [Display(Name = "Options Value")]
        public double OptionsValue { get; set; }
        [Display(Name = "Total Value incl. Options")]
        public double TotalContractValue { get; set; }
        [Display(Name = "Comments")]
        public string Comments { get; set; }
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
        [Display(Name = "Client")]
        public string Client { get; set; }
        [Display(Name ="Win Chance")]
        public decimal WinChance { get; set; }
        [Display(Name = "Client Contact")]
        public string ClientContact { get; set; }
        

        public ProjectStatus()
        {

        }
    }
    public class CatTrueFalse
    {
        public ProjectCategory Cat { get; set; }
        public bool TrueFalse { get; set; }
        public CatTrueFalse()
        {
            Cat = new ProjectCategory();
        }
        public CatTrueFalse(ProjectCategory c,bool b)
        {
            Cat = c;
            TrueFalse = b;
        }
    }
    public class ProjectStatusCreateVM
    {
        public ProjectStatus PS { get; set; }
        public List<CatTrueFalse> Cats { get; set; }
        public ProjectStatusCreateVM()
        {
            
        }
        public ProjectStatusCreateVM(List<ProjectCategory> cats)
        {
            this.PS = new ProjectStatus();
            this.Cats = new List<CatTrueFalse>();
            foreach(var cat in cats)
            {
                Cats.Add(new CatTrueFalse(cat,false));
            }
        }
    }
    public class ProjectStatusEditVM
    {
        public ProjectStatus ProjectStatus { get; set; }
        public List<int> ProjectCategories { get; set; }
        public ProjectStatusEditVM()
        {
            ProjectCategories = new List<int>();
        }
        public ProjectStatusEditVM(ProjectStatus PS)
        {
            ProjectCategories = new List<int>();
            this.ProjectStatus = PS;
            foreach(var item in PS.ProjectStatusProjectCategories)
            {
                ProjectCategories.Add(item.ProjectCategoryId);
            }
        }
    }
    public class YearlyValue
    {
        public int Year { get; set; }
        public double Value { get; set; }
    }
    public class YearlyValueCategory
    {
        public int Year { get; set; }
        public double Value { get; set; }
        public string Category { get; set; }
    }
    public class ProjectStatusVM
    {
        public List<ProjectCategory> Categories { get; set; }
        public List<ProjectStatus> Projects { get; set; }
        public List<YearlyValueCategory> AnnualCategoryTotalWorth { get
            {
                List<YearlyValueCategory> annual = new List<YearlyValueCategory>();
                for (int year = Projects.Select(x => x.StartDate.Year).Min(); year <= Projects.Select(x => x.EndDate.Year).Max(); year++)
                {
                    foreach(var cat in this.Categories)
                    {
                        if(this.Projects.Where(x => x.StartDate.Year.Equals(year) && x.ProjectStatusProjectCategories.Count(y => y.ProjectCategoryId.Equals(cat.Id)) >= 1).Count() > 0) { 
                            annual.Add(new YearlyValueCategory { Year = year, Value = this.Projects.Where(x => x.StartDate.Year.Equals(year) && x.ProjectStatusProjectCategories.Count(y => y.ProjectCategoryId.Equals(cat.Id)) >= 1).Sum(x => x.TotalContractValue / x.ProjectStatusProjectCategories.Count()),Category = cat.Text });
                        }
                        else
                        {
                            annual.Add(new YearlyValueCategory { Year = year, Value = 0.0, Category = cat.Text });
                        }
                    }
                   
                }
                return annual;
            } 
        }
        public List<YearlyValueCategory> AnnualCategoryTotalWorthOverYears
        {
            get
            {
                List<YearlyValueCategory> annual = new List<YearlyValueCategory>();
                for(int year = Projects.Select(x => x.StartDate.Year).Min();year <= Projects.Select(x => x.EndDate.Year).Max();year++)
                {
                    DateTime year_date = new DateTime(year, 1, 1);
                    foreach (var cat in this.Categories)
                    {
                        if (this.Projects.Where(x => x.ProjectStatusProjectCategories.Count(y => y.ProjectCategoryId.Equals(cat.Id)) >= 1).Count() > 0)
                        {
                            
                            annual.Add(new YearlyValueCategory { Year = year, Value = this.Projects.Where(x => x.StartDate.Year <= year && x.EndDate.Year >= year && x.ProjectStatusProjectCategories.Count(y => y.ProjectCategoryId.Equals(cat.Id)) >= 1).Sum(x => x.TotalContractValue * GetFraction(x,year_date) / x.ProjectStatusProjectCategories.Count()), Category = cat.Text });
                        }
                        else
                        {
                            annual.Add(new YearlyValueCategory { Year = year, Value = 0.0, Category = cat.Text });
                        }
                    }

                }
                return annual;
            }
        }
        public static List<YearlyValue> GetYears(ProjectStatus PS)
        {
            List<YearlyValue> thelist = new List<YearlyValue>();
            DateTime RealEnd = new DateTime(PS.EndDate.Year, 12, 31);
            DateTime RealStart = new DateTime(PS.StartDate.Year, 12, 31);
            //int years = 0;
            //int startyeardays = 0;
            //int endyeardays = 0;
            int totaldays = 0;
            int lastyear = PS.EndDate.Year;
            for(DateTime dt = PS.StartDate; dt <= PS.EndDate; dt = dt.AddDays(1))
            {
                totaldays += 1;
            }
            for (DateTime dt = RealStart; dt <= RealEnd; dt = dt.AddYears(1))
            {
                int thisyearsdays = 0;
                if (dt.Equals(RealStart))
                {
                    if(RealStart <= PS.EndDate) { 
                        for(DateTime dtstart = PS.StartDate; dtstart <= RealStart; dtstart = dtstart.AddDays(1))
                        {
                            //startyeardays += 1;
                            thisyearsdays += 1;
                        }
                    }
                    else
                    {
                        for (DateTime dtstart = PS.StartDate; dtstart <= PS.EndDate; dtstart = dtstart.AddDays(1))
                        {
                            //startyeardays += 1;
                            thisyearsdays += 1;
                        }
                    }
                }
                else if (dt.Equals(RealEnd))
                {
                    for (DateTime dtend = new DateTime(PS.EndDate.Year,1,1); dtend <= PS.EndDate; dtend = dtend.AddDays(1))
                    {
                        //endyeardays += 1;
                        thisyearsdays += 1;
                    }
                }
                else
                {
                    //totaldays += 365;
                    thisyearsdays = 365;
                }
                thelist.Add(new YearlyValue { Year = dt.Year, Value = PS.TotalContractValue * (Convert.ToDouble(thisyearsdays) / Convert.ToDouble(totaldays)) });
            }
            return thelist;
        }
        public List<YearlyValue> AnnualEstimatedWork
        {
            get
            {

                List<YearlyValue> annual = new List<YearlyValue>();
                for (int year = Projects.Select(x => x.StartDate.Year).Min(); year <= Projects.Select(x => x.EndDate.Year).Max(); year++)
                {
                    YearlyValue yv = new YearlyValue();
                    yv.Year = year;
                    yv.Value = 0;
                    DateTime year_date = new DateTime(year, 1, 1);

                    yv.Value = this.Projects.Where(x => !x.StatusDescriptionId.Equals(9) && !x.StatusDescriptionId.Equals(12) && x.StartDate.Year.Equals(year)).Sum(x => x.TotalContractValue * (Convert.ToDouble(x.WinChance)/100.0));

                    annual.Add(yv);
                    //annual.Add(new YearlyValue { Year = year, Value = this.Projects.Where(x => x.StartDate.Year.Equals(year))
                    //    .Sum(x => x.TotalContractValue*Convert.ToDouble(x.WinChance)/100.0) });
                }
                return annual;
            }
        }
        //public List<YearlyValue> AnnualEstimatedWorkOverYears
        //{
        //    get
        //    {

        //        List<YearlyValue> annual = new List<YearlyValue>();
        //        foreach (var year in this.Projects.Where(x => !x.StatusDescriptionId.Equals(9) && !x.StatusDescriptionId.Equals(12)).GroupBy(x => x.StartDate.Year).Select(x => x.Key).Distinct())
        //        {
        //            YearlyValue yv = new YearlyValue();
        //            yv.Year = year;
        //            yv.Value = 0;
        //            DateTime year_date = new DateTime(year, 1, 1);
        //            foreach (var proj in this.Projects.Where(x => !x.StatusDescriptionId.Equals(9) && !x.StatusDescriptionId.Equals(12)))
        //            {
        //                List<YearlyValue> projectyears = GetYears(proj);
        //                var theval = projectyears.SingleOrDefault(x => x.Year.Equals(year));
        //                if (theval != null)
        //                {
        //                    yv.Value += projectyears.SingleOrDefault(x => x.Year.Equals(year)).Value * (Convert.ToDouble(proj.WinChance) / 100.0);
        //                }
        //                else
        //                {
        //                    yv.Value += 0;
        //                }

        //            }
        //            annual.Add(yv);
        //            //annual.Add(new YearlyValue { Year = year, Value = this.Projects.Where(x => x.StartDate.Year.Equals(year))
        //            //    .Sum(x => x.TotalContractValue*Convert.ToDouble(x.WinChance)/100.0) });
        //        }
        //        return annual;
        //    }
        //}
        public List<YearlyValue> AnnualEstimatedWorkOverYears
        {
            get
            {

                List<YearlyValue> annual = new List<YearlyValue>();
                //
                for (int year = Projects.Select(x => x.StartDate.Year).Min(); year <= Projects.Select(x => x.EndDate.Year).Max(); year++)
                {
                    DateTime year_date = new DateTime(year, 1, 1);
                    annual.Add(new YearlyValue { Year = year, Value = this.Projects.Where(x => !x.StatusDescriptionId.Equals(9) && !x.StatusDescriptionId.Equals(12)).Sum(x => x.TotalContractValue * (Convert.ToDouble(x.WinChance) / 100.0) * GetFraction(x, year_date)) });
                    

                  
                }
                return annual;
            }
        }
        public double GetFraction(ProjectStatus PS,DateTime year)
        {
            if(year.Year < PS.StartDate.Year || year.Year > PS.EndDate.Year)
            {
                return 0.0;
            }
            if(PS.StartDate.Year == PS.EndDate.Year)
            {
                return 1.0;
            }
            else
            {
                double totaldays = (PS.EndDate - PS.StartDate).TotalDays;
                if(year.Year == PS.StartDate.Year)
                {
                    double daysleftinyear = (new DateTime(PS.StartDate.Year, 12, 31) - PS.StartDate).TotalDays + 1.0;
                    return daysleftinyear / totaldays;
                }
                else if(year.Year == PS.EndDate.Year)
                {
                    double daysleftinyear = (PS.EndDate - new DateTime(PS.EndDate.Year, 1, 1)).TotalDays + 1.0;
                    return daysleftinyear / totaldays;
                }
                else
                {
                    return ((new DateTime(year.Year, 12, 31) - new DateTime(year.Year, 1, 1)).TotalDays + 1.0)/totaldays;
                }
            }
        }
        public List<YearlyValue> AnnualActualWork
        {
            get
            {
                List<YearlyValue> annual = new List<YearlyValue>();
                for (int year = Projects.Select(x => x.StartDate.Year).Min(); year <= Projects.Select(x => x.EndDate.Year).Max(); year++)
                {
                    DateTime yearDate = new DateTime(year, 1, 1);
                    annual.Add(new YearlyValue { Year = year, Value = this.WonProjects.Where(x => x.StartDate.Year.Equals(year)).Sum(x => x.TotalContractValue) });
                }
                return annual;

            }
         }
        public List<YearlyValue> AnnualActualWorkOverYears
        {
            get
            {
                List<YearlyValue> annual = new List<YearlyValue>();
                for (int year = Projects.Select(x => x.StartDate.Year).Min(); year <= Projects.Select(x => x.EndDate.Year).Max(); year++)
                {
                    DateTime yearDate = new DateTime(year, 1, 1);
                    annual.Add(new YearlyValue { Year = year, Value = this.WonProjects.Sum(x => x.TotalContractValue * GetFraction(x, yearDate)) });
                }
                return annual;

            }
        }


        public List<YearlyValueCategory> AnnualCategoryOptionsWorth
        {
            get
            {
                List<YearlyValueCategory> annual = new List<YearlyValueCategory>();
                for (int year = Projects.Select(x => x.StartDate.Year).Min(); year <= Projects.Select(x => x.EndDate.Year).Max(); year++)
                {
                    DateTime year_date = new DateTime(year, 1, 1);
                    foreach (var cat in this.Categories)
                    {
                        if (this.Projects.Where(x => x.StartDate.Year.Equals(year) && x.ProjectStatusProjectCategories.Count(y => y.ProjectCategoryId.Equals(cat.Id)) >= 1).Count() > 0)
                        {
                            annual.Add(new YearlyValueCategory { Year = year, Value = this.Projects.Where(x => x.StartDate.Year.Equals(year) && x.ProjectStatusProjectCategories.Count(y => y.ProjectCategoryId.Equals(cat.Id)) >= 1).Sum(x => x.OptionsValue/x.ProjectStatusProjectCategories.Count()), Category = cat.Text });
                        }
                        else
                        {
                            annual.Add(new YearlyValueCategory { Year = year, Value = 0.0, Category = cat.Text });
                        }
                    }

                }
                return annual;
            }
        }
        public List<YearlyValueCategory> AnnualCategoryOptionsWorthOverYears
        {
            get
            {
                List<YearlyValueCategory> annual = new List<YearlyValueCategory>();
                for (int year = Projects.Select(x => x.StartDate.Year).Min(); year <= Projects.Select(x => x.EndDate.Year).Max(); year++)
                {
                    DateTime year_date = new DateTime(year, 1, 1);
                    foreach (var cat in this.Categories)
                    {
                        if (this.Projects.Where(x => x.StartDate.Year.Equals(year) && x.ProjectStatusProjectCategories.Count(y => y.ProjectCategoryId.Equals(cat.Id)) >= 1).Count() > 0)
                        {
                            annual.Add(new YearlyValueCategory { Year = year, Value = this.Projects.Where(x => x.ProjectStatusProjectCategories.Count(y => y.ProjectCategoryId.Equals(cat.Id)) >= 1).Sum(x => x.OptionsValue * GetFraction(x, year_date) / x.ProjectStatusProjectCategories.Count()), Category = cat.Text });
                        }
                        else
                        {
                            annual.Add(new YearlyValueCategory { Year = year, Value = 0.0, Category = cat.Text });
                        }
                    }

                }
                return annual;
            }
        }

        public List<YearlyValue> AnnualOptions { get
            {
                List<YearlyValue> annual = new List<YearlyValue>();
                for (int year = Projects.Select(x => x.StartDate.Year).Min(); year <= Projects.Select(x => x.EndDate.Year).Max(); year++)
                {
                    DateTime year_date = new DateTime(year, 1, 1);
                    annual.Add(new YearlyValue { Year = year, Value = this.Projects.Where(x => x.StartDate.Year.Equals(year)).Sum(x => x.OptionsValue) });
                }
                return annual;
            } 
        }
        public List<YearlyValue> AnnualOptionsOverYears
        {
            get
            {
                List<YearlyValue> annual = new List<YearlyValue>();
                for (int year = Projects.Select(x => x.StartDate.Year).Min(); year <= Projects.Select(x => x.EndDate.Year).Max(); year++)
                {
                    DateTime year_date = new DateTime(year, 1, 1);
                    annual.Add(new YearlyValue { Year = year, Value = this.Projects.Sum(x => x.OptionsValue * GetFraction(x, year_date)) });
                }
                return annual;
            }
        }
        public List<YearlyValue> AnnualTotal
        {
            get
            {
                List<YearlyValue> annual = new List<YearlyValue>();
                for (int year = Projects.Select(x => x.StartDate.Year).Min(); year <= Projects.Select(x => x.EndDate.Year).Max(); year++)
                {
                    DateTime year_date = new DateTime(year, 1, 1);
                    annual.Add(new YearlyValue { Year = year, Value = this.Projects.Where(x => x.StartDate.Year.Equals(year)).Sum(x => x.TotalContractValue ) });
                }
                return annual;
            }
        }
        public List<YearlyValue> AnnualTotalOverYears
        {
            get
            {
                List<YearlyValue> annual = new List<YearlyValue>();
                for (int year = Projects.Select(x => x.StartDate.Year).Min(); year <= Projects.Select(x => x.EndDate.Year).Max(); year++)
                {
                    DateTime year_date = new DateTime(year, 1, 1);
                    annual.Add(new YearlyValue { Year = year, Value = this.Projects.Sum(x => x.TotalContractValue * GetFraction(x, year_date)) });
                }
                return annual;
            }
        }
        public List<YearlyValue> AnnualWorthWonProjects
        {
            get
            {
                List<YearlyValue> annual = new List<YearlyValue>();
                DateTime year_date;
                for (int year = Projects.Select(x => x.StartDate.Year).Min(); year <= Projects.Select(x => x.EndDate.Year).Max(); year++)
                {
                    year_date = new DateTime(year, 1, 1);
                    annual.Add(new YearlyValue { Year = year, Value = Convert.ToDouble(this.WonProjects.Where(x => Convert.ToDateTime(x.StartDate).Year.Equals(year)).Sum(x => x.TotalContractValue)) });
                }
                return annual;
            }
        }
        public List<YearlyValue> AnnualWorthWonProjectsOverYears
        {
            get
            {
                List<YearlyValue> annual = new List<YearlyValue>();
                DateTime year_date;
                for (int year = Projects.Select(x => x.StartDate.Year).Min(); year <= Projects.Select(x => x.EndDate.Year).Max(); year++)
                {
                    year_date = new DateTime(year, 1, 1);
                    annual.Add(new YearlyValue { Year = year, Value = Convert.ToDouble(this.WonProjects.Sum(x => x.TotalContractValue * GetFraction(x, year_date))) });
                }
                return annual;
            }
        }
        public List<YearlyValue> AnnualWorthOpenProjects
        {
            get
            {
                List<YearlyValue> annual = new List<YearlyValue>();
                DateTime year_date;
                for (int year = Projects.Select(x => x.StartDate.Year).Min(); year <= Projects.Select(x => x.EndDate.Year).Max(); year++)
                {
                    year_date = new DateTime(year, 1, 1);
                    annual.Add(new YearlyValue { Year = year, Value = Convert.ToDouble(this.OpenProjects.Where(x => Convert.ToDateTime(x.StartDate).Year.Equals(year)).Sum(x => x.TotalContractValue)) });
                }
                return annual;
            }
        }
        public List<YearlyValue> AnnualWorthOpenProjectsOverYears
        {
            get
            {
                List<YearlyValue> annual = new List<YearlyValue>();
                DateTime year_date;
                for (int year = Projects.Select(x => x.StartDate.Year).Min(); year <= Projects.Select(x => x.EndDate.Year).Max(); year++)
                {
                    year_date = new DateTime(year, 1, 1);
                    annual.Add(new YearlyValue { Year = year, Value = Convert.ToDouble(this.OpenProjects.Sum(x => x.TotalContractValue * GetFraction(x, year_date))) });
                }
                return annual;
            }
        }
        public List<YearlyValue> AnnualWorthLostProjects
        {
            get
            {
                List<YearlyValue> annual = new List<YearlyValue>();
                for (int year = Projects.Select(x => x.StartDate.Year).Min(); year <= Projects.Select(x => x.EndDate.Year).Max(); year++)
                {
                    DateTime year_date = new DateTime(year, 1, 1);
                    annual.Add(new YearlyValue { Year = year, Value = this.LostProjects.Where(x => Convert.ToDateTime(x.StartDate).Year.Equals(year)).Sum(x => x.TotalContractValue) });
                }
                return annual;
            }
        }
        public List<YearlyValue> AnnualWorthLostProjectsOverYears
        {
            get
            {
                List<YearlyValue> annual = new List<YearlyValue>();
                for (int year = Projects.Select(x => x.StartDate.Year).Min(); year <= Projects.Select(x => x.EndDate.Year).Max(); year++)
                {
                    DateTime year_date = new DateTime(year, 1, 1);
                    annual.Add(new YearlyValue { Year = year, Value = this.LostProjects.Where(x => Convert.ToDateTime(x.StartDate).Year.Equals(year)).Sum(x => x.TotalContractValue * GetFraction(x,year_date)) });
                }
                return annual;
            }
        }
        public List<YearlyValue> AnnualAverageWorthLostProjects
        {
            get
            {
                List<YearlyValue> annual = new List<YearlyValue>();
                for (int year = Projects.Select(x => x.StartDate.Year).Min(); year <= Projects.Select(x => x.EndDate.Year).Max(); year++)
                {
                    DateTime year_date = new DateTime(year, 1, 1);
                    try
                    {
                        annual.Add(new YearlyValue { Year = year, Value = Convert.ToInt32(this.LostProjects.Where(x => x.StartDate.Year.Equals(year)).Sum(x => x.TotalContractValue) / this.LostProjects.Where(x => x.StartDate.Year.Equals(year)).Count() / 1000) * 1000 });
                    }
                    catch
                    {
                        annual.Add(new YearlyValue { Year = year, Value = this.LostProjects.Where(x => x.StartDate.Year.Equals(year)).Sum(x => x.TotalContractValue) / this.LostProjects.Where(x => x.StartDate.Year.Equals(year)).Count() });
                    }

                }
                return annual;
            }
        }
        public List<YearlyValue> AnnualAverageWorthLostProjectsOverYears
        {
            get
            {
                List<YearlyValue> annual = new List<YearlyValue>();
                for (int year = Projects.Select(x => x.StartDate.Year).Min(); year <= Projects.Select(x => x.EndDate.Year).Max(); year++)
                {
                    DateTime year_date = new DateTime(year, 1, 1);
                    try {
                    annual.Add(new YearlyValue { Year = year, Value = Convert.ToInt32(this.LostProjects.Where(x => x.StartDate.Year <= year && x.EndDate.Year >= year).Sum(x => x.TotalContractValue * GetFraction(x, year_date))/ this.LostProjects.Where(x => x.StartDate.Year <= year && x.EndDate.Year >= year).Count()/1000.0)*1000.0 });
                    }
                    catch
                    {
                        annual.Add(new YearlyValue { Year = year, Value = this.LostProjects.Where(x => x.StartDate.Year <= year && x.EndDate.Year >= year).Sum(x => x.TotalContractValue * GetFraction(x, year_date)) / this.LostProjects.Where(x => x.StartDate.Year <= year && x.EndDate.Year >= year).Count() });
                    }
                }
                return annual;
            }
        }
        public List<YearlyValue> AnnualAverageWorthWonProjects
        {
            get
            {
                List<YearlyValue> annual = new List<YearlyValue>();
                for (int year = Projects.Select(x => x.StartDate.Year).Min(); year <= Projects.Select(x => x.EndDate.Year).Max(); year++)
                {
                    DateTime year_date = new DateTime(year, 1, 1);
                    try
                    {
                        annual.Add(new YearlyValue { Year = year, Value = Convert.ToInt32(this.WonProjects.Where(x => x.StartDate.Year.Equals(year)).Sum(x => x.TotalContractValue) / this.WonProjects.Where(x => x.StartDate.Year.Equals(year)).Count() / 1000) * 1000 });
                    }
                    catch
                    {
                        annual.Add(new YearlyValue { Year = year, Value = this.WonProjects.Where(x => x.StartDate.Year.Equals(year)).Sum(x => x.TotalContractValue) / this.WonProjects.Where(x => x.StartDate.Year.Equals(year)).Count() });
                    }
                    
                }
                return annual;
            }
        }
        public List<YearlyValue> AnnualAverageWorthWonProjectsOverYears
        {
            get
            {
                List<YearlyValue> annual = new List<YearlyValue>();
                for (int year = Projects.Select(x => x.StartDate.Year).Min(); year <= Projects.Select(x => x.EndDate.Year).Max(); year++)
                {
                    DateTime year_date = new DateTime(year, 1, 1);
                    try
                    {
                        annual.Add(new YearlyValue { Year = year, Value = Convert.ToInt32(this.WonProjects.Where(x => x.StartDate.Year <= year && x.EndDate.Year >= year).Sum(x => x.TotalContractValue * GetFraction(x, year_date)) / this.WonProjects.Where(x => x.StartDate.Year <= year && x.EndDate.Year >= year).Count() / 1000.0) * 1000.0 });
                    }
                    catch
                    {
                        annual.Add(new YearlyValue { Year = year, Value = this.WonProjects.Where(x => x.StartDate.Year <= year && x.EndDate.Year >= year).Sum(x => x.TotalContractValue * GetFraction(x, year_date)) / this.WonProjects.Where(x => x.StartDate.Year <= year && x.EndDate.Year >= year).Count() });
                    }
                }
                return annual;
            }
        }

        public List<YearlyValue> AnnualAmount
        {
            get
            {
                List<YearlyValue> annual = new List<YearlyValue>();
                for (int year = Projects.Select(x => x.StartDate.Year).Min(); year <= Projects.Select(x => x.EndDate.Year).Max(); year++)
                {
                    annual.Add(new YearlyValue { Year = year, Value = Convert.ToDouble(this.Projects.Where(x => x.StartDate.Year.Equals(year)).Count()) });
                }
                return annual;
            }
        }
        public List<YearlyValue> AnnualAmountWithoutCancels
        {
            get
            {
                List<YearlyValue> annual = new List<YearlyValue>();
                for (int year = Projects.Select(x => x.StartDate.Year).Min(); year <= Projects.Select(x => x.EndDate.Year).Max(); year++)
                {
                    annual.Add(new YearlyValue { Year = year, Value = Convert.ToDouble(this.Projects.Where(x => x.StartDate.Year.Equals(year) && !x.StatusDescriptionId.Equals(9)).Count()) });
                }
                return annual;
            }
        }
        public List<ProjectStatus> WonProjects { get
            {
                return this.Projects.Where(x => x.StatusDescriptionId.Equals(8)).ToList();
            } }
        public List<ProjectStatus> LostProjects
        {
            get
            {
                return this.Projects.Where(x => x.StatusDescriptionId.Equals(12)).ToList();
            }
        }
        public List<ProjectStatus> OpenProjects
        {
            get
            {
                return this.Projects.Where(x => x.StatusDescriptionId.Equals(10) || x.StatusDescriptionId.Equals(11)).ToList();
            }
        }
        public List<YearlyValue> AnnualWinPercent
        {
            get
            {
                
                List<YearlyValue> annual = new List<YearlyValue>();
                for (int year = Projects.Select(x => x.StartDate.Year).Min(); year <= Projects.Select(x => x.EndDate.Year).Max(); year++)
                {
                    int helperindex = this.AnnualAmount.FindIndex(x => x.Year.Equals(year));
                    
                    annual.Add(new YearlyValue { Year = year, Value =  this.WonProjects.Where(x => Convert.ToDateTime(x.StartDate).Year.Equals(year)).Count() / this.AnnualAmount[helperindex].Value });
                }
                return annual;
            }
        }
        public ProjectStatusVM()
        {
            Projects = new List<ProjectStatus>();
            Categories = new List<ProjectCategory>();
            
        }
        public ProjectStatusVM(List<ProjectStatus> projs,List<ProjectCategory> cats)
        {
            this.Projects = projs;
            this.Categories = cats;

        }
    }
    public class StatusDescription
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Phase")]
        public string Description { get; set; }
    }
    public class ProjectCategory
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Project Category")]
        public string Text { get; set; }
        public ICollection<ProjectStatusProjectCategory> ProjectStatusProjectCategories { get; set; }
    }
    
    public class ProjectStatusProjectCategory
    {
        public int ProjectStatusId { get; set; }
        public virtual ProjectStatus ProjectStatus { get; set; }
        [Display(Name = "Project Category")]
        public int ProjectCategoryId { get; set; }
        [Display(Name = "Project Category")]
        public virtual ProjectCategory ProjectCategory { get; set; }
        public ProjectStatusProjectCategory()
        {

        }
        public ProjectStatusProjectCategory(int ProjectStatusId,int ProjectCategoryId)
        {
            this.ProjectStatusId = ProjectStatusId;
            this.ProjectCategoryId = ProjectCategoryId;
        }

    }
    
}
