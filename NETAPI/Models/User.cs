using Microsoft.AspNetCore.Identity;

namespace NETAPI.Models
{
    public enum UserType
    {
        User,
        Admin,
    }

    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Hash { get; set; }
        public UserType Type { get; set; } = UserType.User;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<Token> Tokens { get; set; }
    }

}