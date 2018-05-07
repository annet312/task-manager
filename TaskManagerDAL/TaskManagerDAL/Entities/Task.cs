using System;

namespace TaskManagerDAL.Entities
{
    public class _Task
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public int? AuthorId { get; set; }
        public virtual Person Author { get; set; }
        public string Name { get; set; }
        public int? AssigneeId { get; set; }
        public virtual Person Assignee { get; set; }
        public int StatusId { get; set; }
        public virtual Status Status { get; set; }
        public int? Progress { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? ETA { get; set; }
        public DateTime? DueDate { get; set; }
        public string Comment { get; set; }
    }
}
