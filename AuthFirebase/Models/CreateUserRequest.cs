namespace FirebaseAuthApi.Models
{
    public class CreateUserRequest
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = "usuario"; // valor padrão
    }
}
