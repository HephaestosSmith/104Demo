namespace Demo.Models.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }

        public string Account { get; set; } = null!;

        public string? FullName { get; set; }

        public string? Phone { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
