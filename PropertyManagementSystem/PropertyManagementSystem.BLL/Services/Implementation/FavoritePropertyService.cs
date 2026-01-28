using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;

namespace PropertyManagementSystem.BLL.Services.Implementation
{
    public class FavoritePropertyService : IFavoritePropertyService
    {
        private readonly IFavoritePropertyRepository _favoritePropertyRepository;
        public FavoritePropertyService(IFavoritePropertyRepository favoritePropertyRepository)
        {
            _favoritePropertyRepository = favoritePropertyRepository;
        }
        public async Task<bool> AddToFavoriteAsync(FavoriteProperty favoriteProperty)
        {
            return await _favoritePropertyRepository.AddToFavoriteAsync(favoriteProperty);
        }

        public async Task<IEnumerable<FavoriteProperty>> GetFavoritesByUserIdAsync(int userId)
        {
            return await _favoritePropertyRepository.GetFavoritesByUserIdAsync(userId);
        }

        public async Task<bool> IsPropertyFavoritedAsync(int userId, int propertyId)
        {
            return await _favoritePropertyRepository.IsPropertyFavoritedAsync(userId, propertyId);
        }

        public async Task<bool> RemoveFromFavoriteAsync(int userId, int propertyId)
        {
            return await _favoritePropertyRepository.RemoveFromFavoriteAsync(userId, propertyId);
        }

        public async Task<bool> RemoveFromFavoriteAsync(FavoriteProperty favoriteProperty)
        {
            return await _favoritePropertyRepository.RemoveFromFavoriteAsync(favoriteProperty);
        }

        public async Task<bool> ToggleFavoriteAsync(int userId, int propertyId)
        {
            var isFavorited = await _favoritePropertyRepository.IsPropertyFavoritedAsync(userId, propertyId);

            if (isFavorited)
            {
                // Đã favorite -> Xóa
                return await _favoritePropertyRepository.RemoveFromFavoriteAsync(userId, propertyId);
            }
            else
            {
                // Chưa favorite -> Thêm
                var favoriteProperty = new FavoriteProperty
                {
                    UserId = userId,
                    PropertyId = propertyId,
                    CreatedAt = DateTime.UtcNow
                };
                return await _favoritePropertyRepository.AddToFavoriteAsync(favoriteProperty);
            }
        }
    }
}
