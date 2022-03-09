using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coderush.Models
{
    public class Hire
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string Email { get; set; }
        public string country { get; set; }
        public string phoneNo { get; set; }
        public string Developers { get; set; }
        public TimeSpan? duration { get; set; }
        public string description { get; set; }
        public string filename { get; set; }
        public Byte[]  filebinary { get; set; }
        public string filetype { get; set; }
        public DateTime? Createddate { get; set; }


    }
}
