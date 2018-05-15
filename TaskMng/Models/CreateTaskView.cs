
namespace TaskMng.Models
{
    public class CreateTaskView
    {
        public int? ParentId { get; set; }

        public int? TemplateId { get; set; }

        public string Name { get; set; }

        public string Assignee { get; set; }

        public string Comment { get; set; }
    }
}
