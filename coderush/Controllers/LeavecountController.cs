using coderush.Data;
using coderush.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using coderush.DataEnum;
using Microsoft.AspNetCore.Authorization;
using DemoCreate.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using coderush.ViewModels;
using coderush.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace coderush.Controllers
{
    [Authorize(Roles = "HR,SuperAdmin,Employee")] //Harshal working on
    public class LeavecountController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        //private readonly IHostEnvironment _hostingEnvironment;
        public static string userid;
        public static string username;
        public LeavecountController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager,
           RoleManager<IdentityRole> roleManager,
           ApplicationDbContext context,
           IWebHostEnvironment webHostEnvironment)
        //IHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            //_hostingEnvironment = hostingEnvironment;
        }
        public IActionResult LeaveIndex(string id)
        {
            List<LeaveCountViewModel> model = new List<LeaveCountViewModel>();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> BindGridData(string id, string UserName)
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
                                    .Where(w => w.Userid == user.Id)
                                   .Select(s => new LeaveCountViewModel()
                                   {
                                       Id = s.Id,
                                       //Userid = _userManager.Users.Where(x => x.Id == id).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault(),//s.Userid, 
                                       Userid = user.UserName,
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
                    ab.Userid = user.UserName;
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
                         .Where(w => w.Userid == id)
                                      .Select(s => new LeaveCountViewModel()
                                      {
                                          Id = s.Id,
                                          /*Userid = _userManager.Users.Where(x => x.Id == UserName).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault(),//s.Userid, */
                                          Userid = UserName,
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
                        ab.Userid = UserName;
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

        public async Task<IActionResult> BinddrpdwnData()
        {
            var user = _userManager.GetUserAsync(User).Result;
            var adminrole = await _userManager.IsInRoleAsync(user, "HR");
            var suadminrole = await _userManager.IsInRoleAsync(user, "SuperAdmin");

            if (adminrole || suadminrole)
            {
                var leavecount = _userManager.Users
                                    .Where(w => w.UserName != null)
                                   .Select(s => new SelectListItem()
                                   {
                                       Text = String.Format("{0} {1} {2}", s.UserName, s.FirstName != null ? "|| " + s.FirstName : "", s.LastName != null ? "|| " + s.LastName : "").ToString(),
                                       Value = s.Id.ToString()
                                   }).ToList();
                return Json(leavecount);
            }
            else
            {
                var leavecount = _userManager.Users
                                      .Where(w => w.UserName != null && w.Email == user.ToString())
                                     .Select(s => new SelectListItem()
                                     {
                                         Text = String.Format("{0} {1} {2}", s.UserName, s.FirstName != null ? "|| " + s.FirstName : "", s.LastName != null ? "|| " + s.LastName : "").ToString(),
                                         Value = s.Id.ToString()
                                     }).ToList();
                return Json(leavecount);
            }
            return null;
        }
        //post submitted leavecount data. if todo.TodoId is null then create new, otherwise edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitForm([Bind("Id", "Userid", "Fromdate", "Todate", "Count", "EmployeeDescription", "HrDescription", "ApproveDate", "FileUpload", "Isapprove")] LeaveCountViewModel leaveCounts)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData[StaticString.StatusMessage] = "Error: Model state not valid.";
                    return RedirectToAction(nameof(Form), new { id = leaveCounts.Id > 0 ? leaveCounts.Id : 0, userid = leaveCounts.Userid });
                }
                var user = _userManager.GetUserAsync(User).Result;

                if (leaveCounts.FileUpload != null)
                {

                    string wwwPath = this._webHostEnvironment.WebRootPath;
                    string contentPath = this._webHostEnvironment.ContentRootPath;
                    var filename = leaveCounts.FileUpload.FileName;
                    string path = Path.Combine(this._webHostEnvironment.WebRootPath, "document/Leave");
                    //if (!Directory.Exists(path))
                    //{
                    //    Directory.CreateDirectory(path);
                    //}

                    List<string> uploadedFiles = new List<string>();

                    string fileName = Path.GetFileName(leaveCounts.FileUpload.FileName);
                    using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                    {
                        leaveCounts.FileUpload.CopyTo(stream);
                        uploadedFiles.Add(fileName);
                    }
                }

                //create new
                if (leaveCounts.Id == 0)
                {
                    LeaveCount newleaveCount = new LeaveCount();
                    LeaveHistory leaveHistory = new LeaveHistory();
                    newleaveCount.Userid = leaveCounts.Userid;
                    newleaveCount.Fromdate = leaveCounts.Fromdate;
                    newleaveCount.Todate = leaveCounts.Todate;
                    newleaveCount.Count = leaveCounts.Count;
                    newleaveCount.EmployeeDescription = leaveCounts.EmployeeDescription;
                    newleaveCount.HrDescription = leaveCounts.HrDescription;
                    if (leaveCounts.FileUpload != null)
                    {
                        newleaveCount.FileUpload = leaveCounts.FileUpload.FileName.ToString();
                    }
                    else
                    {
                        newleaveCount.FileUpload = string.Empty;
                    }
                    newleaveCount.ApproveDate = DateTime.Now;
                    newleaveCount.Isapprove = leaveCounts.Isapprove;
                    newleaveCount.CreatedBy = user.Id;
                    newleaveCount.Approveby = user.Id;
                    leaveHistory.Userid = leaveCounts.Userid;
                    leaveHistory.Fromdate = leaveCounts.Fromdate;
                    leaveHistory.Todate = leaveCounts.Todate;
                    leaveHistory.Count = leaveCounts.Count;
                    leaveHistory.EmployeeDescription = leaveCounts.EmployeeDescription;
                    leaveHistory.HRDescription = leaveCounts.HrDescription;
                    if (leaveCounts.FileUpload != null)
                    {
                        leaveHistory.FileUpload = leaveCounts.FileUpload.FileName.ToString();
                    }
                    else
                    {
                        leaveHistory.FileUpload = string.Empty;
                    }
                    leaveHistory.ApproveDate = DateTime.Now;
                    leaveHistory.Isapprove = leaveCounts.Isapprove;
                    leaveHistory.CreatedBy = userid;
                    leaveHistory.Approveby = user.Id;
                    _context.LeaveCount.Add(newleaveCount);
                    _context.LeaveHistory.Add(leaveHistory);
                    _context.SaveChanges();

                    TempData[StaticString.StatusMessage] = "Create new leave count item success.";
                    return RedirectToAction(nameof(Form), new { id = leaveCounts.Id > 0 ? leaveCounts.Id : 0, userid = leaveCounts.Userid });
                }

                //edit existing
                LeaveCount editLeavecount = new LeaveCount();
                LeaveHistory addleaveHistory = new LeaveHistory();
                editLeavecount = _context.LeaveCount.Where(x => x.Id.Equals(leaveCounts.Id)).FirstOrDefault();
                editLeavecount.Userid = leaveCounts.Userid;
                editLeavecount.Fromdate = leaveCounts.Fromdate;
                editLeavecount.Todate = leaveCounts.Todate;
                editLeavecount.Count = leaveCounts.Count;
                editLeavecount.EmployeeDescription = leaveCounts.EmployeeDescription;
                editLeavecount.HrDescription = leaveCounts.HrDescription;
                if (editLeavecount.FileUpload != null)
                {
                    editLeavecount.FileUpload = editLeavecount.FileUpload.ToString();
                }
                editLeavecount.UpdatedBy = user.Id;
                editLeavecount.Isapprove = true;
                editLeavecount.Approveby = user.Id;
                editLeavecount.ApproveDate = DateTime.Now;
                addleaveHistory.Userid = leaveCounts.Userid;
                addleaveHistory.Fromdate = leaveCounts.Fromdate;
                addleaveHistory.Todate = leaveCounts.Todate;
                addleaveHistory.Count = leaveCounts.Count;
                addleaveHistory.EmployeeDescription = leaveCounts.EmployeeDescription;
                addleaveHistory.HRDescription = leaveCounts.HrDescription;
                if (addleaveHistory.FileUpload != null)
                {
                    addleaveHistory.FileUpload = editLeavecount.FileUpload.ToString();
                }
                addleaveHistory.UpdatedBy = user.Id; 
                addleaveHistory.Isapprove = true;
                addleaveHistory.Approveby = user.Id;
                addleaveHistory.ApproveDate = DateTime.Now;
                _context.Update(editLeavecount);
                _context.Add(addleaveHistory);
                _context.SaveChanges();

                TempData[StaticString.StatusMessage] = "Edit existing leave count item success.";
                return RedirectToAction(nameof(Form), new { id = leaveCounts.Id > 0 ? leaveCounts.Id : 0, userid = leaveCounts.Userid });
            }
            catch (Exception ex)
            {

                TempData[StaticString.StatusMessage] = "Error: " + ex.Message;
                return RedirectToAction(nameof(Form), new { id = leaveCounts.Id > 0 ? leaveCounts.Id : 0, userid = leaveCounts.Userid });
            }
        }

        //display leavecount create edit form
        [HttpGet]
        public async Task<IActionResult> Form(int id, string userid)
        {
            LeaveCountViewModel model = new LeaveCountViewModel();
            var userRole = _userManager.GetUserAsync(User).Result;
            var roleHR = await _userManager.IsInRoleAsync(userRole, "HR");
            var roleSuperAdmin = await _userManager.IsInRoleAsync(userRole, " SuperAdmin");

            var userDetail = _userManager.GetUserAsync(User).Result;

            if (!roleHR && !roleSuperAdmin)
                userid = userDetail.Id;

            if (id == 0)
            {
                var udser = _userManager.Users.Where(w => w.Id == userid).FirstOrDefault();
                //newleavecount.Userid = udser.UserName;
                model.UserName = udser.UserName;
                model.Userid = udser.Id;
                // model.strUserid = udser.Id;
                return View(model);
            }
            else
            {
                var editnewleavecount = (from data in _context.LeaveCount
                                         join user in _userManager.Users on userid equals user.Id
                                         where data.Id == id
                                         select new LeaveCountViewModel
                                         {
                                             Id = data.Id,
                                             UserName = userid,
                                             Fromdate = data.Fromdate,
                                             Todate = data.Todate,
                                             Filename = data.FileUpload,
                                             Count = data.Count,
                                             EmployeeDescription = data.EmployeeDescription,
                                             HrDescription = data.HrDescription,
                                             Isapprove = data.Isapprove,
                                             ApproveDate = data.ApproveDate,
                                             Approveby = data.Approveby
                                         }).FirstOrDefault();
                return View(editnewleavecount);
            }
        }
        //display leavecount item for deletion
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var leavecount = _context.LeaveCount.Where(x => x.Id.Equals(id)).FirstOrDefault();
            return View(leavecount);
        }
        //delete submitted leave count item if found, otherwise 404
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitDelete([Bind("Id")] LeaveCount leave)
        {
            try
            {
                var deleteleavecount = _context.LeaveCount.Where(x => x.Id.Equals(leave.Id)).FirstOrDefault();
                if (deleteleavecount == null)
                {
                    return NotFound();
                }
                deleteleavecount.Isapprove = true;
                deleteleavecount.IsDelete = false;
                _context.LeaveCount.Update(deleteleavecount);
                _context.SaveChanges();

                TempData[StaticString.StatusMessage] = "Delete leave count item success.";
                return RedirectToAction(nameof(LeaveIndex));
            }
            catch (Exception ex)
            {
                TempData[StaticString.StatusMessage] = "Error: " + ex.Message;
                return RedirectToAction(nameof(Delete), new { id = leave.Id > 0 ? leave.Id : 0, userid = leave.Userid });
            }
        }

        [HttpGet]
        public IActionResult EditData(int id)
        {
            var Data = _context.LeaveCount.Where(x => x.Id == id).FirstOrDefault();
            return Json(Data);
        }

        [HttpPost]
        public ActionResult leavepopuop(int Id, string HrDescription, bool Isapprove)
        {
            try
            {
                var models = _context.LeaveCount.Where(x => x.Id == Id).FirstOrDefault();
                models.HrDescription = HrDescription;
                models.Isapprove = Isapprove;
                _context.SaveChanges();
                var result = new { Success = "true", Message = "Data save successfully." };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { Success = "False", Message = ex.Message };
                return Json(result);
            }
        }
    }
}


