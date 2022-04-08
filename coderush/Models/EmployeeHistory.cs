using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodesDotHRMS.Models
{
    public class EmployeeHistory
    {
        public  int Id { get; set; }
        public  string UserId { get; set; }
        public  float Salary { get; set; }
        public  DateTime Date { get; set; }

    }

    public class SaveEmployeeHistoryModel
    {
        public string userid { get; set; }
        public float salary { get; set; }
    }
}
