using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using coderush.DataEnum;
using Microsoft.AspNetCore.Http;

namespace coderush.Models
{
    public class CandidateMaster
    {
        [Key]
        public int Id { get; set; }
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
        public int Technologies { get; set; }
        [Display(Name = "CVUpload:")]
        [Required]
        public string FileUpload { get; set; }
        [Display(Name = "Active:")]
        public bool IsActive { get; set; }
        [Display(Name = "Interview Date:")]
        [Required]
        public DateTime? InterviewDate { get; set; }
        [Display(Name = "PlaceOfInterview:")]
        [Required]
        public string PlaceOfInterview { get; set; }
        [Display(Name = "Interview Time:")]
        [Required]

        //[DataType(DataType.Time)]
        //[DisplayFormat(DataFormatString = "{0:hh\\:mm\\:tt}", ApplyFormatInEditMode = true)]
        
        //[DisplayFormat(DataFormatString = "{ hh:mm tt}")]
        public TimeSpan? InterviewTime { get; set; }
        [Display(Name = "Description:")]
        [Required]
        public string InterviewDescription { get; set; }
        [Display(Name = "Reject:")]
        public bool IsReject { get; set; }
        public bool IsDelete { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }

    //public class CandidateMasterDetails
    //{
    //    public List<CandidateMasterViewModel> CandidateMasterlist { get; set; }
    //}
    //public class CandidateMasterViewModel
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //    public string Email { get; set; }
    //    public string Phone { get; set; }
    //    public CandidateTechnologies Technologies { get; set; }
    //}

}
