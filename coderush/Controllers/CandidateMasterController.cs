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

namespace coderush.Controllers
{
    //[Authorize(Roles = Services.App.Pages.CandidateMaster.RoleName)]
    public class CandidateMasterController : Controller
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
        public IActionResult CandidateIndex()
        {
            ViewBag.CandidatetechnologiesList = Enum.GetValues(typeof(CandidateTechnologies)).Cast<CandidateTechnologies>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString(),
            }).ToList();
            //ViewBag.Role = HttpContext.Session.GetString("Role");
            //if (HttpContext.Session.GetString("Role") == "Other")
            //{
            //    return RedirectToAction("PageError", "Home");
            //}

            return View(_context.CandidateMaster.Where(x => !x.IsDelete).ToList());
        }

        //post submitted candidate data. if todo.CandidateId is null then create new, otherwise edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitForm([Bind("Id", "Name", "Email", "Phone", "Technologies", "FileUpload", "IsActive")] CandidateMastersViewModel candidateMasters)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData[StaticString.StatusMessage] = "Error: Model state not valid.";
                    return RedirectToAction(nameof(Form), new { id = candidateMasters.Id > 0 ? candidateMasters.Id : 0 });
                }

                var user = _userManager.GetUserAsync(User).Result;
                //{
                //    var file = Request.Form.Files;
                //    //for (int i = 0; i < file.Count; i++)
                //    //{
                //    var uploadefile = file[0];
                //    var filename = Path.GetFileName(uploadefile.ToString());
                //    var file1 = file[0];
                //    var filepath = Path.Combine(_webHostEnvironment.WebRootPath, "document/Candidate", filename);
                //    string savePath = Path.Combine(_webHostEnvironment.WebRootPath, "document/Candidate", filename);

                //    using (var inputStream = new FileStream(savePath, FileMode.Create))
                //    {
                //        //read file to stream
                //        file1.CopyTo(inputStream);
                //        //stream to byte array
                //    }

                //    //candidateMasters.FileUpload = filepath;
                //}

                string wwwPath = this._webHostEnvironment.WebRootPath;
                string contentPath = this._webHostEnvironment.ContentRootPath;
                var filename = candidateMasters.FileUpload.FileName;
                string path = Path.Combine(this._webHostEnvironment.WebRootPath, "document/Candidate");
                //if (!Directory.Exists(path))
                //{
                //    Directory.CreateDirectory(path);
                //}

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
                    newcandidateMaster.FileUpload = candidateMasters.FileUpload.FileName.ToString();
                    newcandidateMaster.IsActive = candidateMasters.IsActive;
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
                editCandidatemaster.FileUpload = candidateMasters.FileUpload.ToString();
                editCandidatemaster.UpdatedBy = user.Id;
                editCandidatemaster.UpdatedDate = DateTime.Now;
                editCandidatemaster.IsActive = candidateMasters.IsActive;
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
            ViewBag.CandidatetechnologiesList = Enum.GetValues(typeof(Technologies)).Cast<Technologies>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString(),
            }).ToList();

            //create new
            if (id == 0)
            {
                CandidateMastersViewModel newcandidatemaster = new CandidateMastersViewModel();
                return View(newcandidatemaster);
            }

            //edit candidate master
            CandidateMastersViewModel editnewcandidatemaster = new CandidateMastersViewModel();
            var data = _context.CandidateMaster.Where(x => x.Id.Equals(id)).FirstOrDefault();
            editnewcandidatemaster.Id = data.Id;


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


    }
}