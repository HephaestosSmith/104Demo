using System.ComponentModel.DataAnnotations;

namespace Demo.Models.Dtos
{
    public class UpdateUserDto
    {
        [StringLength(100)]
        public string? Account { get; init; }

        [StringLength(200, MinimumLength = 6)]
        public string? Password { get; init; }

        [StringLength(200)]
        public string? FullName { get; init; }

        [StringLength(50)]
        public string? Phone { get; init; }
    }
}
