using NETAPI.Models;
using System;

namespace NETAPI.Models
{
    public class Token
    {
        public int Id { get; set; }
        public int UserId { get; set; }            
        public string RefreshToken { get; set; }   
        public string ExpiresIn { get; set; }      
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; }             
    }
}