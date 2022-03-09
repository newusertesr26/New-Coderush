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
    [Authorize(Roles = "HR,SuperAdmin")]
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
        public IActionResult LeaveIndex(string selectedFilter)
        {
            List<LeaveCountViewModel> model = new List<LeaveCountViewModel>();

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
        public IActionResult BindGridData(string id, string UserName)
        {
            userid = id;
            username = UserName;
            LeaveCountViewModel levcunt = new LeaveCountViewModel();
            var leavecount = _context.LeaveCount
                                .Where(w => w.Userid == id)
                               .Select(s => new LeaveCountViewModel()
                               {
                                   Id = s.Id,
                                   Userid = s.Userid,
                                   Fromdate = s.Fromdate,
                                   Todate = s.Todate,
                                   Filename = s.FileUpload,
                                   Count = s.Count,
                                   Description = s.Description,
                                   Isapprove = s.Isapprove,
                                   ApproveDate = s.ApproveDate,
                                   Approveby = s.Approveby,
                               }).ToList();
            levcunt.List = leavecount;

            //return RedirectToAction(nameof(LeaveIndex), new { id = leaveCounts.Id > 0 ? leaveCounts.Id : 0 });

            return Json(levcunt);
        }
        public IActionResult BinddrpdwnData()
        {
            var leavecount = _userManager.Users
                                .Where(w => w.UserName != null)
                               .Select(s => new SelectListItem()
                               {
                                   Text = s.UserName,
                                   Value = s.Id.ToString()
                               }).ToList();
            return Json(leavecount);
        }

        //post submitted leavecount data. if todo.TodoId is null then create new, otherwise edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitForm([Bind("Id", "Userid", "Fromdate", "Todate", "Count", "Description", "ApproveDate", "FileUpload", "Isapprove")] LeaveCountViewModel leaveCounts)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData[StaticString.StatusMessage] = "Error: Model state not valid.";
                    return RedirectToAction(nameof(Form), new { id = leaveCounts.Id > 0 ? leaveCounts.Id : 0 });
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
                    newleaveCount.Userid = userid;
                    newleaveCount.Fromdate = leaveCounts.Fromdate;
                    newleaveCount.Todate = leaveCounts.Todate;
                    newleaveCount.Count = leaveCounts.Count;
                    newleaveCount.Description = leaveCounts.Description;
                    newleaveCount.FileUpload = leaveCounts.FileUpload.FileName.ToString(); ;
                    newleaveCount.ApproveDate = DateTime.Now;
                    newleaveCount.Isapprove = leaveCounts.Isapprove;
                    newleaveCount.CreatedBy = user.Id;
                    newleaveCount.Approveby = user.Id;
                    leaveHistory.Userid = userid;
                    leaveHistory.Fromdate = leaveCounts.Fromdate;
                    leaveHistory.Todate = leaveCounts.Todate;
                    leaveHistory.Count = leaveCounts.Count;
                    leaveHistory.Description = leaveCounts.Description;
                    leaveHistory.FileUpload = leaveCounts.FileUpload.FileName.ToString();
                    leaveHistory.ApproveDate = DateTime.Now;
                    leaveHistory.Isapprove = leaveCounts.Isapprove;
                    leaveHistory.CreatedBy = userid;
                    leaveHistory.Approveby = user.Id;
                    _context.LeaveCount.Add(newleaveCount);
                    _context.LeaveHistory.Add(leaveHistory);
                    _context.SaveChanges();

                    TempData[StaticString.StatusMessage] = "Create new leave count item success.";
                    return RedirectToAction(nameof(Form), new { id = leaveCounts.Id > 0 ? leaveCounts.Id : 0 });
                }

                //edit existing
                LeaveCount editLeavecount = new LeaveCount();
                LeaveHistory addleaveHistory = new LeaveHistory();
                editLeavecount = _context.LeaveCount.Where(x => x.Id.Equals(leaveCounts.Id)).FirstOrDefault();
                editLeavecount.Userid = userid;
                editLeavecount.Fromdate = leaveCounts.Fromdate;
                editLeavecount.Todate = leaveCounts.Todate;
                editLeavecount.Count = leaveCounts.Count;
                editLeavecount.Description = leaveCounts.Description;
                editLeavecount.FileUpload = leaveCounts.FileUpload.FileName.ToString(); 
                editLeavecount.UpdatedBy = user.Id;
                editLeavecount.Isapprove = true;
                editLeavecount.Approveby = user.Id;
                editLeavecount.ApproveDate = DateTime.Now;
                addleaveHistory.Userid = userid;
                addleaveHistory.Fromdate = leaveCounts.Fromdate;
                addleaveHistory.Todate = leaveCounts.Todate;
                addleaveHistory.Count = leaveCounts.Count;
                addleaveHistory.Description = leaveCounts.Description;
                addleaveHistory.FileUpload = leaveCounts.FileUpload.FileName.ToString();
                addleaveHistory.UpdatedBy = user.Id;
                addleaveHistory.Isapprove = true;
                addleaveHistory.Approveby = user.Id;
                addleaveHistory.ApproveDate = DateTime.Now;
                _context.Update(editLeavecount);
                _context.Add(addleaveHistory);
                _context.SaveChanges();

                TempData[StaticString.StatusMessage] = "Edit existing leave count item success.";
                return RedirectToAction(nameof(Form), new { id = leaveCounts.Id > 0 ? leaveCounts.Id : 0 });
            }
            catch (Exception ex)
            {

                TempData[StaticString.StatusMessage] = "Error: " + ex.Message;
                return RedirectToAction(nameof(Form), new { id = leaveCounts.Id > 0 ? leaveCounts.Id : 0 });
            }
        }

        //display leavecount create edit form
        [HttpGet]
        public IActionResult Form(int id)
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

            //create new
            if (id == 0)
            {
                LeaveCount newleavecount = new LeaveCount();
                newleavecount.Userid = username;
                return View(newleavecount);
            }

            //edit leavecount master
            LeaveCount editnewleavecount = new LeaveCount();
            editnewleavecount = _context.LeaveCount.Where(x => x.Id.Equals(id)).FirstOrDefault();
            editnewleavecount.Userid = username;
            if (editnewleavecount == null)
            {
                return NotFound();
            }

            return View(editnewleavecount);

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
                return RedirectToAction(nameof(Delete), new { id = leave.Id > 0 ? leave.Id : 0 });
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

        public FileResult DownloadFile(string fileName)
        {
            //Build the File Path.
            string path = Path.Combine(this._webHostEnvironment.WebRootPath, "document/Leave/") + fileName;

            //Read the File data into Byte Array.
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            //Send the File to Download.
            return File(bytes, "application/octet-stream", fileName);
        }

        [HttpGet]
        public IActionResult EditData(int id)
        {
            var Data = _context.LeaveCount.Where(x => x.Id == id).FirstOrDefault();
            return Json(Data);
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


