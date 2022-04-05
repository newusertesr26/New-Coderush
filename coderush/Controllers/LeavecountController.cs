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
    [Authorize(Roles = "HR,SuperAdmin,Employee")]
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

            //var data = (from leave in _context.LeadMaster
            //            where leave.IsDelete == false
            //            select new LeaveCountViewModel
            //            {
            //                Id = leave.id,
            //                Userid = _context.ApplicationUser.Where(x => x.Id == Userid).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault(),
            //                Amount = leave.Amount,
            //                ExpenseDate = leave.ExpenseDate,
            //                Description = leave.Description,
            //                filename = leave.FileUpload,
            //                isactive = leave.isactive 

            //            }).ToList();

            //ViewBag.UserList = _userManager.Users
            //                    .Where(w => w.UserName != null)
            //                   .Select(s => new SelectListItem()
            //                   {
            //                       Text = s.UserName,
            //                       Value = s.Id.ToString()
            //                   }).ToList();

            //ViewBag.leavecountList = _userManager.Users
            //                    .Where(w => w.UserName != null)
            //                   .Select(s => new SelectListItem()
            //                   {
            //                       Text = s.UserName,
            //                       Value = s.Id.ToString()
            //                   }).ToList();
            //ViewBag.Role = HttpContext.Session.GetString("Role");
            //if (HttpContext.Session.GetString("Role") == "Other")
            //{
            //    return RedirectToAction("PageError", "Home");
            //}

            //userid = selectedFilter;
            //LeaveCountViewModel levcunt = new LeaveCountViewModel();
            //model = _context.LeaveCount
            //                    .Where(w => w.Userid == selectedFilter)
            //                   .Select(s => new LeaveCountViewModel()
            //                   {
            //                       Id = s.Id,
            //                       Userid = s.Userid,
            //                       Fromdate = s.Fromdate,
            //                       Todate = s.Todate,
            //                       Count = s.Count,
            //                       Description = s.Description,
            //                       Isapprove = s.Isapprove,
            //                       ApproveDate = s.ApproveDate,
            //                       Approveby = s.Approveby,
            //                   }).ToList();
            ////levcunt.List = leavecount;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> BindGridData(string id, string UserName)
        {
            userid = id;
            username = UserName;

            var user = _userManager.GetUserAsync(User).Result;
            var adminrole = await _userManager.IsInRoleAsync(user, "HR");

            //userid = id;
            //username = UserName;
            //user = _userManager.GetUserAsync(User).ToString();
            //var roles =  _userManager.GetRolesAsync(user);


            LeaveCountViewModel levcunt = new LeaveCountViewModel();
            var leavecount = new List<LeaveCountViewModel>();
            if (!adminrole)
            {

                leavecount = _context.LeaveCount
                                    .Where(w => w.Userid == user.Id)
                                   .Select(s => new LeaveCountViewModel()
                                   {
                                       Id = s.Id,
                                       //Userid = _userManager.Users.Where(x => x.Id == id).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault(),//s.Userid, 
                                       Userid = UserName,
                                       Fromdate = s.Fromdate,
                                       Todate = s.Todate,
                                       //FromdateView = s.Fromdate.Value.ToString("MM/dd/yyyy"),
                                       //TodateView = s.Todate.Value.ToString("MM/dd/yyyy"),
                                       Filename = s.FileUpload,
                                       Count = s.Count,
                                       EmployeeDescription = s.EmployeeDescription != null ? s.EmployeeDescription : string.Empty,
                                       HrDescription = s.HrDescription != null ? s.HrDescription : string.Empty,
                                       Isapprove = s.Isapprove,
                                       ApproveDate = s.ApproveDate,
                                       Approveby = s.Approveby,
                                       AdminRole = adminrole,
                                   }).ToList();
            }
            else if (adminrole)
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
                                      }).ToList();
                }
                catch (Exception ew)
                {

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

            if (!adminrole)
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
            else if (adminrole)
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
                    newleaveCount.FileUpload = leaveCounts.FileUpload.FileName.ToString(); ;
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
                    leaveHistory.FileUpload = leaveCounts.FileUpload.FileName.ToString();
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
                editLeavecount.FileUpload = leaveCounts.FileUpload.FileName.ToString();
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
                addleaveHistory.FileUpload = leaveCounts.FileUpload.FileName.ToString();
                addleaveHistory.UpdatedBy = user.Id;
                addleaveHistory.Isapprove = true;
                addleaveHistory.Approveby = user.Id;
                addleaveHistory.ApproveDate = DateTime.Now;
                _context.Update(editLeavecount);
                _context.Add(addleaveHistory);
                _context.SaveChanges();

                TempData[StaticString.StatusMessage] = "Edit existing leave count item success.";
                return RedirectToAction(nameof(Form), new { id = leaveCounts.Id > 0 ? leaveCounts.Id : 0 , userid = leaveCounts.Userid });
            }
            catch (Exception ex)
            {

                TempData[StaticString.StatusMessage] = "Error: " + ex.Message;
                return RedirectToAction(nameof(Form), new { id = leaveCounts.Id > 0 ? leaveCounts.Id : 0, userid = leaveCounts.Userid });
            }
        }

        //display leavecount create edit form
        [HttpGet]
        public IActionResult Form(int id, string userid)
        {
            //ViewBag.leavecountList = _userManager.Users
            //                    .Where(w => w.UserName != null)
            //                   .Select(s => new SelectListItem()
            //                   {
            //                       Text = s.UserName,
            //                       Value = s.Id.ToString()
            //                   }).ToList();



            LeaveCountViewModel model = new LeaveCountViewModel();



            //ViewBag.Role = HttpContext.Session.GetString("Role");
            //if (HttpContext.Session.GetString("Role") == "Other")
            //{
            //    return RedirectToAction("PageError", "Home");
            //}
            //return View(model);

            ////create new
          //LeaveCountViewModel model = new LeaveCountViewModel();

            if (id == 0)
            {

                var user = _userManager.GetUserAsync(User).Result;
                var udser = _userManager.Users.Where(w => w.Id == userid).FirstOrDefault();
               
                //newleavecount.Userid = udser.UserName;
                model.UserName = udser.UserName;
                model.Userid = udser.Id;
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
                    //_context.LeaveCount.
                    //Where(x => x.Id.Equals(id)).Select().FirstOrDefault();
            }

            ////edit leavecount master
            //LeaveCount editnewleavecount = new LeaveCount();
            //editnewleavecount = _context.LeaveCount.Where(x => x.Id.Equals(id)).FirstOrDefault();
            //editnewleavecount.Userid = username;
            //if (editnewleavecount == null)
            //{
            //    return NotFound();
            //}

            //return View(editnewleavecount);

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

        //[HttpPost]
        //public async Task<IActionResult> AddOrEdit(DataMaster model)
        //{
        //    bool IsDataExist = false;

        //    DataMaster data = await _context.Datamaster.FindAsync(model.Id);

        //    if (data != null)
        //    {
        //        IsDataExist = true;
        //    }
        //    else
        //    {
        //        data = new DataMaster();
        //    }
        //    var user = _userManager.GetUserAsync(User).Result;
        //    //if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            data.Type = model.Type;
        //            data.Text = model.Text;
        //            data.Description = model.Description;
        //            data.Isactive = model.Isactive;

        //            if (IsDataExist)
        //            {
        //                data.UpdatedBy = user.Id;
        //                data.UpdatedDate = DateTime.Now;
        //                _context.Update(data);
        //            }
        //            else
        //            {
        //                data.CreatedBy = user.Id;
        //                data.CreatedDate = DateTime.Now;
        //                _context.Add(data);
        //            }
        //            await _context.SaveChangesAsync();
        //    }
        //        catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = ex.Message });
        //    }
        //}

        //    return Json(new { success = true, message = "Data saved successfully." });
        //}

        //public FileResult DownloadFile(string fileName)
        //{
        //    //Build the File Path.
        //    string path = Path.Combine(this._webHostEnvironment.WebRootPath, "document/Leave/") + fileName;

        //    //Read the File data into Byte Array.
        //    byte[] bytes = System.IO.File.ReadAllBytes(path);

        //    //Send the File to Download.
        //    return File(bytes, "application/octet-stream", fileName);
        //}

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
        //[HttpPost]
        //public async Task<IActionResult> Delete(int Id)
        //{
        //    var data = await _context.Datamaster.FindAsync(Id);
        //    data.Isdeleted = true;
        //    _context.Datamaster.Update(data);
        //    await _context.SaveChangesAsync();

        //    return Json(new { success = true, message = "Data deleted successfully." });
        //}
    }
}


