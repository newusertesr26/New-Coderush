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

namespace coderush.Controllers
{
    [Authorize(Roles = "HR,SuperAdmin")]
    public class CandidateMasterController : Controller  //Harshal working on-->
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CandidateMasterController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager,
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
        public IActionResult CandidateIndex(string sdate, string edate, string lastmont, string technology)
        {
            var user = _userManager.GetUserAsync(User).Result;
            var typelist1 = _context.Datamaster.Where(x => x.Type == DataSelection.technologies).ToList();

            //TimeSpan timespan = new TimeSpan(03, 00, 00);
            //DateTime time = DateTime.Today.Add(timespan);
            //string displayTime = time.ToString("hh:mm tt"); 

            ViewBag.CandidatetechnologiesList = typelist1.Select(v => new SelectListItem
            {
                Text = v.Text.ToString(),
                Value = ((int)v.Id).ToString(),
            }).ToList();
            var data = new List<CandidateMastersViewModel>();

            DateTime? nulldate = null;

          
            var TODAYDATE = DateTime.Now.AddDays(-1);
            var top8date = _context.CandidateMaster.OrderByDescending(x => x.InterviewDate).Take(9).ToList();
            var maxdate = top8date.Where(x => x.InterviewDate > TODAYDATE).Max(x => x.InterviewDate);
            var mindate = top8date.Where(x => x.InterviewDate > TODAYDATE).Min(x => x.InterviewDate);
            data = (from candidate in _context.CandidateMaster.OrderByDescending(x => x.Id)
                    where !candidate.IsDelete
                    

                    //orderby candidate.Id descending
                    let commentdate = _context.Comments.OrderByDescending(x => x.Id).Where(w => w.CandidateId == candidate.Id).Select(s => s.NextFollowUpdate).FirstOrDefault()                   
                    //let After7Day = DateTime.Now.AddDays(+9)
                    select new CandidateMastersViewModel
                    {
                        Id = candidate.Id,
                        Name = candidate.Name,
                        Email = candidate.Email,
                        Phone = candidate.Phone,
                        Technologies =candidate.Technologies,
                        technologies = _context.Datamaster.Where(x => x.Id == candidate.Technologies).Select(x => x.Text).FirstOrDefault(),
                        filename = candidate.FileUpload,
                        IsActive = candidate.IsActive,
                        InterviewDate = candidate.InterviewDate,
                        PlaceOfInterview = candidate.PlaceOfInterview,
                        InterviewDescription = candidate.InterviewDescription,
                        InterviewTime = candidate.InterviewTime,
                        IsReject = candidate.IsReject,
                        Status = candidate.Status,
                        dateforNext = (commentdate != null ? commentdate : nulldate),
                        CreatedBy = _userManager.Users.Where(x => x.Id == candidate.CreatedBy).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault(),
                        CreatedDate = candidate.CreatedDate,
                        //Color = Last7Day < candidate.InterviewDate && TODAYDATE < candidate.InterviewDate ? "" : "#ffe0bb"
                        Color = (mindate <= candidate.InterviewDate) && (maxdate >= candidate.InterviewDate) ?  "#ffe0bb" : ""
                        //Color = TODAYDATE < candidate.InterviewDate ? "" : "#ffe0bb"

                    }).ToList();

            try
            {
                int montha;
                if (lastmont == "2")
                {
                    int dt = DateTime.Now.Month;

                  data = data.Where(w => w.InterviewDate.Value.Month == dt).ToList();
                }

                if (lastmont == "1")
                {
                    var today = DateTime.Today;
                    var month = new DateTime(today.Year, today.Month, 1);
                    var first = month.AddMonths(-1);
                    montha = first.Month;

                    data = data.Where(w => w.InterviewDate.Value.Month == montha).ToList();

                }
                if (lastmont == "0")
                {
                    data = data.ToList();
                }

                if (technology != null)
                {
                    var T = Convert.ToInt32(technology);
                    if (T == 0)
                    {
                        data = data.ToList();

                    }
                    else
                    {
                      
                        data = data.Where(w => w.Technologies == T).ToList();
                    }
                }

                if (sdate != null && edate != null)
                {
                    data = data.Where(w =>
                                             w.InterviewDate >= Convert.ToDateTime(sdate)
                                            && w.InterviewDate <= Convert.ToDateTime(edate)
                                           ).ToList();

                }


                ViewData["selectedtech"] = technology;
                ViewData["lastmonth"] = lastmont;
                ViewData["sdate"] = sdate;
                ViewData["edate"] = edate;
            }
               
               
            catch (Exception ex)
            {
                throw ex;
            }

            return View(data);
        }

        //ViewBag.Role = HttpContext.Session.GetString("Role");
        //if (HttpContext.Session.GetString("Role") == "Other")
        //{
        //    return RedirectToAction("PageError", "Home");
        //}

