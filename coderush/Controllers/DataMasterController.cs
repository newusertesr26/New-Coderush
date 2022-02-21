using coderush.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using DemoCreate.DataEnum;
using Microsoft.AspNetCore.Mvc.Rendering;
using coderush.Models;
using Microsoft.AspNetCore.Authorization;
using coderush.Services.Security;
using Microsoft.Extensions.Options;

namespace coderush.Controllers
{
    [Authorize(Roles = Services.App.Pages.Membership.RoleName)]
    public class DataMasterController : Controller
    {
        private readonly Services.Security.ICommon _security;
        private readonly IdentityDefaultOptions _identityDefaultOptions;
        private readonly SuperAdminDefaultOptions _superAdminDefaultOptions;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        //dependency injection through constructor, to directly access services
        public DataMasterController(
            Services.Security.ICommon security,
            IOptions<IdentityDefaultOptions> identityDefaultOptions,
            IOptions<SuperAdminDefaultOptions> superAdminDefaultOptions,
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager
            )
        {
            _security = security;
            _identityDefaultOptions = identityDefaultOptions.Value;
            _superAdminDefaultOptions = superAdminDefaultOptions.Value;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult DataMasters()

        {
            ViewBag.SelectionList = Enum.GetValues(typeof(DataSelection)).Cast<DataSelection>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString(),
            }).ToList();
            //ViewBag.Role = HttpContext.Session.GetString("Role");
            return View(_context.Datamaster.Where(x => !x.Isdeleted).ToList());
        }

        [HttpGet]
        public IActionResult Data(string id)
        {
            //create new
            if (id == null)
            {
                DataMaster newdata = new DataMaster();
                return View(newdata);
            }

            //edit todo
            DataMaster data = new DataMaster();
            data = _context.Datamaster.Where(x => x.Id.Equals(id)).FirstOrDefault();

            if (data == null)
            {
                return NotFound();
            }

            return View(data);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitDataForm([Bind("Id", "Type", "Isactive")] DataMaster dataMaster)
        {
            bool IsDataExist = false;

            DataMaster data = await _context.Datamaster.FindAsync(dataMaster.Id);

            if (data != null)
            {
                IsDataExist = true;
            }
            else
            {
                data = new DataMaster();
            }
            var user = _userManager.GetUserAsync(User).Result;
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    TempData[StaticString.StatusMessage] = "Error: Model state not valid.";
                //    return RedirectToAction(nameof(Form), new { id = DataMaster.Id ?? "" });
                //}

                //create new
                if (dataMaster.Type == null)
                {
                    DataMaster newdata = new DataMaster();
                    newdata.Type = dataMaster.Type;
                    newdata.Text = dataMaster.Text;
                    newdata.Description = dataMaster.Description;
                    newdata.Isactive = dataMaster.Isactive;


                    if (IsDataExist)
                    {
                        data.UpdatedBy = user.Id;
                        data.UpdatedDate = DateTime.Now;
                        _context.Update(data);
                    }
                    else
                    {
                        data.CreatedBy = user.Id;
                        data.CreatedDate = DateTime.Now;
                        _context.Add(data);
                    }
                    _context.Datamaster.Add(dataMaster);
                    _context.SaveChanges();

                    TempData[StaticString.StatusMessage] = "Create new data item success.";
                    return RedirectToAction(nameof(Data), new { id = dataMaster.Id });
                }

                //edit existing
                DataMaster editdata = new DataMaster();
                editdata = _context.Datamaster.Where(x => x.Id.Equals(dataMaster.Id)).FirstOrDefault();
                editdata.Type = dataMaster.Type;
                editdata.Text = dataMaster.Text;
                editdata.Description = dataMaster.Description;
                _context.Update(editdata);
                _context.SaveChanges();

                TempData[StaticString.StatusMessage] = "Edit existing data item success.";
                return RedirectToAction(nameof(Data), new { id = dataMaster.Id });
            }
            catch (Exception ex)
            {

                TempData[StaticString.StatusMessage] = "Error: " + ex.Message;
                return RedirectToAction(nameof(Data), new { id = dataMaster.Id });
            }
        }

        [HttpGet]
        public IActionResult Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var data = _context.Datamaster.Where(x => x.Id.Equals(id)).FirstOrDefault();
            return View(data);
        }

        //delete submitted todo item if found, otherwise 404
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitDelete([Bind("Id")] DataMaster dataMaster)
        {
            try
            {
                var deletedata = _context.Datamaster.Where(x => x.Id.Equals(dataMaster.Id)).FirstOrDefault();
                if (deletedata == null)
                {
                    return NotFound();
                }

                _context.Datamaster.Remove(deletedata);
                _context.SaveChanges();

                TempData[StaticString.StatusMessage] = "Delete data item success.";
                return RedirectToAction(nameof(DataMasters));
            }
            catch (Exception ex)
            {

                TempData[StaticString.StatusMessage] = "Error: " + ex.Message;
                return RedirectToAction(nameof(Delete), new { id = dataMaster.Id });
            }
        }
    }
}