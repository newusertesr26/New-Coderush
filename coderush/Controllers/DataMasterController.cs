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
using coderush.Models.ViewModels;

namespace coderush.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class DataMasterController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        //private readonly IHostEnvironment _hostingEnvironment;
        public DataMasterController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager,
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
        public IActionResult Index()
        {
            ViewBag.SelectionList = Enum.GetValues(typeof(DataSelection)).Cast<DataSelection>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString(),
            }).ToList();

            var datamaster = _context.Datamaster.Where(x => !x.Isdeleted).ToList();
            return View(datamaster);
        }

        //post submitted todo data. if todo.TodoId is null then create new, otherwise edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitForm([Bind("Id", "Type", "Text", "Description", "Isactive")] DataMaster dataMasters)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData[StaticString.StatusMessage] = "Error: Model state not valid.";
                    return RedirectToAction(nameof(Form), new { id = dataMasters.Id > 0 ? dataMasters.Id : 0 });
                }

                var user = _userManager.GetUserAsync(User).Result;

                //create new
                if (dataMasters.Id == 0)
                {
                    DataMaster newdataMaster = new DataMaster();
                    newdataMaster.Description = dataMasters.Description;
                    newdataMaster.CreatedDate = DateTime.Now;
                    newdataMaster.Text = dataMasters.Text;
                    newdataMaster.Type = dataMasters.Type;
                    newdataMaster.Isactive = dataMasters.Isactive;
                    newdataMaster.CreatedBy = user.Id;
                    _context.Datamaster.Add(newdataMaster);
                    _context.SaveChanges();

                    TempData[StaticString.StatusMessage] = "Create new data master item success.";
                    return RedirectToAction(nameof(Form), new { id = dataMasters.Id > 0 ? dataMasters.Id : 0 });
                }

                //edit existing
                DataMaster editDatamaster = new DataMaster();
                editDatamaster = _context.Datamaster.Where(x => x.Id.Equals(dataMasters.Id)).FirstOrDefault();
                editDatamaster.Text = dataMasters.Text;
                editDatamaster.Description = dataMasters.Description;
                editDatamaster.UpdatedBy = user.Id;
                editDatamaster.UpdatedDate = DateTime.Now;
                editDatamaster.Isactive = dataMasters.Isactive;
                _context.Update(editDatamaster);
                _context.SaveChanges();

                TempData[StaticString.StatusMessage] = "Edit existing data master item success.";
                return RedirectToAction(nameof(Form), new { id = dataMasters.Id > 0 ? dataMasters.Id : 0 });
            }
            catch (Exception ex)
            {

                TempData[StaticString.StatusMessage] = "Error: " + ex.Message;
                return RedirectToAction(nameof(Form), new { id = dataMasters.Id > 0 ? dataMasters.Id : 0 });
            }
        }

        //display datamaster create edit form
        [HttpGet]
        public IActionResult Form(int id)
        {
            ViewBag.SelectionList = Enum.GetValues(typeof(DataSelection)).Cast<DataSelection>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString(),
            }).ToList();

            //create new
            if (id == 0)
            {
                DataMaster newdatamaster = new DataMaster();
                return View(newdatamaster);
            }

            //edit data master
            DataMaster editnewdatamaster = new DataMaster();
            editnewdatamaster = _context.Datamaster.Where(x => x.Id.Equals(id)).FirstOrDefault();

            if (editnewdatamaster == null)
            {
                return NotFound();
            }

            return View(editnewdatamaster);

        }

        //display data master item for deletion
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var datamaster = _context.Datamaster.Where(x => x.Id.Equals(id)).FirstOrDefault();
            return View(datamaster);
        }

        //delete submitted data master item if found, otherwise 404
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitDelete([Bind("Id")] DataMaster data)
        {
            try
            {
                var deletedatamaster = _context.Datamaster.Where(x => x.Id.Equals(data.Id)).FirstOrDefault();
                if (deletedatamaster == null)
                {
                    return NotFound();
                }

                deletedatamaster.Isdeleted = true;
                deletedatamaster.Isactive = false;
                _context.Datamaster.Update(deletedatamaster);
                _context.SaveChanges();

                TempData[StaticString.StatusMessage] = "Delete data master item success.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

                TempData[StaticString.StatusMessage] = "Error: " + ex.Message;
                return RedirectToAction(nameof(Delete), new { id = data.Id > 0 ? data.Id : 0 });
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
            var Data = _context.Datamaster.Where(x => x.Id == id).FirstOrDefault();
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


