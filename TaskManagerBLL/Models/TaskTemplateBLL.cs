
namespace TaskManagerBLL.Models
{
    public class TaskTemplateBLL
    {
        public int Id { get; set; }
        
        // NULL if line is name of template
        public int? TemplateId { get; set; }
       
        public string Name { get; set; }
    }
}
