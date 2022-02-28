﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace coderush.Models
{
    public class ProjectMaster
    {

   
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "* A valid Project Name is required.")]
        public string ProjectName { get; set; }
        [Required]
        public string Technologies { get; set; }
        [Required]
        [StringLength(60, MinimumLength = 3)]
        public string Description { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "* A valid Manager Name is required.")]
        public string ManagerName { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "* A valid Developer Name is required.")]
        public string DeveloperName { get; set; }
        public bool isactive { get; set; }
        public bool isdeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [Required]
        public string paymenttype { get; set; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "project amount must be numeric")]
        public string projectamount { get; set; }
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        public string currency { get; set; }

    }


    public class ProjectsMasterDetails
    {
        public List<ProjectMasterDetailsViewModel> ProjectMasterlist { get; set; }
    }
    public class ProjectMasterDetailsViewModel
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string Technologies { get; set; }
        public string Amount { get; set; }
        public string Description { get; set; }
        public string ManagerName { get; set; }
        public string DeveloperName { get; set; }
        public string paymenttype { get; set; }
        public string projectamount { get; set; }
        public decimal currency { get; set; }
    }

}
