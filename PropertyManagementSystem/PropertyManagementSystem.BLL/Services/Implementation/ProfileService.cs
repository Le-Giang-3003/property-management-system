using PropertyManagementSystem.BLL.DTOs.User;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystem.BLL.Services.Implementation
{
    public class ProfileService : IProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<User> _userGenericRepository;

        public ProfileService(IUnitOfWork unitOfWork, IGenericRepository<User> userGenericRepository)
        {
            _unitOfWork = unitOfWork;
            _userGenericRepository = userGenericRepository;
        }
        public async Task<UserProfileDto?> GetProfileAsync(int userId)
        {
            var user = await _userGenericRepository.GetByIdAsync(userId);
            return user == null ? null : MapToDto(user);
        }

        public async Task<IEnumerable<UserSearchResultDto>> SearchUsersAsync(UserSearchDto searchDto)
        {
            if (string.IsNullOrWhiteSpace(searchDto.Name) && string.IsNullOrWhiteSpace(searchDto.PhoneNumber))
                return Enumerable.Empty<UserSearchResultDto>();

            var users = await _unitOfWork.Users.SearchUsersAsync(searchDto.Name, searchDto.PhoneNumber);

            return users.Select(u => new UserSearchResultDto
            {
                UserId = u.UserId,
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber,
                Email = u.Email,
                IsActive = u.IsActive,
                Roles = u.UserRoles?.Select(ur => ur.Role.RoleName).ToList() ?? new()
            });
        }

        public async Task<(bool Success, string Message)> UpdateProfileAsync(int userId, UpdateProfileDto model)
        {
            var user = await _userGenericRepository.GetByIdAsync(userId);
            if (user == null)
                return (false, "Không tìm thấy người dùng");

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                if (await _unitOfWork.Users.IsPhoneExistsAsync(model.PhoneNumber, userId))
                    return (false, "Số điện thoại đã được sử dụng");
            }

            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.DateOfBirth = model.DateOfBirth;
            user.Avatar = model.Avatar;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return (true, "Cập nhật thành công");
        }
        private static UserProfileDto MapToDto(User user) => new()
        {
            UserId = user.UserId,
            Email = user.Email,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address,
            Avatar = user.Avatar,
            DateOfBirth = user.DateOfBirth,
            IsActive = user.IsActive,
            EmailVerified = user.EmailVerified,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            Roles = user.UserRoles?.Select(ur => ur.Role.RoleName).ToList() ?? new()
        };
    }
}
