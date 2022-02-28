using coderush.Data;
using coderush.Models;
using coderush.Models.ViewModels;
using coderush.Services.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace coderush.Controllers
{
    //[Authorize(Roles = Services.App.Pages.Home.RoleName)]
    public class HomeController : Controller
    {
        private readonly Services.Security.ICommon _security;
        private readonly IdentityDefaultOptions _identityDefaultOptions;
        private readonly SuperAdminDefaultOptions _superAdminDefaultOptions;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        //dependency injection through constructor, to directly access services
        public HomeController(
            Services.Security.ICommon security,
            IOptions<IdentityDefaultOptions> identityDefaultOptions,
            IOptions<SuperAdminDefaultOptions> superAdminDefaultOptions,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager
            )
        {
            _security = security;
            _identityDefaultOptions = identityDefaultOptions.Value;
            _superAdminDefaultOptions = superAdminDefaultOptions.Value;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            //return View();
            return LocalRedirect("/Identity/Account/Login");
        }

        //public async Task<IActionResult> NewLogin()
        //{
        //    var user = _userManager.GetUserAsync(User).Result;
        //    var adminrole = await _userManager.IsInRoleAsync(user, "HR");
        //    if (adminrole)
        //    {
        //        HttpContext.Session.SetString("Role", "HR");
        //        ViewBag.Role = HttpContext.Session.GetString("Role");
        //        //TempData["Role"] = "HR";
        //        //    ViewBag.HR = true;
        //    }
        //    else
        //    {
        //        HttpContext.Session.SetString("Role", "Other");
        //        ViewBag.Role = HttpContext.Session.GetString("Role");
        //        //TempData["Role"] = "Other";
        //        //  ViewBag.HR = false;
        //    }
        //    return View();
        //}


        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}

        //public IActionResult PageError()
        //{
        //    return View();
        //}
    }
}
