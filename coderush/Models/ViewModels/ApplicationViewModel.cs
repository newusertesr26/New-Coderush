using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coderush.Models.ViewModels
{
    public class ApplicationViewModel : IdentityUser
    {
        //public string UserName { get; set; }
        //public string PhoneNumber { get; set; }
        //public string EmailConfirmed { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? JoiningDate { get; set; }
        public IFormFile ProfilePicture { get; set; }
         public bool isSuperAdmin { get; set; } = false;
    }
}
