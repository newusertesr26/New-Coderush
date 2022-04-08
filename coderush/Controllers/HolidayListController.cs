using coderush;
using coderush.Controllers;
using coderush.Data;
using coderush.Models;
using CodesDotHRMS.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodesDotHRMS.Controllers
{
    public class HolidayListController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public HolidayListController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager,
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
        public IActionResult HolidayIndex()
        {
            var data = new List<HolidayListViewModel>();
            data = (from h in _context.HolidayList.OrderByDescending(x => x.Id)
                    where !h.Isdelete
                    select new HolidayListViewModel
                    {
                        Id = h.Id,
                        Name = h.Name,
                        Day = h.Day,
                        Date = h.Date,


                    }).ToList();

            return View(data);
        }

        [HttpGet]
        public IActionResult Form()
        {

            HolidayListViewModel newHoliday = new HolidayListViewModel();

            return View(newHoliday);
        }
        [HttpPost]
        public IActionResult SubmitForm([Bind("Id", "Name", "Day", "Date")] HolidayListViewModel holiday)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData[StaticString.StatusMessage] = "Error: Model state not valid.";
                    return RedirectToAction(nameof(Form), new { id = holiday.Id });
                }

                HolidayList newHoliday = new HolidayList();

                newHoliday.Name = holiday.Name;
                newHoliday.Day = holiday.Day;
                newHoliday.Date = holiday.Date;

                _context.HolidayList.Add(newHoliday);
                _context.SaveChanges();

                TempData[StaticString.StatusMessage] = "Create new HolidayList item successfully.";
                return RedirectToAction(nameof(Form));

            }
            catch (Exception ex)
            {

                TempData[StaticString.StatusMessage] = "Error: " + ex.Message;
                return RedirectToAction(nameof(Form));
            }

        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var holiday = _context.HolidayList.Where(x => x.Id.Equals(id)).FirstOrDefault();
            return View(holiday);

        }

        [HttpPost]
        public IActionResult SubmitDelete([Bind("Id")] HolidayList holiday)
        {
            try
            {
                var deleteholiday = _context.HolidayList.Where(x => x.Id.Equals(holiday.Id)).FirstOrDefault();
                if (deleteholiday == null)
                {
                    return NotFound();
                }

                deleteholiday.Isdelete = true;
            
                _context.HolidayList.Update(deleteholiday);
                _context.SaveChanges();

                TempData[StaticString.StatusMessage] = "Delete HolidayList item successfully.";
                return RedirectToAction(nameof(HolidayIndex));
            }
            catch (Exception ex)
            {

                TempData[StaticString.StatusMessage] = "Error: " + ex.Message;
                return RedirectToAction(nameof(Delete), new { id = holiday.Id > 0 ? holiday.Id : 0 });
            }
        }


    }
}
