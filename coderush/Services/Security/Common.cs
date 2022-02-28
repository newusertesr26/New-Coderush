using coderush.Data;
using coderush.Models;
using coderush.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coderush.Services.Security
{
    //custom service provided for common user and membership activities such as get user , create user etc..
    public class Common : ICommon
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SuperAdminDefaultOptions _superAdminDefaultOptions;
        private readonly ApplicationDbContext _context;

        public Common(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<SuperAdminDefaultOptions> superAdminDefaultOptions,
            ApplicationDbContext context
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _superAdminDefaultOptions = superAdminDefaultOptions.Value;
            _context = context;
        }

        public async Task CreateDefaultSuperAdmin()
        {
            try
            {

                var superAdmin = await CreateApplicationUser(
                     new ApplicationViewModel
                     {
                         Email = _superAdminDefaultOptions.Email,
                         UserName = _superAdminDefaultOptions.Email,
                         EmailConfirmed = true,
                         isSuperAdmin = true
                     }
                     , _superAdminDefaultOptions.Password);

                //loop all the roles and then fill to SuperAdmin so he become powerfull
                var user = await _userManager.FindByEmailAsync(superAdmin.Email);

                List<string> roles = new List<string>();
                if (user != null)
                {
                    foreach (var item in typeof(App.Pages).GetNestedTypes())
                    {
                        var roleName = item.Name;
                        if (!await _roleManager.RoleExistsAsync(roleName))
                        {
                            await _roleManager.CreateAsync(new IdentityRole(roleName));
                            roles.Add(roleName);
                        }

                    }

                    await _userManager.AddToRolesAsync(user, roles);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<String> GetAllRoles()
        {
            try
            {
                List<String> roles = new List<string>();
                foreach (var item in typeof(App.Pages).GetNestedTypes())
                {
                    var roleName = item.Name;
                    roles.Add(roleName);

                }

                return roles;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<ApplicationUser> GetAllMembers()
        {
            try
            {
                List<ApplicationUser> users = new List<ApplicationUser>();
                var user = _context.ApplicationUser.ToList();
                return user;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ApplicationUser GetMemberByApplicationId(string applicationId)
        {
            try
            {
                var userdata = _context.ApplicationUser.Where(x => x.Id.Equals(applicationId)).FirstOrDefault();

                return userdata;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<ApplicationUser> CreateApplicationUser(ApplicationViewModel applicationUser, string password)
        {
            try
            {
                ApplicationUser appUser = new ApplicationUser();

                appUser.Email = applicationUser.Email;
                appUser.UserName = applicationUser.Email;
                appUser.EmailConfirmed = applicationUser.EmailConfirmed;
                appUser.isSuperAdmin = applicationUser.isSuperAdmin;

                await _userManager.CreateAsync(appUser, password);

                return appUser;
            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}
