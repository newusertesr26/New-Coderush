using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace coderush.Models
{
    //todo class entity for simple todo app example
    public class Leave
    {
        public string LeaveID { get; set; }
        [Required]
        [Display(Name = "Title")]
        public string LeaveName{ get; set; }
        [Display(Name = "Is Done?")]
        public bool IsDone { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
