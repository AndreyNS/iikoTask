using System.ComponentModel.DataAnnotations;

namespace iikoTask.Models
{
    public class User
    {
        [Key]
        public long ClientId { get; set; }
        public string Username { get; set; } = string.Empty;
        public Guid SystemId { get; set; }
    }
}