        //return View(_context.CandidateMaster.Where(x => !x.IsDelete).ToList());
        [HttpGet]
        public IActionResult Searchdata(string sdate, string edate)
        {
            var serchadata = new List<CandidateMaster>();
            serchadata = _context.CandidateMaster.Where(x => !x.IsDelete && x.CreatedDate >= Convert.ToDateTime(sdate) && x.UpdatedDate <= Convert.ToDateTime(edate)).ToList();
            return View(serchadata);
        }

        public IActionResult SearchdataByCurrentmonth()
        {
            ViewBag.CandidatetechnologiesList = Enum.GetValues(typeof(CandidateTechnologies)).Cast<CandidateTechnologies>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString(),

            }).ToList();
            //ViewBag.Role = HttpContext.Session.GetString("Role");
            var SearchingList = _context.CandidateMaster.ToList();
            var searching = SearchingList.Where(x => x.CreatedDate == DateTime.Today.AddMonths(-1));

            return View(searching);

        }

        //post submitted candidate data. if todo.CandidateId is null then create new, otherwise edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitForm([Bind("Id", "Name", "Email", "Phone", "Technologies", "FileUpload", "InterviewDate", "PlaceOfInterview", "InterviewTime", "dateforNext", "InterviewDescription", "IsActive", "IsReject", "Status")] CandidateMastersViewModel candidateMasters)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData[StaticString.StatusMessage] = "Error: Model state not valid.";
                    return RedirectToAction(nameof(Form), new { id = candidateMasters.Id > 0 ? candidateMasters.Id : 0 });
                }

                var user = _userManager.GetUserAsync(User).Result;

                string wwwPath = this._webHostEnvironment.WebRootPath;
                string contentPath = this._webHostEnvironment.ContentRootPath;
                var filename = candidateMasters.FileUpload.FileName;
                string path = Path.Combine(this._webHostEnvironment.WebRootPath, "document/Candidate");

                List<string> uploadedFiles = new List<string>();

                string fileName = Path.GetFileName(candidateMasters.FileUpload.FileName);
                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    candidateMasters.FileUpload.CopyTo(stream);
                    uploadedFiles.Add(fileName);
                }


                //}
                //create new
                if (candidateMasters.Id == 0)
                {
                    CandidateMaster newcandidateMaster = new CandidateMaster();
                    newcandidateMaster.Name = candidateMasters.Name;
                    newcandidateMaster.CreatedDate = DateTime.Now;
                    newcandidateMaster.Email = candidateMasters.Email;
                    newcandidateMaster.Phone = candidateMasters.Phone;
                    newcandidateMaster.Technologies = candidateMasters.Technologies;
                    newcandidateMaster.InterviewDate = candidateMasters.InterviewDate;
                    newcandidateMaster.PlaceOfInterview = candidateMasters.PlaceOfInterview;
                    newcandidateMaster.InterviewTime = candidateMasters.InterviewTime;
                    newcandidateMaster.InterviewDescription = candidateMasters.InterviewDescription;
                    newcandidateMaster.FileUpload = candidateMasters.FileUpload.FileName.ToString();
                    newcandidateMaster.IsActive = candidateMasters.IsActive;
                    newcandidateMaster.IsReject = candidateMasters.IsReject;
                    newcandidateMaster.Status = candidateMasters.Status;
                    //  newcandidateMaster.Schedule = candidateMasters.Schedule;
                    newcandidateMaster.CreatedBy = user.Id;
                    _context.CandidateMaster.Add(newcandidateMaster);
                    _context.SaveChanges();

                    TempData[StaticString.StatusMessage] = "Create new Candidate Master item success.";
                    return RedirectToAction(nameof(Form), new { id = candidateMasters.Id > 0 ? candidateMasters.Id : 0 });
                }

                //edit existing
                CandidateMaster editCandidatemaster = new CandidateMaster();
                editCandidatemaster = _context.CandidateMaster.Where(x => x.Id.Equals(candidateMasters.Id)).FirstOrDefault();
                editCandidatemaster.Name = candidateMasters.Name;
                editCandidatemaster.Email = candidateMasters.Email;
                editCandidatemaster.Phone = candidateMasters.Phone;
                editCandidatemaster.Technologies = candidateMasters.Technologies;
                editCandidatemaster.InterviewDate = candidateMasters.InterviewDate;
                editCandidatemaster.PlaceOfInterview = candidateMasters.PlaceOfInterview;
                editCandidatemaster.InterviewTime = candidateMasters.InterviewTime;
                editCandidatemaster.InterviewDescription = candidateMasters.InterviewDescription;
                editCandidatemaster.FileUpload = candidateMasters.FileUpload.ToString();
                editCandidatemaster.UpdatedDate = DateTime.Now;
                editCandidatemaster.IsActive = candidateMasters.IsActive;
                editCandidatemaster.UpdatedBy = user.Id;
                editCandidatemaster.IsReject = candidateMasters.IsReject;
                editCandidatemaster.Status = candidateMasters.Status;
                // editCandidatemaster.Schedule = candidateMasters.Schedule;
                _context.CandidateMaster.Update(editCandidatemaster);
                _context.SaveChanges();

                TempData[StaticString.StatusMessage] = "Edit existing candidate master item success.";
                return RedirectToAction(nameof(Form), new { id = candidateMasters.Id > 0 ? candidateMasters.Id : 0 });
            }
            catch (Exception ex)
            {

                TempData[StaticString.StatusMessage] = "Error: " + ex.Message;
                return RedirectToAction(nameof(Form), new { id = candidateMasters.Id > 0 ? candidateMasters.Id : 0 });
            }
        }

        //display CandidateMaster create edit form
        [HttpGet]
        public IActionResult Form(int id)
        {

            //ViewBag.CandidatetechnologiesList = Enum.GetValues(typeof(Technologies)).Cast<Technologies>().Select(v => new SelectListItem
            //{
            //    Text = v.ToString(),
            //    Value = ((int)v).ToString(),
            //}).ToList();
            var typelist1 = _context.Datamaster.Where(x => x.Type == DataSelection.technologies).ToList();

            ViewBag.CandidatetechnologiesList = typelist1.Select(v => new SelectListItem
            {
                Text = v.Text.ToString(),
                Value = v.Id.ToString(),
            }).ToList();


            //create new
            if (id == 0)
            {
                CandidateMastersViewModel newcandidatemaster = new CandidateMastersViewModel();
                return View(newcandidatemaster);
            }

            //edit candidate master
            CandidateMastersViewModel editnewcandidatemaster = new CandidateMastersViewModel();
            var candidatedata = _context.CandidateMaster.Where(x => x.Id.Equals(id)).FirstOrDefault();
            editnewcandidatemaster.Id = candidatedata.Id;
            editnewcandidatemaster.Name = candidatedata.Name;
            editnewcandidatemaster.Email = candidatedata.Email;
            editnewcandidatemaster.Phone = candidatedata.Phone;
            editnewcandidatemaster.Technologies = candidatedata.Technologies;
            editnewcandidatemaster.InterviewDate = candidatedata.InterviewDate;
            editnewcandidatemaster.InterviewTime = candidatedata.InterviewTime;
            editnewcandidatemaster.PlaceOfInterview = candidatedata.PlaceOfInterview;
            editnewcandidatemaster.filename = candidatedata.FileUpload;
            editnewcandidatemaster.IsReject = candidatedata.IsReject;
            editnewcandidatemaster.IsActive = candidatedata.IsActive;
            editnewcandidatemaster.Status = candidatedata.Status;
            editnewcandidatemaster.InterviewDescription = candidatedata.InterviewDescription;
            // editnewcandidatemaster.Schedule = candidatedata.Schedule;

            if (editnewcandidatemaster == null)
            {
                return NotFound();
            }

            return View(editnewcandidatemaster);

        }

        //display candidate master item for deletion
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var candidatemaster = _context.CandidateMaster.Where(x => x.Id.Equals(id)).FirstOrDefault();
            return View(candidatemaster);
        }

        //delete submitted candidate master item if found, otherwise 404
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitDelete([Bind("Id")] CandidateMaster candidate)
        {
            try
            {
                var deletecandidatemaster = _context.CandidateMaster.Where(x => x.Id.Equals(candidate.Id)).FirstOrDefault();
                if (deletecandidatemaster == null)
                {
                    return NotFound();
                }

                deletecandidatemaster.IsDelete = true;
                deletecandidatemaster.IsActive = false;
                _context.CandidateMaster.Update(deletecandidatemaster);
                _context.SaveChanges();

                TempData[StaticString.StatusMessage] = "Delete candidate master item success.";
                return RedirectToAction(nameof(CandidateIndex));
            }
            catch (Exception ex)
            {

                TempData[StaticString.StatusMessage] = "Error: " + ex.Message;
                return RedirectToAction(nameof(Delete), new { id = candidate.Id > 0 ? candidate.Id : 0 });
            }
        }


        [HttpGet]
        public IActionResult EditData(int id)
        {
            var candidate = _context.CandidateMaster.Where(x => x.Id == id).FirstOrDefault();
            return Json(candidate);
        }


        public ActionResult Notes(int id)
        {
            if (id == 0)
            {
                return null;
            }

            Comments model = new Comments();
          // ViewData["user"] = _userManager.GetUserAsync(User).Result;
            var models = _context.Comments.Where(x => x.CandidateId.Equals(id)).ToList();

            return Json(models);
        }
        [HttpPost]
        public ActionResult SaveNotes(Comments data)
        {

            try
            {
               var user = _userManager.GetUserAsync(User).Result;
                Comments models = new Comments();
                models.CandidateId = data.Id;
                models.Note = data.Note;
                models.NextFollowUpdate = data.NextFollowUpdate;
                models.Status = data.Status;
                models.LoginUser = user.Email;
                _context.Comments.Add(models);
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
