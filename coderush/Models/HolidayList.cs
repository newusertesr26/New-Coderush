using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CodesDotHRMS.Models
{
    public class HolidayList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Day { get; set; }
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }
        public bool Isdelete { get; set; }
    }
}
