using coderush.Data;
using coderush.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using coderush.DataEnum;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using coderush.Models.ViewModels;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using CodesDotHRMS.Models;

namespace coderush.Controllers
{
    [Authorize(Roles = "HR,SuperAdmin,Employee")].
    public class DashboardController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public DashboardController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context,
            IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
            //_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult DashboardIndex()
        {

            var TODAYDATE = DateTime.Now;
            var top15date = _context.HolidayList.OrderByDescending(x => x.Date).Take(15).ToList();
            var maxdate = top15date.Where(x => x.Date > TODAYDATE).Max(x => x.Date);
            var mindate = top15date.Where(x => x.Date > TODAYDATE).Min(x => x.Date);
             
            var data = new List<HolidayListViewModel>();

            data = (from h in _context.HolidayList.OrderByDescending(x => x.Id)
                    where !h.Isdelete && h.Date >= mindate && h.Date <= maxdate

                    select new HolidayListViewModel
                    {
                        Id = h.Id,
                        Name = h.Name,
                        Day = h.Day,
                        Date = h.Date,
                     
                    }).ToList();

            return View(data);
        }
    }
}


