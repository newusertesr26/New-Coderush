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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using coderush.Models.ViewModels;

namespace coderush.Controllers
{
    [Authorize(Roles = "SuperAdmin,HR")]
    public class ExpenseMasterController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        //private readonly IHostEnvironment _hostingEnvironment;
        public ExpenseMasterController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager,
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
        public IActionResult ExpenseIndex()
        {
            ViewBag.ExpensesList = Enum.GetValues(typeof(Expensestype)).Cast<Expensestype>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString(),

            }).ToList();
            //ViewBag.Role = HttpContext.Session.GetString("Role");
            return View(_context.ExpenseMaster.Where(x => !x.Isdelete).ToList());
        }

        //post submitted expenseMasters data. if expenseMasters.expenseMastersId is null then create new, otherwise edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitForm([Bind("Id", "ExpName", "Exptype", "Amount", "Description", "isactive")] ExpenseMasterViewModel expenseMasters)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData[StaticString.StatusMessage] = "Error: Model state not valid.";
                    return RedirectToAction(nameof(Form), new { id = expenseMasters.Id > 0 ? expenseMasters.Id : 0 });
                }

                var user = _userManager.GetUserAsync(User).Result;

                //create new
                if (expenseMasters.Id == 0)
                {
                    ExpenseMaster newexpenseMaster = new ExpenseMaster();
                    newexpenseMaster.Description = expenseMasters.Description;
                    newexpenseMaster.CreatedDate = DateTime.Now;
                    newexpenseMaster.ExpName = expenseMasters.ExpName;
                    newexpenseMaster.Exptype = expenseMasters.Exptype;
                    newexpenseMaster.Amount = expenseMasters.Amount;
                    newexpenseMaster.isactive = expenseMasters.isactive;
                    newexpenseMaster.CreatedBy = user.Id;
                    _context.ExpenseMaster.Add(newexpenseMaster);
                    _context.SaveChanges();

                    TempData[StaticString.StatusMessage] = "Create new candidate master item success.";
                    return RedirectToAction(nameof(Form), new { id = expenseMasters.Id > 0 ? expenseMasters.Id : 0 });
                }

                //edit existing
                ExpenseMaster editexpensemaster = new ExpenseMaster();
                editexpensemaster = _context.ExpenseMaster.Where(x => x.Id.Equals(expenseMasters.Id)).FirstOrDefault();
                editexpensemaster.ExpName = expenseMasters.ExpName;
                editexpensemaster.Exptype = expenseMasters.Exptype;
                editexpensemaster.Amount = expenseMasters.Amount;
                editexpensemaster.Description = expenseMasters.Description;
                editexpensemaster.UpdatedBy = user.Id;
                editexpensemaster.UpdatedDate = DateTime.Now;
                editexpensemaster.isactive = expenseMasters.isactive;
                _context.Update(editexpensemaster);
                _context.SaveChanges();

                TempData[StaticString.StatusMessage] = "Edit existing expense masters  item success.";
                return RedirectToAction(nameof(Form), new { id = expenseMasters.Id > 0 ? expenseMasters.Id : 0 });
            }
            catch (Exception ex)
            {

                TempData[StaticString.StatusMessage] = "Error: " + ex.Message;
                return RedirectToAction(nameof(Form), new { id = expenseMasters.Id > 0 ? expenseMasters.Id : 0 });
            }
        }

        //display expensemaster create edit form
        [HttpGet]
        public IActionResult Form(int id)
        {
            ViewBag.ExpensesList = Enum.GetValues(typeof(Expensestype)).Cast<Expensestype>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString(),

            }).ToList();

            //create new
            if (id == 0)
            {
                ExpenseMasterViewModel newexpensemaster = new ExpenseMasterViewModel();
                return View(newexpensemaster);
            }

            //edit expense master
            ExpenseMasterViewModel editnewexpensemaster = new ExpenseMasterViewModel();
            var  expensemaster = _context.ExpenseMaster.Where(x => x.Id.Equals(id)).FirstOrDefault();
            editnewexpensemaster.Id = expensemaster.Id;


            if (editnewexpensemaster == null)
            {
                return NotFound();
            }

            return View(editnewexpensemaster);

        }

        //display expense master item for deletion
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var expensemaster = _context.ExpenseMaster.Where(x => x.Id.Equals(id)).FirstOrDefault();
            return View(expensemaster);
        }

        //delete submitted expense master item if found, otherwise 404
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitDelete([Bind("Id")] ExpenseMaster expense)
        {
            try
            {
                var deleteexpensemaster = _context.ExpenseMaster.Where(x => x.Id.Equals(expense.Id)).FirstOrDefault();
                if (deleteexpensemaster == null)
                {
                    return NotFound();
                }

                deleteexpensemaster.Isdelete = true;
                _context.ExpenseMaster.Update(deleteexpensemaster);
                _context.SaveChanges();

                TempData[StaticString.StatusMessage] = "Delete expense master item success.";
                return RedirectToAction(nameof(ExpenseIndex));
            }
            catch (Exception ex)
            {

                TempData[StaticString.StatusMessage] = "Error: " + ex.Message;
                return RedirectToAction(nameof(Delete), new { id = expense.Id > 0 ? expense.Id : 0 });
            }
        }

       

        [HttpGet]
        public IActionResult EditData(int id)
        {
            var Data = _context.ExpenseMaster.Where(x => x.Id == id).FirstOrDefault();
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


