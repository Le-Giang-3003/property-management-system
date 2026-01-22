using PropertyManagementSystem.DAL.Entities;

namespace PropertyManagementSystem.BLL.Services.Interface
{
    public interface IFavoritePropertyService 
    {
        Task<IEnumerable<FavoriteProperty>> GetFavoritesByUserIdAsync(int userId);
        Task<bool> AddToFavoriteAsync(FavoriteProperty favoriteProperty);
        Task<bool> IsPropertyFavoritedAsync(int userId, int propertyId);
        Task<bool> RemoveFromFavoriteAsync(int userId, int propertyId);
        Task<bool> RemoveFromFavoriteAsync(FavoriteProperty favoriteProperty);
        Task<bool> ToggleFavoriteAsync(int userId, int propertyId);
    }
}
