using coderush.DataEnum;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace coderush.Models.ViewModels
{
    public class CandidateMastersViewModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Name:")]
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
        [Required]
        [Display(Name = "Technologies:")]

        public int Technologies { get; set; }
        public string technologies { get; set; }
        [Display(Name = "CVUpload:")]
        [Required]
        public IFormFile FileUpload { get; set; }
        public string filename { get; set; }
        [Display(Name = "Active:")]
        public bool IsActive { get; set; }
        [Display(Name = "Interview Date:")]
        [Required]
        public DateTime? InterviewDate { get; set; }
        [Display(Name = "Place:")]
        [Required]
        public string PlaceOfInterview { get; set; }
        [Display(Name = "Interview Time:")]
        [Required]
        public TimeSpan? InterviewTime { get; set; }
        [Display(Name = "Description:")]
        [Required]
        public string InterviewDescription { get; set; }
        [Display(Name = "Reject:")]
        public bool IsReject { get; set; }
        public string Color { get; set; }
        public bool IsDelete { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
       
        public DateTime? dateforNext { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        //  public Comments comment { get; set; }

        public int? Status { get; set; }
     //   public Comments comment { get; set; }

        //  public string Schedule { get; set; }
    }
}
