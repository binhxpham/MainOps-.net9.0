using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MainOps.Data;
using MainOps.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin,DivisionAdmin,ProjectMember,Supervisor,Member,Manager")]
    public class HourSchedulesController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HourSchedulesController(DataContext context,UserManager<ApplicationUser> userManager):base(context,userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: HourSchedules
        public async Task<IActionResult> Index()
        {
            return View(await _context.HourSchedules.ToListAsync());
        }
        [HttpGet]
        [AllowAnonymous]
        public async  Task<JsonResult> GetHourSchedule(string theId)
        {
            int id = Convert.ToInt32(theId);
            var hourschedule = await _context.HourSchedules.FindAsync(id);
            double[] array = new double[14];
            array[0] = hourschedule.day01;
            array[1] = hourschedule.day02;
            array[2] = hourschedule.day03;
            array[3] = hourschedule.day04;
            array[4] = hourschedule.day05;
            array[5] = hourschedule.day06;
            array[6] = hourschedule.day07;
            array[7] = hourschedule.day08;
            array[8] = hourschedule.day09;
            array[9] = hourschedule.day10;
            array[10] = hourschedule.day11;
            array[11] = hourschedule.day12;
            array[12] = hourschedule.day13;
            array[13] = hourschedule.day14;
            return Json(array);
        }
        // GET: HourSchedules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hourSchedule = await _context.HourSchedules
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hourSchedule == null)
            {
                return NotFound();
            }

            return View(hourSchedule);
        }

        // GET: HourSchedules/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HourSchedules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,day01,day02,day03,day04,day05,day06,day07,day08,day09,day10,day11,day12,day13,day14")] HourSchedule hourSchedule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hourSchedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hourSchedule);
        }

        // GET: HourSchedules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hourSchedule = await _context.HourSchedules.FindAsync(id);
            if (hourSchedule == null)
            {
                return NotFound();
            }
            return View(hourSchedule);
        }

        // POST: HourSchedules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,day01,day02,day03,day04,day05,day06,day07,day08,day09,day10,day11,day12,day13,day14")] HourSchedule hourSchedule)
        {
            if (id != hourSchedule.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hourSchedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HourScheduleExists(hourSchedule.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(hourSchedule);
        }

        // GET: HourSchedules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hourSchedule = await _context.HourSchedules
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hourSchedule == null)
            {
                return NotFound();
            }

            return View(hourSchedule);
        }

        // POST: HourSchedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hourSchedule = await _context.HourSchedules.FindAsync(id);
            _context.HourSchedules.Remove(hourSchedule);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HourScheduleExists(int id)
        {
            return _context.HourSchedules.Any(e => e.Id == id);
        }
    }
}
