
namespace TaskManagerBLL.Models
{
    public class PersonBLL
    {
        public int Id { get; set; }
        public string UserId { get; set; } 
        public string Name { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public TeamBLL Team { get; set; }
    }
}
