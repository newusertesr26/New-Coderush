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
using Microsoft.AspNetCore.Http;
using coderush.Models.ViewModels;

namespace coderush.Controllers
{
    //[Authorize(Roles = Services.App.Pages.InvoiceMaster.RoleName)]
    public class InvoiceMasterController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        //private readonly IHostEnvironment _hostingEnvironment;
        public InvoiceMasterController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager,
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
        public IActionResult InvoiceIndex()
        {
            //ViewBag.InvoiceSelectionList = Enum.GetValues(typeof(Expenses)).Cast<Expenses>().Select(v => new SelectListItem
            //{
            //    Text = v.ToString(),
            //    Value = ((int)v).ToString(),
            //}).ToList();

            //ViewBag.Role = HttpContext.Session.GetString("Role");+
            var data = _context.InvoiceMaster.Where(x => !x.Isdeleted).ToList();
            return View(data);
        }
        [HttpGet]
        //public IActionResult BindDropDown()
        //{
        //    //var devicemakelist = _context.ProjectMaster
        //    //                    .Where(w => w.isactive == true)
        //    //                   .Select(s => new SelectListItem()
        //    //                   {
        //    //                       Text = s.ProjectName,
        //    //                       Value = s.Id.ToString()
        //    //                   }).ToList();
        //    //return Json(devicemakelist);
        //}
        [HttpGet]
        public IActionResult Bindamount(int ProjectId)
        {
            var projectAmount = _context.ProjectMaster
                                .Where(w => w.Id == ProjectId)
                               .Select(s => s.projectamount).FirstOrDefault();
            return Json(projectAmount);
        }

        //post submitted todo data. if todo.TodoId is null then create new, otherwise edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitForm([Bind("Id", "ProjectId", "Amount", "Duedate", "InvoiceNumber", "PendingAmount", "Isactive")] InvoiceMaster invoiceMasters)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData[StaticString.StatusMessage] = "Error: Model state not valid.";
                    return RedirectToAction(nameof(Form), new { id = invoiceMasters.Id > 0 ? invoiceMasters.Id : 0 });
                }

                var user = _userManager.GetUserAsync(User).Result;

                //create new
                if (invoiceMasters.Id == 0)
                {
                    InvoiceMaster newinvoiceMaster = new InvoiceMaster();
                    newinvoiceMaster.ProjectId = invoiceMasters.ProjectId;
                    newinvoiceMaster.CreatedDate = DateTime.Now;
                    newinvoiceMaster.Amount = invoiceMasters.Amount;
                    newinvoiceMaster.Duedate = DateTime.Now;
                    newinvoiceMaster.InvoiceNumber = invoiceMasters.InvoiceNumber;
                    newinvoiceMaster.PendingAmount = invoiceMasters.PendingAmount;
                    newinvoiceMaster.Isverify = true;
                    newinvoiceMaster.CreatedBy = user.Id;
                    _context.InvoiceMaster.Add(newinvoiceMaster);
                    _context.SaveChanges();

                    TempData[StaticString.StatusMessage] = "Create new Invoice master item success.";
                    return RedirectToAction(nameof(Form), new { id = invoiceMasters.Id > 0 ? invoiceMasters.Id : 0 });
                }

                //edit existing
                InvoiceMaster editInvoicemaster = new InvoiceMaster();
                editInvoicemaster = _context.InvoiceMaster.Where(x => x.Id.Equals(invoiceMasters.Id)).FirstOrDefault();
                editInvoicemaster.ProjectId = invoiceMasters.ProjectId;
                editInvoicemaster.Amount = invoiceMasters.Amount;
                editInvoicemaster.InvoiceNumber = invoiceMasters.InvoiceNumber;
                editInvoicemaster.PendingAmount = invoiceMasters.PendingAmount;
                editInvoicemaster.Duedate = DateTime.Now;
                editInvoicemaster.UpdatedBy = user.Id;
                editInvoicemaster.UpdatedDate = DateTime.Now;
                editInvoicemaster.Isverify = true;
                _context.Update(editInvoicemaster);
                _context.SaveChanges();

                TempData[StaticString.StatusMessage] = "Edit existing invoice Masters item success.";
                return RedirectToAction(nameof(Form), new { id = invoiceMasters.Id > 0 ? invoiceMasters.Id : 0 });
            }
            catch (Exception ex)
            {

                TempData[StaticString.StatusMessage] = "Error: " + ex.Message;
                return RedirectToAction(nameof(Form), new { id = invoiceMasters.Id > 0 ? invoiceMasters.Id : 0 });
            }
        }

        //display invoiceMasters create edit form
        [HttpGet]
        public IActionResult Form(int id)
        {
            ViewBag.ProjectMasterSelectionList = _context.ProjectMaster
                               .Where(w => w.isactive == true)
                              .Select(s => new SelectListItem()
                              {
                                  Text = s.ProjectName,
                                  Value = s.Id.ToString()
                              }).ToList();
            //ViewBag.SelectionList = Enum.GetValues(typeof(DataSelection)).Cast<DataSelection>().Select(v => new SelectListItem
            //{
            //    Text = v.ToString(),
            //    Value = ((int)v).ToString(),
            //}).ToList();

            //create new
            if (id == 0)
            {
                InvoiceMaster newinvoicemaster = new InvoiceMaster();
                return View(newinvoicemaster);
            }

            //edit invoice master
            InvoiceMaster editnewinvoicemaster = new InvoiceMaster();
            editnewinvoicemaster = _context.InvoiceMaster.Where(x => x.Id.Equals(id)).FirstOrDefault();

            if (editnewinvoicemaster == null)
            {
                return NotFound();
            }

            return View(editnewinvoicemaster);

        }

        //display invoice master item for deletion
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var invoicemaster = _context.InvoiceMaster.Where(x => x.Id.Equals(id)).FirstOrDefault();
            return View(invoicemaster);
        }

        //delete submitted Invoice master item if found, otherwise 404
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitDelete([Bind("Id")] InvoiceMaster Invoice)
        {
            try
            {
                var deleteInvoicemaster = _context.InvoiceMaster.Where(x => x.Id.Equals(Invoice.Id)).FirstOrDefault();
                if (deleteInvoicemaster == null)
                {
                    return NotFound();
                }

                deleteInvoicemaster.Isdeleted = true;
                _context.InvoiceMaster.Update(deleteInvoicemaster);
                _context.SaveChanges();

                TempData[StaticString.StatusMessage] = "Delete Invoice master item success.";
                return RedirectToAction(nameof(InvoiceIndex));
            }
            catch (Exception ex)
            {

                TempData[StaticString.StatusMessage] = "Error: " + ex.Message;
                return RedirectToAction(nameof(Delete), new { id = Invoice.Id > 0 ? Invoice.Id : 0 });
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
            var Data = _context.InvoiceMaster.Where(x => x.Id == id).FirstOrDefault();
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


