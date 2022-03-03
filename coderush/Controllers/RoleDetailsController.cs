using coderush.Data;
using coderush.Models;
using coderush.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coderush.Controllers
{

    public class RoleDetailsController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        //private readonly IHostEnvironment _hostingEnvironment;
        public RoleDetailsController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager,
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

        public IActionResult RoleIndex()
        {
            //List<RoleDetails> model = new List<RoleDetails>();
            ////ViewData["roles"] = _roleManager.Roles.ToList();
            //return View(model);
            var roledetail = _context.RoleDetails.Where(x => !x.Isdelete).ToList();
            return View(roledetail);
        }

        //post submitted todo data. if role.roleId is null then create new, otherwise edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitForm([Bind("PageId", "Pagename", "Rolename", "Isactive")] RoleDetailsViewModel roleDetails)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData[StaticString.StatusMessage] = "Error: Model state not valid.";
                    return RedirectToAction(nameof(Form), new { id = roleDetails.PageId > 0 ? roleDetails.PageId : 0 });
                }
                
                var user = _userManager.GetUserAsync(User).Result;

                //create new
                if (roleDetails.PageId == 0)
                {
                    foreach(var i in roleDetails.Rolename)
                    {
                        RoleDetails newrolededatils = new RoleDetails();
                        newrolededatils.Pagename = roleDetails.Pagename;
                        newrolededatils.Rolename = i;
                        newrolededatils.Isactive = roleDetails.Isactive;
                        _context.RoleDetails.Add(newrolededatils);
                        _context.SaveChanges();
                    }
                   

                    TempData[StaticString.StatusMessage] = "Create new role details item success.";
                    return RedirectToAction(nameof(Form), new { id = roleDetails.PageId > 0 ? roleDetails.PageId : 0 });
                }

                //edit existing
                RoleDetails editRoledetails = new RoleDetails();
                editRoledetails = _context.RoleDetails.Where(x => x.PageId.Equals(roleDetails.PageId)).FirstOrDefault();
                editRoledetails.Pagename = roleDetails.Pagename;
                editRoledetails.Rolename = roleDetails.Rolename.FirstOrDefault();
                editRoledetails.Isactive = roleDetails.Isactive;
                _context.Update(editRoledetails);
                _context.SaveChanges();

                TempData[StaticString.StatusMessage] = "Edit existing role details item success.";
                return RedirectToAction(nameof(Form), new { id = roleDetails.PageId > 0 ? roleDetails.PageId : 0 });
            }
            catch (Exception ex)
            {

                TempData[StaticString.StatusMessage] = "Error: " + ex.Message;
                return RedirectToAction(nameof(Form), new { id = roleDetails.PageId > 0 ? roleDetails.PageId : 0 });
            }
        }

        //display role create edit form
        [HttpGet]
        public IActionResult Form(int id)
        {
            //create new
            if (id == 0)
            {
                ViewData["roles"] = _roleManager.Roles.ToList();
                RoleDetailsViewModel newroledetails = new RoleDetailsViewModel();
                return View(newroledetails);
            }

            //edit RoleDetails  
            ViewData["roles"] = _roleManager.Roles.ToList();
            RoleDetailsViewModel editnewroledetails = new RoleDetailsViewModel();
            var edit = _context.RoleDetails.Where(x => x.PageId.Equals(id)).FirstOrDefault();

            editnewroledetails.Pagename = edit.Pagename;
            //editnewroledetails.Rolename = new string[];
            editnewroledetails.Isactive = edit.Isactive;



            if (editnewroledetails == null)
            {
                return NotFound();
            }

            return View(editnewroledetails);

        }

        //display role details item for deletion
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var roledetails = _context.RoleDetails.Where(x => x.PageId.Equals(id)).FirstOrDefault();
            return View(roledetails);
        }

        //delete submitted Role Details item if found, otherwise 404
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitDelete([Bind("PageId")] RoleDetails roledeatil)
        {
            try
            {
                var deleteroledetalis = _context.RoleDetails.Where(x => x.PageId.Equals(roledeatil.PageId)).FirstOrDefault();
                if (deleteroledetalis == null)
                {
                    return NotFound();
                }

                deleteroledetalis.Isdelete = true;
                _context.RoleDetails.Update(deleteroledetalis);
                _context.SaveChanges();

                TempData[StaticString.StatusMessage] = "Delete Role Details item success.";
                return RedirectToAction(nameof(RoleIndex));
            }
            catch (Exception ex)
            {

                TempData[StaticString.StatusMessage] = "Error: " + ex.Message;
                return RedirectToAction(nameof(Delete), new { id = roledeatil.PageId > 0 ? roledeatil.PageId : 0 });
            }
        }

        [HttpGet]
        public IActionResult EditData(int id)
        {
            var Data = _context.RoleDetails.Where(x => x.PageId == id).FirstOrDefault();
            return Json(Data);
        }
    }

}
