using Demo.Data.Context;
using Demo.Data.Entities;
using Demo.Helpers;
using Demo.Interface;
using Demo.Models.Dtos;
using Demo.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Demo.Services
{
    public class UserService : IUserService
    {
        private readonly DemoDbContext _db;
        private readonly ILogger<UserService> _logger;
        private readonly JwtSetting _jwtSetting;

        public UserService(DemoDbContext db, ILogger<UserService> logger, IOptionsSnapshot<JwtSetting> jwtSettingAccessor)
        {
            _db = db;
            _logger = logger;
            _jwtSetting = jwtSettingAccessor.Value;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.Users
                .AsNoTracking()
                .OrderBy(u => u.Id)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Account = u.Account,
                    FullName = u.FullName,
                    Phone = u.Phone,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var u = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (u == null) return null;

            return new UserDto
            {
                Id = u.Id,
                Account = u.Account,
                FullName = u.FullName,
                Phone = u.Phone,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            };
        }

        public async Task<UserDto?> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            var exists = await _db.Users.AnyAsync(u => u.Account == dto.Account, cancellationToken);
            if (exists) return null;

            var user = new User
            {
                Account = dto.Account,
                FullName = dto.FullName,
                Phone = dto.Phone,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            user.SetPassword(dto.Password);

            _db.Users.Add(user);
            await _db.SaveChangesAsync(cancellationToken);

            return new UserDto
            {
                Id = user.Id,
                Account = user.Account,
                FullName = user.FullName,
                Phone = user.Phone,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateUserDto dto, CancellationToken cancellationToken = default)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
            if (user == null) return false;

            if (!string.IsNullOrWhiteSpace(dto.Account) &&
                !string.Equals(user.Account, dto.Account, StringComparison.OrdinalIgnoreCase))
            {
                var conflict = await _db.Users.AnyAsync(u => u.Account == dto.Account && u.Id != id, cancellationToken);
                if (conflict) return false;
                user.Account = dto.Account;
            }

            if (dto.FullName != null) user.FullName = dto.FullName;
            if (dto.Phone != null) user.Phone = dto.Phone;
            if (!string.IsNullOrEmpty(dto.Password)) user.SetPassword(dto.Password);

            user.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Concurrency issue when updating user {UserId}", id);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
            if (user == null) return false;

            _db.Users.Remove(user);
            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<TokenResponseDto?> AuthenticateAsync(LoginRequestDto dto, CancellationToken cancellationToken)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            // Retrieve user by account (async)
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Account == dto.Account, cancellationToken);
            if (user == null) return null;

            // Verify password using entity's password verification (SetPassword used on create/update)
            // Replace `VerifyPassword` with the actual verification method on your User entity if different.
            var passwordValid = false;
            try
            {
                passwordValid = user.VerifyPassword(dto.Password);
            }
            catch (MissingMethodException)
            {
                // Fallback: if VerifyPassword is not implemented, compare plain text (legacy)
                passwordValid = string.Equals(user.PasswordHash, dto.Password);
            }

            if (!passwordValid) return null;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Account),
            };

            var helper = new JwtBearerHelper();
            var access = helper.GenerateAccessToken(_jwtSetting, claims);
            var refresh = helper.GenerateRefreshToken();

            var token = new TokenResponseDto
            {
                AccessToken = access,
                RefreshToken = refresh,
                ExpiresIn = 60 * _jwtSetting.Expires
            };

            return token;
        }
    }
}