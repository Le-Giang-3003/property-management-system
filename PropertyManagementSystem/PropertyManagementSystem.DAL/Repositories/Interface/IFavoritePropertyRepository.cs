using PropertyManagementSystem.DAL.Entities;

namespace PropertyManagementSystem.DAL.Repositories.Interface
{
    public interface IFavoritePropertyRepository : IGenericRepository<FavoriteProperty>
    {
        Task<IEnumerable<FavoriteProperty>> GetFavoritesByUserIdAsync(int userId);
        Task<bool> AddToFavoriteAsync(FavoriteProperty favoriteProperty);
        Task<bool> IsPropertyFavoritedAsync(int userId, int propertyId);
        Task<bool> RemoveFromFavoriteAsync(int userId, int propertyId);
        Task<bool> RemoveFromFavoriteAsync(FavoriteProperty favoriteProperty);
    }
}
