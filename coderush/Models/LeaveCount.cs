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
        [Required]
        public string Userid { get; set; }
        [Required] 
        public DateTime? Fromdate { get; set; }
        [Required]
        public DateTime? Todate { get; set; }
        [Required]
        public string Count { get; set; }
        [Required]
        [StringLength(60, MinimumLength = 3)]
        public string Description { get; set; }
        public bool Isapprove { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string Approveby { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

    }
}
