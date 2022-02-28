using coderush.DataEnum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace coderush.Models
{
    
    public class DataMaster
    {
        public int Id { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        [StringLength(60, MinimumLength = 3)]
        public string Description { get; set; }
        public bool Isactive { get; set; }
        public bool Isdeleted { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class DataMasterDeatils
    {
        public List<DataMasterViewModel> DataMasterList { get; set; }
    }
    public class DataMasterViewModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
    }

}

