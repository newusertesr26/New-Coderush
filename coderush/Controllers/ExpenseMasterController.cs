using coderush.Data;
using coderush.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using coderush.DataEnum;
using Microsoft.AspNetCore.Authorization;
using coderush.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace coderush.Controllers
{
    [Authorize(Roles = "SuperAdmin,HR")]
    public class ExpenseMasterController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        //private readonly IHostEnvironment _hostingEnvironment;
        public ExpenseMasterController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager,
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
        public IActionResult ExpenseIndex(string sdate, string edate, string curentmonth, string lastmont)
        {

            var user = _userManager.GetUserAsync(User).Result;

            ExpenseMasterViewModel serchadata = new ExpenseMasterViewModel();

             serchadata.List = (from expense in _context.ExpenseMaster
                        where expense.Isdelete == false
                        orderby expense.Id descending
                        select new ExpenseMasterViewModel
                        {
                            Id = expense.Id,
                            ExpName = expense.ExpName,
                            exptype = _context.Datamaster.Where(x => x.Id == expense.Exptype).Select(x => x.Text).FirstOrDefault(),
                            Amount = Convert.ToInt64(expense.Amount),
                            ExpenseDate = expense.ExpenseDate,
                            Description = expense.Description,
                            filename = expense.FileUpload,
                            isactive = expense.isactive,
                            CreatedDate = expense.CreatedDate,
                            CreatedBy = _userManager.Users.Where(x => x.Id == expense.CreatedBy).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault(),
                        }).ToList();

            //return View(data);
            var typelist1 = _context.Datamaster.Where(x => x.Type == DataSelection.Expenses).ToList();
            ViewBag.TotalCredit = _context.Credit.Sum(item => item.Amount);
            serchadata.TotalAmount = serchadata.List.Sum(item => item.Amount);
            ViewBag.TotalExpense = serchadata.TotalAmount;

            ViewBag.Expensetypelist = typelist1.Select(v => new SelectListItem
            {
                Text = v.Text.ToString(),
                Value = ((int)v.Id).ToString(),
            }).ToList();


            ViewBag.ExpensesList = Enum.GetValues(typeof(Expensestype)).Cast<Expensestype>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString(),

            }).ToList();
            //ViewBag.Role = HttpContext.Session.GetString("Role");
            try
            {
                int montha;
                if (curentmonth == "2")
                {
                    int dt = DateTime.Now.Month;

                    serchadata.List = serchadata.List.Where(x => x.Isdelete == false && x.ExpenseDate.Value.Month == dt).ToList();
                    //  return View(data);
                }
                else if (lastmont == "1")
                {
                    var today = DateTime.Today;
                    var month = new DateTime(today.Year, today.Month, 1);
                    var first = month.AddMonths(-1);
                    montha = first.Month;

                    serchadata.List = serchadata.List.Where(x => x.Isdelete == false && x.ExpenseDate.Value.Month == montha).ToList();
                    // return View(data);
                }


                else if (sdate != null && edate != null)
                {
                    ViewBag.startdate = sdate;
                    ViewBag.enddate = edate;
                    //serchadata = _context.ExpenseMaster.Where(x => !x.Isdelete && x.CreatedDate >= Convert.ToDateTime(sdate) && x.UpdatedDate <= Convert.ToDateTime(edate)).ToList();
                   // serchadata = _context.ExpenseMaster.Where(x => !x.Isdelete && x.CreatedDate >= Convert.ToDateTime(sdate)).ToList();
                    return View(serchadata);

                    serchadata.List = serchadata.List.Where(x => x.Isdelete == false
                                         && x.ExpenseDate >= Convert.ToDateTime(sdate) && x.ExpenseDate <= Convert.ToDateTime(edate)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
          

            return View(serchadata);
        }
        [HttpGet]
        public IActionResult Searchdata(string sdate, string edate)
        {

            var serchadata = new List<ExpenseMaster>();
            serchadata = _context.ExpenseMaster.Where(x => !x.Isdelete && x.CreatedDate >= Convert.ToDateTime(sdate) && x.UpdatedDate <= Convert.ToDateTime(edate)).ToList();
            return View(serchadata);
            //string y = null;
            //var searchingDate = y;
            //try
            //{
            //    ViewBag.ExpensesList = Enum.GetValues(typeof(Expensestype)).Cast<Expensestype>().Select(v => new SelectListItem
            //    {
            //        Text = v.ToString(),
            //        Value = ((int)v).ToString(),

            //    }).ToList();
            //    //ViewBag.Role = HttpContext.Session.GetString("Role");
            //    var Searching = _context.ExpenseMaster.ToList();
            // searchingDate = Convert.ToString(Searching.Where(s => s.CreatedDate >= Convert.ToDateTime(sdate) && s.UpdatedDate <= Convert.ToDateTime(edate)).ToList());

            //}
            //catch (Exception ex)
            //{

            //    throw;
            //}
            //return View(searchingDate);
        }


        public IActionResult SearchdataByCurrentmonth()
        {
            ViewBag.ExpensesList = Enum.GetValues(typeof(Expensestype)).Cast<Expensestype>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString(),

            }).ToList();
            //ViewBag.Role = HttpContext.Session.GetString("Role");
            var SearchingList = _context.ExpenseMaster.ToList();
            var searching = SearchingList.Where(x => x.CreatedDate == DateTime.Today.AddMonths(-1));

            return View(searching);




        }

        //post submitted expenseMasters data. if expenseMasters.expenseMastersId is null then create new, otherwise edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitForm([Bind("Id", "ExpName", "Exptype", "Amount", " ExpenseDate ", "Description", "FileUpload", "isactive")] ExpenseMasterViewModel expenseMasters)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData[StaticString.StatusMessage] = "Error: Model state not valid.";
                    return RedirectToAction(nameof(Form), new { id = expenseMasters.Id > 0 ? expenseMasters.Id : 0 });
                }

                var user = _userManager.GetUserAsync(User).Result;

                if (expenseMasters.FileUpload != null)
                {

                   string wwwPath = this._webHostEnvironment.WebRootPath;
                    string contentPath = this._webHostEnvironment.ContentRootPath;
                    var filename = expenseMasters.FileUpload.FileName;
                    string path = Path.Combine(this._webHostEnvironment.WebRootPath, "document/Expense");
                    //if (!Directory.Exists(path))
                    //{
                    //    Directory.CreateDirectory(path);
                    //}

                    List<string> uploadedFiles = new List<string>();

                    string fileName = Path.GetFileName(expenseMasters.FileUpload.FileName);
                    using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                    {
                        expenseMasters.FileUpload.CopyTo(stream);
                        uploadedFiles.Add(fileName);
                    }
                }

                //create new
                if (expenseMasters.Id == 0)
                {
                    ExpenseMaster newexpenseMaster = new ExpenseMaster();
                    newexpenseMaster.Description = expenseMasters.Description;
                    newexpenseMaster.CreatedDate = DateTime.Now;
                    newexpenseMaster.ExpName = expenseMasters.ExpName;
                    if (expenseMasters.FileUpload != null)
                    {
                        newexpenseMaster.FileUpload = expenseMasters.FileUpload.FileName.ToString();
                    }
                    else
                    {
                        newexpenseMaster.FileUpload = string.Empty;
                    }
                    newexpenseMaster.Exptype = expenseMasters.Exptype;
                    newexpenseMaster.Amount = expenseMasters.Amount.ToString();
                    newexpenseMaster.ExpenseDate = expenseMasters.ExpenseDate;
                    newexpenseMaster.isactive = expenseMasters.isactive;
                    newexpenseMaster.CreatedBy = user.Id.ToString();
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
                editexpensemaster.Amount = expenseMasters.Amount.ToString();
                editexpensemaster.ExpenseDate = expenseMasters.ExpenseDate;
                if (editexpensemaster.FileUpload != null)
                {
                    editexpensemaster.FileUpload = expenseMasters.FileUpload.FileName.ToString();
                }
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
            var typelist1 = _context.Datamaster.Where(x => x.Type == DataSelection.Expenses && x.Isactive==true).ToList();

            ViewBag.Expensetypelist = typelist1.Select(v => new SelectListItem
            {
                Text = v.Text.ToString(),
                Value = ((int)v.Id).ToString(),
            }).ToList();

            var user = _userManager.GetUserAsync(User).Result;
            //create new
            if (id == 0)
            {
                ExpenseMasterViewModel newexpensemaster = new ExpenseMasterViewModel();
                return View(newexpensemaster);
            }

            //edit expense master
            ExpenseMasterViewModel editnewexpensemaster = new ExpenseMasterViewModel();
            var expensemaster = _context.ExpenseMaster.Where(x => x.Id.Equals(id)).FirstOrDefault();
            editnewexpensemaster.Id = expensemaster.Id;
            editnewexpensemaster.ExpName = expensemaster.ExpName;
            editnewexpensemaster.Exptype = expensemaster.Exptype;
            editnewexpensemaster.Amount = Convert.ToInt64(expensemaster.Amount);
            editnewexpensemaster.ExpenseDate = expensemaster.ExpenseDate;
            //if (editexpensemaster.FileUpload != null)
            //{
            //    editexpensemaster.FileUpload = expenseMasters.FileUpload.FileName.ToString();
            //}
            editnewexpensemaster.Description = expensemaster.Description;
            editnewexpensemaster.UpdatedBy = user.Id;
            editnewexpensemaster.UpdatedDate = DateTime.Now;
            editnewexpensemaster.isactive = expensemaster.isactive;


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
                deleteexpensemaster.isactive = false;
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

        //public FileResult DownloadFile(string fileName)
        //{
        //    //Build the File Path.
        //    string path = Path.Combine(this._webHostEnvironment.WebRootPath, "document/Expense/") + fileName;

        //    //Read the File data into Byte Array.
        //    byte[] bytes = System.IO.File.ReadAllBytes(path);

        //    //Send the File to Download.
        //    return File(bytes, "application/octet-stream", fileName);
        //}


        [HttpGet]
        public IActionResult EditData(int id)
        {
            var Data = _context.ExpenseMaster.Where(x => x.Id == id).FirstOrDefault();
            return Json(Data);
        }

        public ActionResult Notes()
        {
            //if (id == 0)
            //{
            //    return null;
            //}

            Credit model = new Credit();

            var models = _context.Credit.ToList();
            
            return Json(models);
        }
        public ActionResult SaveNotes(int Id, int Amount, string Managername, string creditdate)
        {

            try
            {
                Credit models = new Credit();
                models.Id = Id;
                models.Amount = Amount;
                models.Managername = Managername;
                models.Creditdate = Convert.ToDateTime(creditdate);
                _context.Credit.Add(models);
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


