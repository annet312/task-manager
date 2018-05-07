
using System.Collections.Generic;

namespace TaskManagerDAL.Entities
{
    public class Status
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual IEnumerable<_Task> Tasks { get; set; }
    }
}
