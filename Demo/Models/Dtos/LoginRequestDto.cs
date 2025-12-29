using System.ComponentModel.DataAnnotations;

namespace Demo.Models.Dtos
{
    public class LoginRequestDto
    {
        [Required]
        public string Account { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
