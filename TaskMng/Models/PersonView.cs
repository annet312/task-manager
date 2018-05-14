using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskMng.Models
{
    public class PersonView
    {
        public int Id { get; set; }
       // public string UserId { get; set; } //foreign key to AspNetuser
        public string Name { get; set; }
        //public string Role { get; set; }//?????????
        public string Email { get; set; }
        public TeamView Team { get; set; }
    }
} 