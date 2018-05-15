using System;

namespace TaskManagerBLL.Models
{
    public class TaskBLL
    {
        public int Id { get; set; }

        public int? ParentId { get; set; }

        public PersonBLL Author { get; set; }

        public string Name { get; set; }

        public PersonBLL Assignee { get; set; }

        public StatusBLL Status { get; set; }

        public int? Progress { get; set; }

        public DateTime? DateStart { get; set; }

        public DateTime? ETA { get; set; }

        public DateTime? DueDate { get; set; }

        public string Comment { get; set; }
    }
}
