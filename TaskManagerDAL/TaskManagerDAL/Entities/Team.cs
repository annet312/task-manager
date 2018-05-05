using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagerDAL.Entities
{
    public class Team
    {
        [Key]
        public int Id { get; set; }
        //[Required]
        //[ForeignKey("Person")]
        public int ManagerId { get; set; }
        public string TeamName { get; set; }
    }
}
