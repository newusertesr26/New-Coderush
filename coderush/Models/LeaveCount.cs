using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace coderush.Models
{
    public class LeaveCount
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Userid:")]

        [Required]
        public string Userid { get; set; }
        [Display(Name = "Fromdate:")]

        [Required] 
        public DateTime? Fromdate { get; set; }
        [Display(Name = "Todate:")]

        [Required]
        public DateTime? Todate { get; set; }
        [Display(Name = "Count:")]

        [Required]
        public string Count { get; set; }
        [Display(Name = "Description:")]

        [Required]
        [StringLength(60, MinimumLength = 3)]
        public string Description { get; set; }
        [Display(Name = "Isapprove:")]

        public bool Isapprove { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string Approveby { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

    }
}
