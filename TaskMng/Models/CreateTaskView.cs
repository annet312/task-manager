using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskMng.Models
{
    public class CreateTaskView
    {
        public string Name { get; set; }
        public string AssigneeId { get; set; }
        public string Comment { get; set; }
    }
}