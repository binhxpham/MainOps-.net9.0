using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MainOps.Data;
using MainOps.Models;
using MainOps.Resources;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
namespace MainOps.Controllers
{
    public class CostController : BaseController
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly LocService _SharedLocalizer;

        public CostController(DataContext context, IWebHostEnvironment env,UserManager<ApplicationUser> userManager,LocService loc):base(context,userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
            _SharedLocalizer = loc;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<decimal> GetCosts(BoQHeadLine boqheadline,DateTime startdate,DateTime enddate)
        {
            decimal totalcost = (decimal)0.00;
            var costInstallations = await _context.Installations
                .Where(x => x.TimeStamp.Date >= startdate.Date
                &&
                x.ProjectId.Equals(boqheadline.ProjectId) 
                && Convert.ToInt32(x.ItemType.BoQnr) == Convert.ToInt32(boqheadline.BoQnum)).ToListAsync();
            foreach(var item in costInstallations)
            {
                totalcost += Convert.ToDecimal(item.ItemType.initial_cost);
                if(item.DeinstallDate != null)
                {
                    totalcost += Convert.ToDecimal( (Convert.ToDateTime(item.DeinstallDate) - item.TimeStamp).TotalDays + 1 ) * Convert.ToDecimal(item.ItemType.daily_cost);
                }
                else
                {
                    totalcost += Convert.ToDecimal((enddate.Date - item.TimeStamp).TotalDays + 1) * Convert.ToDecimal(item.ItemType.daily_cost);
                }
            }

            return totalcost;
        }


    }
}