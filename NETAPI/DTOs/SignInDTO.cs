namespace NETAPI.DTOs
{
    public class SignInDTO
    {
        public string Email { get; set; }       
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
