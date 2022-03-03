using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace coderush.Models
{
    public class RoleDetails
    {
        [Key]
        public int PageId { get; set; }
        [Required]
        [Display(Name = "Page Name:")]
        public string Pagename { get; set; }
        [Display(Name = "Role Name:")]
        public string Rolename { get; set; }
        [Display(Name = "Is Active:")]
        public bool Isactive { get; set; }
        public bool Isdelete { get; set; }
    }
}
