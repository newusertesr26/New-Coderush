using coderush.Data;
using coderush.DataEnum;
using coderush.Models;
using coderush.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coderush.Controllers
{
   // [Authorize(Roles = Services.App.Pages.ProjectMaster.RoleName)]
    public class ProjectMasterController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        //private readonly IHostEnvironment _hostingEnvironment;
        public ProjectMasterController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager,
           RoleManager<IdentityRole> roleManager,
           ApplicationDbContext context)
        //IHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
            //_hostingEnvironment = hostingEnvironment;
        }
        public IActionResult ProjectIndex()
        {
            ViewBag.projectTechnologiesList = Enum.GetValues(typeof(Technologies)).Cast<Technologies>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString(),
            }).ToList();
            //ViewBag.Role = HttpContext.Session.GetString("Role");
            return View(_context.ProjectMaster.Where(x => !x.isdeleted).ToList());
        }

        //post submitted projectMasters data. if todo.TodoId is null then create new, otherwise edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitForm([Bind("Id", "ProjectName", "Technologies", "Description", "ManagerName", "DeveloperName", "paymenttype", "projectamount", "currency", "isactive")] ProjectMaster projectMasters)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData[StaticString.StatusMessage] = "Error: Model state not valid.";
                    return RedirectToAction(nameof(Form), new { id = projectMasters.Id > 0 ? projectMasters.Id : 0 });
                }

                var user = _userManager.GetUserAsync(User).Result;

                //create new
                if (projectMasters.Id == 0)
                {
                    ProjectMaster newprojectMaster = new ProjectMaster();
                    newprojectMaster.ProjectName = projectMasters.ProjectName;
                    newprojectMaster.CreatedDate = DateTime.Now;
                    newprojectMaster.Technologies = projectMasters.Technologies;
                    newprojectMaster.Description = projectMasters.Description;
                    newprojectMaster.ManagerName = projectMasters.ManagerName;
                    newprojectMaster.DeveloperName = projectMasters.DeveloperName;
                    newprojectMaster.paymenttype = projectMasters.paymenttype;
                    newprojectMaster.projectamount = projectMasters.projectamount;
                    newprojectMaster.currency = projectMasters.currency;
                    newprojectMaster.isactive = projectMasters.isactive;
                    newprojectMaster.CreatedBy = user.Id;
                    _context.ProjectMaster.Add(newprojectMaster);
                    _context.SaveChanges();

                    TempData[StaticString.StatusMessage] = "Create new project Masters item success.";
                    return RedirectToAction(nameof(Form), new { id = projectMasters.Id > 0 ? projectMasters.Id : 0 });
                }

                //edit existing
                ProjectMaster editProjectmaster = new ProjectMaster();
                editProjectmaster = _context.ProjectMaster.Where(x => x.Id.Equals(projectMasters.Id)).FirstOrDefault();
                editProjectmaster.ProjectName = projectMasters.ProjectName;
                editProjectmaster.Technologies = projectMasters.Technologies;
                editProjectmaster.Description = projectMasters.Description;
                editProjectmaster.ManagerName = projectMasters.ManagerName;
                editProjectmaster.DeveloperName = projectMasters.DeveloperName;
                editProjectmaster.paymenttype = projectMasters.paymenttype;
                editProjectmaster.projectamount = projectMasters.projectamount;
                editProjectmaster.currency = projectMasters.currency;
                editProjectmaster.UpdatedBy = user.Id;
                editProjectmaster.UpdatedDate = DateTime.Now;
                editProjectmaster.isactive = projectMasters.isactive;
                _context.Update(editProjectmaster);
                _context.SaveChanges();

                TempData[StaticString.StatusMessage] = "Edit existing project Masters item success.";
                return RedirectToAction(nameof(Form), new { id = projectMasters.Id > 0 ? projectMasters.Id : 0 });
            }
            catch (Exception ex)
            {

                TempData[StaticString.StatusMessage] = "Error: " + ex.Message;
                return RedirectToAction(nameof(Form), new { id = projectMasters.Id > 0 ? projectMasters.Id : 0 });
            }
        }

        //display datamaster create edit form
        [HttpGet]
        public IActionResult Form(int id)
        {
            ViewBag.projectTechnologiesList = Enum.GetValues(typeof(Technologies)).Cast<Technologies>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString(),
            }).ToList();

            //create new
            if (id == 0)
            {
                ProjectMaster newprojectmaster = new ProjectMaster();
                return View(newprojectmaster);
            }

            //edit ProjectMaster  
            ProjectMaster editnewprojectmaster = new ProjectMaster();
            editnewprojectmaster = _context.ProjectMaster.Where(x => x.Id.Equals(id)).FirstOrDefault();

            if (editnewprojectmaster == null)
            {
                return NotFound();
            }

            return View(editnewprojectmaster);

        }

        //display project master item for deletion
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var projectmaster = _context.ProjectMaster.Where(x => x.Id.Equals(id)).FirstOrDefault();
            return View(projectmaster);
        }

        //delete submitted project master item if found, otherwise 404
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitDelete([Bind("Id")] ProjectMaster project)
        {
            try
            {
                var deleteprojectmaster = _context.ProjectMaster.Where(x => x.Id.Equals(project.Id)).FirstOrDefault();
                if (deleteprojectmaster == null)
                {
                    return NotFound();
                }

                deleteprojectmaster.isdeleted = true;
                _context.ProjectMaster.Update(deleteprojectmaster);
                _context.SaveChanges();

                TempData[StaticString.StatusMessage] = "Delete project master item success.";
                return RedirectToAction(nameof(ProjectIndex));
            }
            catch (Exception ex)
            {

                TempData[StaticString.StatusMessage] = "Error: " + ex.Message;
                return RedirectToAction(nameof(Delete), new { id = project.Id > 0 ? project.Id : 0 });
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
            var Data = _context.ProjectMaster.Where(x => x.Id == id).FirstOrDefault();
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


