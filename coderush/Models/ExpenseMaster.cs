using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using coderush.DataEnum;

namespace coderush.Models
{
    public class ExpenseMaster
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "* A valid Name is required.")]
        [Display(Name = "Name")]
        public string ExpName { get; set; }
        [Required]
        [Display(Name = "Type")]
        public Expensestype Exptype { get; set; }
        [Display(Name = "Amount:")]
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Amount must be numeric")]
        public string Amount { get; set; }
        [Display(Name = "Description:")]
        [Required]
        [StringLength(60, MinimumLength = 3)]
        public string Description { get; set; }
        [Display(Name = "CVUpload:")]
        [Required]
        public string FileUpload { get; set; }
        [Display(Name = "isactive:")]
        public bool isactive { get; set; }
        public bool Isdelete { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }

    public class ExpensesMasterDetails
    {
        public List<ExpensesMasterDetailsViewModel> ExpensesMasterlist { get; set; }
    }
    public class ExpensesMasterDetailsViewModel
    {
        public int Id { get; set; }
        public string ExpName { get; set; }
        public string Exptype { get; set; }
        public string Amount { get; set; }
        public string Description { get; set; }
    }

}
