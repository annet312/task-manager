using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskManagerDAL.Entities
{
    public class Team
    {
        [Key]
        public int Id { get; set; }

        public string TeamName { get; set; }

        public virtual IEnumerable<Person> TeamMates { get; set; }
    }
}
