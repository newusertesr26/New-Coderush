using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace coderush.Models
{
    public partial class ApplicationUser : IdentityUser
    {
        //override identity user, add new column
      //  public int Id { get; set; }
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Display(Name = "JoiningDate:")]
        public DateTime? JoiningDate { get; set; }
        [Display(Name = "ProfilePicture:")]
        public string ProfilePicture { get; set; }

        public bool isSuperAdmin { get; set; } = false;

        public int PaidLeave { get; set; }
        public int UnpaidLeave { get; set; }
        public string Pl { get; set; }
       // public int TotalLeave { get; set; }


    }
}
