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
using coderush.ViewModels;
using System.Threading.Tasks;

namespace coderush.Controllers
{
    [Authorize(Roles = "HR,SuperAdmin,Employee")]
    public class DashboardController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public static string userid;
        public static string username;

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
            var after15day = DateTime.Now.AddDays(+15);
           
            var data = new List<HolidayListViewModel>();

            data = (from h in _context.HolidayList.OrderByDescending(x => x.Id)
                    where !h.Isdelete && h.Date <= after15day && h.Date >= TODAYDATE

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
        public async Task<IActionResult> DashboardBindGridData(string id, string UserName)
        {
            userid = id;
            username = UserName;

            var user = _userManager.GetUserAsync(User).Result;
            var adminrole = await _userManager.IsInRoleAsync(user, "HR");
            var suadminrole = await _userManager.IsInRoleAsync(user, "SuperAdmin");

            LeaveCountViewModel levcunt = new LeaveCountViewModel();
            var leavecount = new List<LeaveCountViewModel>();
            var todayDate = DateTime.Now;
            if (!adminrole)
            {
                leavecount = _context.LeaveCount
                                    //.Where(w => w.Userid == user.Id)
                                   .Select(s => new LeaveCountViewModel()
                                   {
                                       Id = s.Id,
                                       Userid = _userManager.Users.Where(x => x.Id == s.Userid).Select(x => x.UserName).FirstOrDefault(),//s.Userid, 
                                       //Userid = user.UserName,
                                       Fromdate = s.Fromdate,
                                       Todate = s.Todate,
                                       Filename = s.FileUpload,
                                       Count = s.Count,
                                       EmployeeDescription = s.EmployeeDescription != null ? s.EmployeeDescription : string.Empty,
                                       HrDescription = s.HrDescription != null ? s.HrDescription : string.Empty,
                                       Isapprove = s.Isapprove,
                                       ApproveDate = s.ApproveDate,
                                       Approveby = s.Approveby,
                                       AdminRole = adminrole,
                                       isedit = (s.Todate <= todayDate) ? true : false,
                                       colouris = s.Todate > todayDate ? "#ffe0bb" : "",
                                   }).ToList();
                var leavecount1 = leavecount.Where(x => x.colouris == "").ToList();
                leavecount = leavecount.OrderByDescending(x => x.Todate).Where(x => x.Todate > todayDate).OrderBy(x => x.Todate).ToList();
                foreach (var data in leavecount1)
                {
                    LeaveCountViewModel ab = new LeaveCountViewModel();
                    ab.Id = data.Id;
                    //ab.Userid = _userManager.Users.Where(x => x.Id == data.Userid).Select(x => x.UserName).FirstOrDefault();//s.Userid, 
                    ab.Userid = data.Userid;
                    ab.Fromdate = data.Fromdate;
                    ab.Todate = data.Todate;
                    ab.Filename = data.Filename;
                    ab.Count = data.Count;
                    ab.EmployeeDescription = data.EmployeeDescription != null ? data.EmployeeDescription : string.Empty;
                    ab.HrDescription = data.HrDescription != null ? data.HrDescription : string.Empty;
                    ab.Isapprove = data.Isapprove;
                    ab.ApproveDate = data.ApproveDate;
                    ab.Approveby = data.Approveby;
                    ab.AdminRole = adminrole;
                    ab.isedit = (data.Todate <= todayDate) ? true : false;
                    leavecount.Add(ab);
                }

            }
            else if (adminrole || suadminrole)
            {
                try
                {
                    leavecount = _context.LeaveCount
                         //.Where(w => w.Userid == id)
                                      .Select(s => new LeaveCountViewModel()
                                      {
                                          Id = s.Id,
                                          /*Userid = _userManager.Users.Where(x => x.Id == UserName).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault(),//s.Userid, */
                                          Userid = _userManager.Users.Where(x => x.Id == s.Userid).Select(x => x.UserName).FirstOrDefault(),//s.Userid, 
                                          //Userid = UserName,
                                          Fromdate = s.Fromdate,
                                          Todate = s.Todate,
                                          Filename = s.FileUpload,
                                          Count = s.Count,
                                          EmployeeDescription = s.EmployeeDescription != null ? s.EmployeeDescription : string.Empty,
                                          HrDescription = s.HrDescription != null ? s.HrDescription : string.Empty,
                                          Isapprove = s.Isapprove,
                                          ApproveDate = s.ApproveDate,
                                          Approveby = s.Approveby,
                                          AdminRole = adminrole,
                                          isedit = (s.Todate <= todayDate) ? true : false,
                                          colouris = s.Todate > todayDate ? "#ffe0bb" : "",
                                      }).ToList();
                    //leavecount = leavecount.OrderByDescending(x => x.Todate).ToList();
                    //leavecount = leavecount.OrderByDescending(x => x.Todate).Where(x => x.Todate > todayDate).OrderBy(x=>x.Todate).ToList();
                    var leavecount1 = leavecount.Where(x => x.colouris == "").ToList();
                    leavecount = leavecount.OrderByDescending(x => x.Todate).Where(x => x.Todate > todayDate).OrderBy(x => x.Todate).ToList();
                    foreach (var data in leavecount1)
                    {
                        LeaveCountViewModel ab = new LeaveCountViewModel();
                        ab.Id = data.Id;
                        //ab.Userid = _userManager.Users.Where(x => x.Id == data.Userid).Select(x => x.UserName).FirstOrDefault();//s.Userid, 
                        ab.Userid = data.Userid;
                        ab.Fromdate = data.Fromdate;
                        ab.Todate = data.Todate;
                        ab.Filename = data.Filename;
                        ab.Count = data.Count;
                        ab.EmployeeDescription = data.EmployeeDescription != null ? data.EmployeeDescription : string.Empty;
                        ab.HrDescription = data.HrDescription != null ? data.HrDescription : string.Empty;
                        ab.Isapprove = data.Isapprove;
                        ab.ApproveDate = data.ApproveDate;
                        ab.Approveby = data.Approveby;
                        ab.AdminRole = adminrole;
                        ab.isedit = (data.Todate <= todayDate) ? true : false;
                        leavecount.Add(ab);

                    }


                }
                catch (Exception ew)
                {
                    throw ew;
                }
            }
            levcunt.List = leavecount;
            //return RedirectToAction(nameof(LeaveIndex), new { id = leaveCounts.Id > 0 ? leaveCounts.Id : 0 });
            return Json(levcunt);
        }
    }
}


