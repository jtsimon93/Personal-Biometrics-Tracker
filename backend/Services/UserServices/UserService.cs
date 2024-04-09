using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PersonalBiometricsTracker.Data;
using PersonalBiometricsTracker.Dtos;
using PersonalBiometricsTracker.Entities;

namespace PersonalBiometricsTracker.Services
{
    public class UserService : IUserService
    {
        private readonly PersonalBiometricsTrackerDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;

        public UserService(PersonalBiometricsTrackerDbContext context, IPasswordHasher<User> passwordHasher, IConfiguration configuration)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        public async Task<User> RegisterUserAsync(UserRegistrationDto userDto)
        {
            // Check if the user already exists
            var existingUserByEmail = await _context.Users.AnyAsync(u => u.Email == userDto.Email);
            var existingUserByUserName = await _context.Users.AnyAsync(u => u.Username == userDto.Username);

            if (existingUserByEmail)
            {
                throw new Exception("Email already in use.");
            }

            if (existingUserByUserName)
            {
                throw new Exception("Username already taken.");
            }

            var user = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
            };

            // Hash the password
            user.Password = _passwordHasher.HashPassword(user, userDto.Password);

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<string> AuthenticateAsync(UserLoginDto userDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userDto.Username);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var PasswordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, userDto.Password);

            if (PasswordVerificationResult != PasswordVerificationResult.Success)
            {
                throw new Exception("Invalid credentials.");
            }

            // Generate JWT token
            var token = GenerateJwtToken(user);

            return token;
        }

        public async Task<User> UpdateUserAsync(int userId, UserUpdateDto userUpdateDto)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Update username if provided and it's different from the current one
            if (!string.IsNullOrEmpty(userUpdateDto.Username) && userUpdateDto.Username != user.Username)
            {
                var usernameExists = await _context.Users.AnyAsync(u => u.Username == userUpdateDto.Username);
                if (usernameExists)
                {
                    throw new Exception("Username already in use.");
                }
                user.Username = userUpdateDto.Username;
            }

            // Update email if provided and it's different from the current one
            if (!string.IsNullOrEmpty(userUpdateDto.Email) && userUpdateDto.Email != user.Email)
            {
                var emailExists = await _context.Users.AnyAsync(u => u.Email == userUpdateDto.Email);
                if (emailExists)
                {
                    throw new Exception("Email already in use.");
                }
                user.Email = userUpdateDto.Email;
            }

            // Update password if provided
            if (!string.IsNullOrEmpty(userUpdateDto.Password))
            {
                user.Password = _passwordHasher.HashPassword(user, userUpdateDto.Password);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("An error occurred while updating the user.");
            }

            return user;
        }


        private string GenerateJwtToken(User user)
        {
            var jwtSecret = _configuration.GetValue<string>("JWTSecret");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}