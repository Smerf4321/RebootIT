﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RebootIT.TimesheetApp.Data;

namespace RebootIT.TimesheetApp.Controllers
{
    public class TimesheetsController : Controller
    {
        static public int CurrentStaffId = 0;
        static public int CurrentClientId = 0;
        static public int CurrentLocationId = 0;
        private readonly TimesheetDbContext _context;

        public TimesheetsController(TimesheetDbContext context)
        {
            _context = context;
        }

        // GET: Timesheets
        public async Task<IActionResult> Index()
        {
            var timesheetDbContext = _context.Timesheets.Include(t => t.Client).Include(t => t.Location).Include(t => t.Staff);

            return View(await timesheetDbContext.ToListAsync());
        }

        // GET: Staff Timesheets
        public async Task<IActionResult> StaffIndex(int staffId)
        {
            CurrentStaffId = staffId;
            var timesheetDbContext = _context.Timesheets.Include(t => t.Client).Include(t => t.Location).Include(t => t.Staff).Where(t => t.StaffId == staffId);

            return View("Index", await timesheetDbContext.ToListAsync());
        }

        // GET: Client Timesheets
        public async Task<IActionResult> ClientIndex(int clientId)
        {
            CurrentClientId = clientId;
            var timesheetDbContext = _context.Timesheets.Include(t => t.Client).Include(t => t.Location).Include(t => t.Staff).Where(t => t.ClientId == clientId);

            return View("Index", await timesheetDbContext.ToListAsync());
        }

        // GET: Client Timesheets
        public async Task<IActionResult> LocationIndex(int locationId)
        {
            CurrentLocationId = locationId;
            var timesheetDbContext = _context.Timesheets.Include(t => t.Client).Include(t => t.Location).Include(t => t.Staff).Where(t => t.LocationId == locationId);

            return View("Index", await timesheetDbContext.ToListAsync());
        }

        // GET: Timesheets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timesheet = await _context.Timesheets
                .Include(t => t.Client)
                .Include(t => t.Location)
                .Include(t => t.Staff)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (timesheet == null)
            {
                return NotFound();
            }

            return View(timesheet);
        }

        // GET: Timesheets/Create
        public IActionResult Create()
        {
            if (CurrentClientId == 0)
            {
                ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "CompanyName");
            }
            else
            {
                ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "CompanyName", CurrentClientId);
            }
            
            if (CurrentLocationId == 0)
            {
                ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name");
            }
            else
            {
                ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name", CurrentLocationId);
            }

            if (CurrentStaffId == 0)
            {
                ViewData["StaffId"] = new SelectList(_context.Staff, "Id", "Email");
            }
            else
            {
                ViewData["StaffId"] = new SelectList(_context.Staff, "Id", "Email", CurrentStaffId);
            }

            return View();
        }

        // POST: Timesheets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MinutesWorked,StaffId,ClientId,LocationId")] Timesheet timesheet)
        {
            if (ModelState.IsValid)
            {
                _context.Add(timesheet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "CompanyName", timesheet.ClientId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name", timesheet.LocationId);
            ViewData["StaffId"] = new SelectList(_context.Staff, "Id", "Email", timesheet.StaffId);
            return View(timesheet);
        }

        // GET: Timesheets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timesheet = await _context.Timesheets.FindAsync(id);
            if (timesheet == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "CompanyName", timesheet.ClientId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name", timesheet.LocationId);

            var staffList = _context.Staff.Select(s => new { Id = s.Id, FullName = s.Forename + " " + s.Surname } );

            ViewData["StaffId"] = new SelectList(staffList, "Id", "FullName", timesheet.StaffId);
            return View(timesheet);
        }

        // POST: Timesheets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MinutesWorked,StaffId,ClientId,LocationId")] Timesheet timesheet)
        {
            if (id != timesheet.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(timesheet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TimesheetExists(timesheet.Id))
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
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "BillingAddress", timesheet.ClientId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Address", timesheet.LocationId);
            ViewData["StaffId"] = new SelectList(_context.Staff, "Id", "Email", timesheet.StaffId);
            return View(timesheet);
        }

        // GET: Timesheets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timesheet = await _context.Timesheets
                .Include(t => t.Client)
                .Include(t => t.Location)
                .Include(t => t.Staff)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (timesheet == null)
            {
                return NotFound();
            }

            return View(timesheet);
        }

        // POST: Timesheets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var timesheet = await _context.Timesheets.FindAsync(id);
            _context.Timesheets.Remove(timesheet);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TimesheetExists(int id)
        {
            return _context.Timesheets.Any(e => e.Id == id);
        }
    }
}
