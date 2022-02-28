using coderush.Data;
using coderush.DataEnum;
using coderush.Models;
using coderush.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace coderush.Controllers
{
   // [Authorize(Roles = Services.App.Pages.ExpenseMaster.RoleName)]
    public class LeadMasterController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public LeadMasterController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager,
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
        public IActionResult LeadIndex()
        {
            ViewBag.LeadTechnologiesList = Enum.GetValues(typeof(LeadTechnologies)).Cast<LeadTechnologies>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString(),
            }).ToList();
            //ViewBag.Role = HttpContext.Session.GetString("Role");
            return View(_context.LeadMaster.Where(x => !x.IsDelete).ToList());
        }


        //post submitted todo data. if leadMasters.leadMastersId is null then create new, otherwise edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitForm([Bind("id", "Name", "Email", "Phone", "Technologies", "FileUpload", "IsActive")] LeadMastersViewModel leadMasters)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData[StaticString.StatusMessage] = "Error: Model state not valid.";
                    return RedirectToAction(nameof(Form), new { id = leadMasters.id > 0 ? leadMasters.id : 0 });
                }

                var user = _userManager.GetUserAsync(User).Result;

                string wwwPath = this._webHostEnvironment.WebRootPath;
                string contentPath = this._webHostEnvironment.ContentRootPath;
                var filename = leadMasters.FileUpload.FileName;
                string path = Path.Combine(this._webHostEnvironment.WebRootPath, "document/Lead");
                //if (!Directory.Exists(path))
                //{
                //    Directory.CreateDirectory(path);
                //}

                List<string> uploadedFiles = new List<string>();

                string fileName = Path.GetFileName(leadMasters.FileUpload.FileName);
                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    leadMasters.FileUpload.CopyTo(stream);
                    uploadedFiles.Add(fileName);
                }

                //create new
                if (leadMasters.id == 0)
                {
                    LeadMaster newleadMaster = new LeadMaster();
                    newleadMaster.Name = leadMasters.Name;
                    newleadMaster.CreatedDate = DateTime.Now;
                    newleadMaster.Email = leadMasters.Email;
                    newleadMaster.Phone = leadMasters.Phone;
                    newleadMaster.Technologies = leadMasters.Technologies;
                    newleadMaster.FileUpload = leadMasters.FileUpload.FileName.ToString(); ;
                    newleadMaster.IsActive = true;
                    newleadMaster.CreatedBy = user.Id;
                    _context.LeadMaster.Add(newleadMaster);
                    _context.SaveChanges();

                    TempData[StaticString.StatusMessage] = "Create new lead master item success.";
                    return RedirectToAction(nameof(Form), new { id = leadMasters.id > 0 ? leadMasters.id : 0 });
                }

                //edit existing
                LeadMaster editLeadmaster = new LeadMaster();
                editLeadmaster = _context.LeadMaster.Where(x => x.id.Equals(leadMasters.id)).FirstOrDefault();
                editLeadmaster.Name = leadMasters.Name;
                editLeadmaster.Email = leadMasters.Email;
                editLeadmaster.Phone = leadMasters.Phone;
                editLeadmaster.Technologies = leadMasters.Technologies;
                editLeadmaster.FileUpload = leadMasters.FileUpload.ToString();
                editLeadmaster.UpdatedBy = user.Id;
                editLeadmaster.UpdatedDate = DateTime.Now;
                editLeadmaster.IsActive = true;
                _context.Update(editLeadmaster);
                _context.SaveChanges();

                TempData[StaticString.StatusMessage] = "Edit existing lead master item success.";
                return RedirectToAction(nameof(Form), new { id = leadMasters.id > 0 ? leadMasters.id : 0 });
            }
            catch (Exception ex)
            {

                TempData[StaticString.StatusMessage] = "Error: " + ex.Message;
                return RedirectToAction(nameof(Form), new { id = leadMasters.id > 0 ? leadMasters.id : 0 });
            }
        }

        //display leadmaster create edit form
        [HttpGet]
        public IActionResult Form(int id)
        {
            ViewBag.LeadTechnologiesList = Enum.GetValues(typeof(LeadTechnologies)).Cast<LeadTechnologies>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString(),
            }).ToList();
            //ViewBag.Role = HttpContext.Session.GetString("Role");
            //return View(_context.LeadMaster.Where(x => !x.IsDelete).ToList());

            //create new
            if (id == 0)
            {
                LeadMastersViewModel newleadmaster = new LeadMastersViewModel();
                return View(newleadmaster);
            }

            //edit lead master
            LeadMastersViewModel editnewleadmaster = new LeadMastersViewModel();
            var data = _context.LeadMaster.Where(x => x.id.Equals(id)).FirstOrDefault();
            editnewleadmaster.id = data.id;


            if (editnewleadmaster == null)
            {
                return NotFound();
            }

            return View(editnewleadmaster);

        }

        //display lead master item for deletion
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var leadmaster = _context.LeadMaster.Where(x => x.id.Equals(id)).FirstOrDefault();
            return View(leadmaster);
        }

        //delete submitted lead master item if found, otherwise 404
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitDelete([Bind("Id")] LeadMaster lead)
        {
            try
            {
                var deleteleadmaster = _context.LeadMaster.Where(x => x.id.Equals(lead.id)).FirstOrDefault();
                if (deleteleadmaster == null)
                {
                    return NotFound();
                }

                deleteleadmaster.IsDelete = true;
                _context.LeadMaster.Update(deleteleadmaster);
                _context.SaveChanges();

                TempData[StaticString.StatusMessage] = "Delete lead master item success.";
                return RedirectToAction(nameof(LeadIndex));
            }
            catch (Exception ex)
            {

                TempData[StaticString.StatusMessage] = "Error: " + ex.Message;
                return RedirectToAction(nameof(Delete), new { id = lead.id > 0 ? lead.id : 0 });
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

        [HttpGet]
        public IActionResult EditData(int id)
        {
            var Data = _context.LeadMaster.Where(x => x.id == id).FirstOrDefault();
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


