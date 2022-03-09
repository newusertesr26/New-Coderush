using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace coderush.Models
{
    public class Comments
    {
        public int Id { get; set; }
        [Display(Name ="Note:")]
        [Required]
        public string Note { get; set; }
        public int CandidateId { get; set; }
    }
}
