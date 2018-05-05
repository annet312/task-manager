

namespace TaskManagerDAL.Entities
{
    public class TaskTemplate
    {
        public int Id { get; set; }
        public int? TemplateId { get; set; } 
        // NULL if line is name of template
        public string Name { get; set; } 
        //if TemplateId = NULL Name = Name of template, else Name = Name of subtask 
    }
}
