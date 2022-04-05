using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace coderush.Models
{
    public class Credit
    {
        //public int Id { get; set; }
        //[Display(Name = "Amount:")]
        //[Required]
        //public int Amount { get; set; }
        //public string Managername { get; set; }

        //[Display(Name = "Date:")]
        //public DateTime? Createddate { get; set; }
        public int Id { get; set; }
        [Display(Name = "Amount:")]
        [Required]
        public int? Amount { get; set; }
        public string Managername { get; set; }

        [Display(Name = "Date:")]
        public DateTime? Creditdate { get; set; }


    }
}
