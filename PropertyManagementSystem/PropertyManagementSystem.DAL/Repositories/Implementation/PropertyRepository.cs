using Microsoft.EntityFrameworkCore;
using PropertyManagementSystem.DAL.Data;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;

namespace PropertyManagementSystem.DAL.Repositories.Implementation
{
    /// <summary>
    /// Repository for managing properties.
    /// </summary>
    /// <seealso cref="PropertyManagementSystem.DAL.Repositories.Implementation.GenericRepository&lt;PropertyManagementSystem.DAL.Entities.Property&gt;" />
    /// <seealso cref="PropertyManagementSystem.DAL.Repositories.Interface.IPropertyRepository" />
    public class PropertyRepository : GenericRepository<Property>, IPropertyRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public PropertyRepository(AppDbContext context) : base(context)
        {
        }
        /// <summary>
        /// Adds the property asynchronous.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public async Task<bool> AddPropertyAsync(Property property)
        {
            await _context.Properties.AddAsync(property);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        /// <summary>
        /// Deletes the property asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<bool> DeletePropertyAsync(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
            {
                return false;
            }
            property.Status = "Unavailable";
            _context.Properties.Update(property);
            var result = await _context.SaveChangesAsync();
            return result > 0;

        }

        /// <summary>
        /// Gets all properties asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Property>> GetAllPropertiesAsync()
        {
            return await _context.Properties
                .Where(p => p.Status != "Unavailable")
                .Include(p => p.Landlord)
                .Include(p => p.Images)
                .ToListAsync();
        }

        /// <summary>
        /// Gets the available properties asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Property>> GetAvailablePropertiesAsync()
        {
            return await _dbSet
                .Include(p => p.Landlord)
                .Include(p => p.Images.Where(i => i.IsThumbnail))
                .Where(p => p.Status == "Available")
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Gets the by landlord identifier asynchronous.
        /// </summary>
        /// <param name="landlordId">The landlord identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Property>> GetByLandlordIdAsync(int landlordId)
        {
            return await _dbSet
                .Include(p => p.Images.Where(i => i.IsThumbnail))
                .Where(p => p.LandlordId == landlordId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Gets the by landlord with details asynchronous.
        /// </summary>
        /// <param name="landlordId">The landlord identifier.</param>
        /// <param name="take">The take.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<Property>> GetByLandlordWithDetailsAsync(int landlordId, int take = 10)
        {
            return await _dbSet
                .Include(p => p.Landlord)
                .Include(p => p.Images.Where(i => i.IsThumbnail))
                .Where(p => p.LandlordId == landlordId && p.Status != "Unavailable")
                .OrderByDescending(p => p.CreatedAt)
                .Take(take)
                .ToListAsync();
        }

        /// <summary>
        /// Gets the count by status asynchronous.
        /// </summary>
        /// <param name="landlordId">The landlord identifier.</param>
        /// <param name="status">The status.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<int> GetCountByStatusAsync(int landlordId, string status)
        {
            return await _dbSet.CountAsync(p => p.LandlordId == landlordId && p.Status == status);
        }

        /// <summary>
        /// Gets the properties by landlord identifier asynchronous.
        /// </summary>
        /// <param name="landlordId">The landlord identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Property>> GetPropertiesByLandlordIdAsync(int landlordId)
        {
            return await _context.Properties
                .Where(p => p.LandlordId == landlordId && p.Status != "Unavailable")
                .Include(p => p.Images)
                .ToListAsync();
        }

        /// <summary>
        /// Gets the property by identifier asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<Property?> GetPropertyByIdAsync(int id)
        {
            return await _context.Properties
                .Include(p => p.Landlord)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.PropertyId == id && p.Status != "Unavailable");
        }

        /// <summary>
        /// Gets the property with details asynchronous.
        /// </summary>
        /// <param name="propertyId">The property identifier.</param>
        /// <returns></returns>
        public async Task<Property?> GetPropertyWithDetailsAsync(int propertyId)
        {
            return await _dbSet
                .Include(p => p.Landlord)
                .Include(p => p.Images.OrderBy(i => i.DisplayOrder))
                .FirstOrDefaultAsync(p => p.PropertyId == propertyId);
        }

        /// <summary>
        /// Gets the total by landlord asynchronous.
        /// </summary>
        /// <param name="landlordId">The landlord identifier.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<int> GetTotalByLandlordAsync(int landlordId)
        {
            return await _dbSet.CountAsync(p => p.LandlordId == landlordId && p.Status != "Unavailable");
        }

        /// <summary>
        /// Gets the total monthly revenue asynchronous.
        /// </summary>
        /// <param name="landlordId">The landlord identifier.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<decimal> GetTotalMonthlyRevenueAsync(int landlordId)
        {
            return await _dbSet
                .Where(p => p.LandlordId == landlordId && p.Status == "Rented")
                .SumAsync(p => p.RentAmount);
        }

        /// <summary>
        /// Searches the properties asynchronous.
        /// </summary>
        /// <param name="city">The city.</param>
        /// <param name="propertyType">Type of the property.</param>
        /// <param name="minRent">The minimum rent.</param>
        /// <param name="maxRent">The maximum rent.</param>
        /// <param name="minBedrooms">The minimum bedrooms.</param>
        /// <param name="status">The status filter.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Property>> SearchPropertiesAsync(
            string? city = null,
            string? propertyType = null,
            decimal? minRent = null,
            decimal? maxRent = null,
            int? minBedrooms = null,
            string? status = null)
        {
            var query = _context.Properties
                .Include(p => p.Landlord)
                .Include(p => p.Images)
                .Where(p => p.Status != "Unavailable"); // Exclude unavailable

            // Status filter (optional)
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(p => p.Status == status);
            }
            else
            {
                // Default: chỉ show Available nếu không có status filter
                query = query.Where(p => p.Status == "Available");
            }

            // City filter (optional)
            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(p => p.City.Contains(city) ||
                                        p.District.Contains(city) ||
                                        p.Address.Contains(city));
            }

            // Property type
            if (!string.IsNullOrWhiteSpace(propertyType))
            {
                query = query.Where(p => p.PropertyType == propertyType);
            }

            // Price range
            if (minRent.HasValue)
                query = query.Where(p => p.RentAmount >= minRent.Value);
            if (maxRent.HasValue)
                query = query.Where(p => p.RentAmount <= maxRent.Value);

            // Min bedrooms filter
            if (minBedrooms.HasValue)
            {
                query = query.Where(p => p.Bedrooms >= minBedrooms.Value);
            }

            return await query
                .OrderByDescending(p => p.CreatedAt)
                .Take(50) // Pagination
                .ToListAsync();
        }

        /// <summary>
        /// Updates the property asynchronous.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public async Task<bool> UpdatePropertyAsync(Property property)
        {
            var existingProperty = await _context.Properties.FindAsync(property.PropertyId);
            if (existingProperty == null)
            {
                return false;
            }

            _context.Entry(existingProperty).CurrentValues.SetValues(property);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
    }
}
