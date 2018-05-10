using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskMng.Models
{
    public class TaskEditView:CreateTaskView
    {
        public int Id { get; set; }
        public DateTime? ETA { get; set; }
        public DateTime? DueDate { get; set; }
    }
}