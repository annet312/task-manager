using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagerDAL.Entities
{
    public class _Task
    {
        public int Id { get; set; }

        public int? ParentId { get; set; }

        [ForeignKey("Author")]
        public int AuthorId { get; set; }

        public string Name { get; set; }

        [ForeignKey("Assignee")]
        public int AssigneeId { get; set; }

        [ForeignKey("Status")]
        public int StatusId { get; set; }

        public int? Progress { get; set; }

        public DateTime? DateStart { get; set; }

        public DateTime? ETA { get; set; }

        public DateTime? DueDate { get; set; }

        public string Comment { get; set; }

        public virtual Person Assignee { get; set; }
        public virtual Person Author { get; set; }
        public virtual Status Status { get; set; }
    }
}
