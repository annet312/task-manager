using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskMng.Models
{
    public class DetailsTaskView
    {
        public TaskView MainTask { get; set; }
        public IEnumerable<TaskView> Subtasks { get; set; }
    }
}