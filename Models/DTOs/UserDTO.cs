namespace Notes.Models.DTOs
{
    public class UserDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = Roles.STUDENT.ToString();
    }
}
