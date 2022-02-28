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
        public string ProjectId { get; set; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Amount must be numeric")]
        public string Amount { get; set; }
        [Required]
        public DateTime Duedate { get; set; }
        [Required]
        public string InvoiceNumber { get; set; }
        public string PendingAmount { get; set; }
        public bool Isdeleted { get; set; }
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
        public DateTime Duedate { get; set; }
        public string InvoiceNumber { get; set; }
        public string PendingAmount { get; set; }
        public int TotalAmount { get; set; }

    }
}