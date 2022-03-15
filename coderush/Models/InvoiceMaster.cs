using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace coderush.Models
{
     public class InvoiceMaster
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "ProjectId:")]

        public int ProjectId { get; set; }
        [Display(Name = "Amount:")]

        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Amount must be numeric")]
        public string Amount { get; set; }
        [Display(Name = "Duedate:")]

        [Required]
        public DateTime? Duedate { get; set; }
        [Display(Name = "InvoiceNumber:")]

        [Required]
        public string InvoiceNumber { get; set; }
        [Display(Name = "PendingAmount:")]

        public string PendingAmount { get; set; }
        public bool Isdeleted { get; set; }
        [Display(Name = "Isverify:")]
        public bool Isverify { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
    public class InvoiceMasterDeatils
    {
        public List<InvoiceMasterViewModel> InvoiceMasterList { get; set; }
    }
    public class InvoiceMasterViewModel
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Amount { get; set; }
        public DateTime? Duedate { get; set; }
        public string InvoiceNumber { get; set; }
        public string PendingAmount { get; set; }
        public int TotalAmount { get; set; }

    }
}