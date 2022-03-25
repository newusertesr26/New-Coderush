using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace coderush.Models.ViewModels
{
    public class TodoViewModel
    {
        public string TodoId { get; set; }
        [Required]
        [Display(Name = "Todo Item")]
        public string TodoItem { get; set; }
        public string Users { get; set; }
        [Required]
        [Display(Name = "Duedtae:")]
        public DateTime? Duedate { get; set; }
        [Display(Name = "FileUpload:")]
        public IFormFile FileUpload { get; set; }
        [Display(Name = "Is Done?")]
        public bool IsDone { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string FileName{ get; set; }

    }
}
