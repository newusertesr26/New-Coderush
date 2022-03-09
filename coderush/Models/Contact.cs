using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace coderush.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phoneno { get; set; }
        public string description { get; set; }
    }
}
