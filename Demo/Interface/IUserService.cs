using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Demo.Models.Dtos;

namespace Demo.Interface
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<UserDto?> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(int id, UpdateUserDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<TokenResponseDto?> AuthenticateAsync(LoginRequestDto dto, CancellationToken cancellationToken = default);
    }
}