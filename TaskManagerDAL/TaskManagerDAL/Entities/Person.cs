
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagerDAL.Entities
{
    public class Person
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; } //foreign key to AspNetuser
        public string Name { get; set; }
        public string Role { get; set; }//?????????
        public string EAdress { get; set; }
        public Team Team { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
