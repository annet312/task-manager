using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagerBLL.Models
{
    public class TaskTemplateBLL
    {
        public int Id { get; set; }
        public int? TemplateId { get; set; }
        // NULL if line is name of template
        public string Name { get; set; }
    }
}
