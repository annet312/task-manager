namespace TaskManagerDAL.Entities
{
    /// <summary>
    /// Templates of sets of subtasks
    /// </summary>
    public class TaskTemplate
    {
        public int Id { get; set; }

        // NULL if line is name of template
        public int? TemplateId { get; set; }

        //if TemplateId = NULL Name = Name of template, else Name = Name of subtask 
        public string Name { get; set; } 
        
    }
}
