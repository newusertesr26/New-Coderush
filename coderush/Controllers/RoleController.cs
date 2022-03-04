using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Identity;

namespace coderush.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly Services.Security.ICommon _security;

        //dependency injection through constructor, to directly access services
        public RoleController(Services.Security.ICommon security, RoleManager<IdentityRole> roleManager)
        {
            _security = security;
            _roleManager = roleManager;
        }

        //consume custom security service to get all roles
        public IActionResult Index()
        {
            List<string> roles = new List<string>();
            roles = _security.GetAllRoles();
            return View(roles);
        }

        public IActionResult Create()
        {
            return View(new IdentityRole());
        }

        [HttpPost]
        public async Task<IActionResult> Create(IdentityRole role)
        {
            role.NormalizedName = role.Name.ToUpper();
            await _roleManager.CreateAsync(role);
            return RedirectToAction("Index");
        }
    }
}