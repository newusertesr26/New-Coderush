using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using coderush.Data;
using coderush.Models;
using coderush.Models.ViewModels;
using coderush.Services.Security;
using coderush.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace coderush.Controllers
{
    //[Authorize(Roles = Services.App.Pages.Membership.RoleName)]
    public class MembershipController : Controller
    {
        private readonly Services.Security.ICommon _security;
        private readonly IdentityDefaultOptions _identityDefaultOptions;
        private readonly SuperAdminDefaultOptions _superAdminDefaultOptions;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        //dependency injection through constructor, to directly access services
        public MembershipController(
            Services.Security.ICommon security,
            IOptions<IdentityDefaultOptions> identityDefaultOptions,
            IOptions<SuperAdminDefaultOptions> superAdminDefaultOptions,
            ApplicationDbContext context,
             IWebHostEnvironment webHostEnvironment,
            UserManager<ApplicationUser> userManager
            )
        {
            _security = security;
            _identityDefaultOptions = identityDefaultOptions.Value;
            _superAdminDefaultOptions = superAdminDefaultOptions.Value;
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment; ;
        }

        public IActionResult Index()
        {            
            List<ApplicationUser> users = new List<ApplicationUser>();
            users = _security.GetAllMembers();
            return View(users);
        }

        //display change profile screen if member founded, otherwise 404
        [HttpGet]
        public IActionResult ChangeProfile(string id)
        {
          

            if (id == null)
            {
                return NotFound();
            }

           var appUser = _security.GetMemberByApplicationId(id);

            if (appUser == null)
            {
                return NotFound();
            }

            return View(appUser);
        }

        //post submited change profile request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitChangeProfile([Bind("Id,EmailConfirmed,Email,PhoneNumber,JoiningDate,ProfilePicture")] ApplicationViewModel applicationUser)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    TempData[StaticString.StatusMessage] = "Error: Model state not valid.";
                    return RedirectToAction(nameof(ChangeProfile), new { id = applicationUser.Id });
                }

                string wwwPath = this._webHostEnvironment.WebRootPath;
                string contentPath = this._webHostEnvironment.ContentRootPath;
                var filename = applicationUser.ProfilePicture.FileName.ToString();
                string path = Path.Combine(this._webHostEnvironment.WebRootPath, "document/Userimage");
                //if (!Directory.Exists(path))
                //{
                //    Directory.CreateDirectory(path);
                //}

                List<string> uploadedFiles = new List<string>();

                string fileName = Path.GetFileName(applicationUser.ProfilePicture.FileName);
                string filetext = Path.GetExtension(fileName);
                using (FileStream stream = new FileStream(Path.Combine(path, applicationUser.Id + filetext), FileMode.Create))
                {
                    applicationUser.ProfilePicture.CopyTo(stream);
                    uploadedFiles.Add(fileName);
                }

                var updatedUser = _security.GetMemberByApplicationId(applicationUser.Id);
                if (updatedUser == null)
                {
                    TempData[StaticString.StatusMessage] = "Error: Can not found the member.";
                    return RedirectToAction(nameof(ChangeProfile), new { id = applicationUser.Id });
                }

                if (_identityDefaultOptions.IsDemo && _superAdminDefaultOptions.Email.Equals(applicationUser.Email))
                {
                    TempData[StaticString.StatusMessage] = "Error: Demo mode can not change super@admin.com data.";
                    return RedirectToAction(nameof(ChangeProfile), new { id = applicationUser.Id });
                }                

                updatedUser.Email = applicationUser.Email;
                updatedUser.PhoneNumber = applicationUser.PhoneNumber;
                updatedUser.EmailConfirmed = applicationUser.EmailConfirmed;
                updatedUser.JoiningDate = applicationUser.JoiningDate;
                updatedUser.ProfilePicture = applicationUser.Id + filetext;
                

                _context.Update(updatedUser);
                await _context.SaveChangesAsync();

                TempData[StaticString.StatusMessage] = "Update success";
                return RedirectToAction(nameof(ChangeProfile), new { id = updatedUser.Id});
            }
            catch (Exception ex)
            {
                TempData[StaticString.StatusMessage] = "Error: " + ex.Message;
                return RedirectToAction(nameof(ChangeProfile), new { id = applicationUser.Id });
            }
            
        }

        //display change password screen if user founded, otherwise 404
        [HttpGet]
        public IActionResult ChangePassword(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = _security.GetMemberByApplicationId(id);
            if (member == null)
            {
                return NotFound();
            }

            ResetPassword cp = new ResetPassword();
            cp.Id = id;
            cp.UserName = member.UserName;

            return View(cp);
        }

        //post submitted change password request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitChangePassword([Bind("Id,OldPassword,NewPassword,ConfirmPassword")] ResetPassword changePassword)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData[StaticString.StatusMessage] = "Error: Model state not valid.";
                    return RedirectToAction(nameof(ChangePassword), new { id = changePassword.Id });
                }

                var member = _security.GetMemberByApplicationId(changePassword.Id);

                if (member == null)
                {
                    TempData[StaticString.StatusMessage] = "Error: Can not found the member.";
                    return RedirectToAction(nameof(ChangePassword), new { id = changePassword.Id });
                }

                if (_identityDefaultOptions.IsDemo && _superAdminDefaultOptions.Email.Equals(member.Email))
                {
                    TempData[StaticString.StatusMessage] = "Error: Demo mode can not change super@admin.com data.";
                    return RedirectToAction(nameof(ChangePassword), new { id = changePassword.Id });
                }
                var tokenResetPassword = await _userManager.GeneratePasswordResetTokenAsync(member);
                var changePasswordResult = await _userManager.ResetPasswordAsync(member, tokenResetPassword, changePassword.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    TempData[StaticString.StatusMessage] = "Error: ";
                    foreach (var error in changePasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                        TempData[StaticString.StatusMessage] = TempData[StaticString.StatusMessage] + " " + error.Description;
                    }
                    return RedirectToAction(nameof(ChangePassword), new { id = changePassword.Id });
                }

                TempData[StaticString.StatusMessage] = "Reset password success";
                return RedirectToAction(nameof(ChangePassword), new { id = changePassword.Id });
            }
            catch (Exception ex)
            {
                TempData[StaticString.StatusMessage] = "Error: " + ex.Message;
                return RedirectToAction(nameof(ChangePassword), new { id = changePassword.Id });
            }

        }

        //display change role screen if user founded, otherwise 404
        [HttpGet]
        public async Task<IActionResult> ChangeRole(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = _security.GetMemberByApplicationId(id);
            if (member == null)
            {
                return NotFound();
            }
            
            var registeredRoles = await _userManager.GetRolesAsync(member);            

            ChangeRoles changeRole = new ChangeRoles();
            changeRole.Id = id;
            changeRole.UserName = member.UserName;
            changeRole.HR = registeredRoles.Contains("HR") ? true : false;
            changeRole.SuperAdmin = registeredRoles.Contains("SuperAdmin") ? true : false;
            changeRole.Admin = registeredRoles.Contains("Admin") ? true : false;
            changeRole.Employee = registeredRoles.Contains("Employee") ? true : false;

            return View(changeRole);
        }

        //post submitted change role request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitChangeRole([Bind("Id", "HR", "Admin", "SuperAdmin", "Employee")]ChangeRoles changeRoles)
        {
            try
            {                

                if (!ModelState.IsValid)
                {
                    TempData[StaticString.StatusMessage] = "Error: Model state not valid.";
                    return RedirectToAction(nameof(ChangeRole), new { id = changeRoles.Id });
                }

                var member = _security.GetMemberByApplicationId(changeRoles.Id);
                if (member == null)
                {
                    TempData[StaticString.StatusMessage] = "Error: Can not found the member.";
                    return RedirectToAction(nameof(ChangeRole), new { id = changeRoles.Id });
                }

                if (_identityDefaultOptions.IsDemo && _superAdminDefaultOptions.Email.Equals(member.Email))
                {
                    TempData[StaticString.StatusMessage] = "Error: Demo mode can not change super@admin.com data.";
                    return RedirectToAction(nameof(ChangeRole), new { id = changeRoles.Id });
                }

                //todo role
                //if (changeRoles.IsTodoRegistered)
                //{
                //    await _userManager.AddToRoleAsync(member, "Todo");
                //}
                //else
                //{
                //    await _userManager.RemoveFromRoleAsync(member, "Todo");
                //}

                ////membership role
                //if (changeRoles.IsMembershipRegistered)
                //{
                //    await _userManager.AddToRoleAsync(member, "Membership");
                //}
                //else
                //{
                //    await _userManager.RemoveFromRoleAsync(member, "Membership");
                //}

                ////role role
                //if (changeRoles.IsRoleRegistered)
                //{
                //    await _userManager.AddToRoleAsync(member, "Role");
                //}
                //else
                //{
                //    await _userManager.RemoveFromRoleAsync(member, "Role");
                //}
                //if (changeRoles.IsCandidateMasterRegistered)
                //{
                //    await _userManager.AddToRoleAsync(member, "CandidateMaster");
                //}
                //else
                //{
                //    await _userManager.RemoveFromRoleAsync(member, "CandidateMaster");
                //}
                //if (changeRoles.IsDataMasterRegistered)
                //{
                //    await _userManager.AddToRoleAsync(member, "DataMaster");
                //}
                //else
                //{
                //    await _userManager.RemoveFromRoleAsync(member, "DataMaster");
                //}
                //if (changeRoles.IsExpenseMasterRegistered)
                //{
                //    await _userManager.AddToRoleAsync(member, "ExpenseMaster");
                //}
                //else
                //{
                //    await _userManager.RemoveFromRoleAsync(member, "ExpenseMaster");
                //}
                //if (changeRoles.IsInvoiceMasterRegistered)
                //{
                //    await _userManager.AddToRoleAsync(member, "InvoiceMaster");
                //}
                //else
                //{
                //    await _userManager.RemoveFromRoleAsync(member, "InvoiceMaster");
                //}
                //if (changeRoles.IsLeadMasterRegistered)
                //{
                //    await _userManager.AddToRoleAsync(member, "LeadMaster");
                //}
                //else
                //{
                //    await _userManager.RemoveFromRoleAsync(member, "LeadMaster");
                //}
                //if (changeRoles.IsLeavecountRegistered)
                //{
                //    await _userManager.AddToRoleAsync(member, "Leavecount");
                //}
                //else
                //{
                //    await _userManager.RemoveFromRoleAsync(member, "Leavecount");
                //}
                //if (changeRoles.IsProjectMasterRegistered)
                //{
                //    await _userManager.AddToRoleAsync(member, "ProjectMaster");
                //}
                //else
                //{
                //    await _userManager.RemoveFromRoleAsync(member, "ProjectMaster");
                //}
                if (changeRoles.HR)
                {
                    await _userManager.AddToRoleAsync(member, "HR");
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(member, "HR");
                }
                if (changeRoles.SuperAdmin)
                {
                    await _userManager.AddToRoleAsync(member, "SuperAdmin");
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(member, "SuperAdmin");
                }
                if (changeRoles.Admin)
                {
                    await _userManager.AddToRoleAsync(member, "Admin");
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(member, "Admin");
                }
                if (changeRoles.Employee)
                {
                    await _userManager.AddToRoleAsync(member, "Employee");
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(member, "Employee");
                }

                TempData[StaticString.StatusMessage] = "Update success";
                return RedirectToAction(nameof(ChangeRole), new { id = changeRoles.Id });
            }
            catch (Exception ex)
            {
                TempData[StaticString.StatusMessage] = "Error: " + ex.Message;
                return RedirectToAction(nameof(ChangeRole), new { id = changeRoles.Id });
            }

        }

        //display member registration screen
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        //post submitted registration request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitRegister([Bind("EmailConfirmed,Email,PhoneNumber,Password,ConfirmPassword")] Register register)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData[StaticString.StatusMessage] = "Error: Model state not valid.";
                    return RedirectToAction(nameof(Register));
                }

                ApplicationUser newMember = new ApplicationUser();
                newMember.Email = register.Email;
                newMember.UserName = register.Email;
                newMember.PhoneNumber = register.PhoneNumber;
                newMember.EmailConfirmed = register.EmailConfirmed;
                newMember.isSuperAdmin = false;

                var result = await _userManager.CreateAsync(newMember, register.Password);
                if (result.Succeeded)
                {
                    TempData[StaticString.StatusMessage] = "Register new member success";
                    return RedirectToAction(nameof(ChangeProfile), new { id = newMember.Id });
                }
                else
                {
                    TempData[StaticString.StatusMessage] = "Error: Register new member not success";
                    return RedirectToAction(nameof(Register));
                }
                
            }
            catch (Exception ex)
            {
                TempData[StaticString.StatusMessage] = "Error: " + ex.Message;
                return RedirectToAction(nameof(Register));
            }

        }

    }
}