using System;

namespace TaskMng.Models
{
    public class TaskView
    {
        public int Id { get; set; }

        public int? ParentId { get; set; }

        public PersonView Author { get; set; }

        public string Name { get; set; }

        public PersonView Assignee { get; set; }

        public StatusView Status { get; set; }

        public int? Progress { get; set; }

        public DateTime? DateStart { get; set; }

        public DateTime? ETA { get; set; }

        public DateTime? DueDate { get; set; }

        public string Comment { get; set; }
    }
}