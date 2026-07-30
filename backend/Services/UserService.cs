using backend.DTOs;
using backend.Model;
using backend.Repositories;

namespace backend.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserProfileDto?> GetUserProfileAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            return MapToProfileDto(user);
        }

        public async Task<UserProfileDto?> UpdateUserProfileAsync(int userId, UpdateUserProfileDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            // Check if username is taken by another user
            bool isTaken = await _userRepository.IsUserNameTakenAsync(dto.UserName, userId);
            if (isTaken)
            {
                throw new InvalidOperationException($"Username '{dto.UserName}' is already taken.");
            }

            user.UserName = dto.UserName;
            user.ImageUrl = dto.ImageUrl;

            await _userRepository.UpdateUserAsync(user);

            return MapToProfileDto(user);
        }

        private static UserProfileDto MapToProfileDto(User user)
        {
            // Determine profile completion status based on Role
            bool isProfileComplete = user.Role switch
            {
                Role.Freelancer => user.Freelancer != null && !string.IsNullOrWhiteSpace(user.Freelancer.Bio),
                Role.Client => user.Client != null && !string.IsNullOrWhiteSpace(user.Client.CompanyName),
                Role.Admin => true,
                _ => false
            };

            return new UserProfileDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                ImageUrl = user.ImageUrl,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                IsProfileComplete = isProfileComplete
            };
        }
    }
}
