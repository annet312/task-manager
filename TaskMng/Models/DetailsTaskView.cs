using System.Collections.Generic;

namespace TaskMng.Models
{
    public class DetailsTaskView
    {
        public TaskView MainTask { get; set; }

        public IEnumerable<TaskView> Subtasks { get; set; }
    }
}