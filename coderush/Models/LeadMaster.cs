using coderush.DataEnum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace coderush.Models
{
    public class LeadMaster
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Name:")]
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "* A valid Name is required.")]
        public string Name { get; set; }
        [Display(Name = "Email:")]
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Display(Name = "Phone:")]
        [Required]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string Phone { get; set; }
        [Display(Name = "Technologies:")]
        [Required]
        public LeadTechnologies Technologies { get; set; }
        [Display(Name = "FileUpload:")]
        [Required]
        public string FileUpload { get; set; }
        [Display(Name = "IsActive:")]
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }

}