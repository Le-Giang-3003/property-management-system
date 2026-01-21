using Microsoft.EntityFrameworkCore;
using PropertyManagementSystem.DAL.Data;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;

namespace PropertyManagementSystem.DAL.Repositories.Implementation
{
    /// <summary>
    /// Repository for managing properties.
    /// </summary>
    /// <seealso cref="PropertyManagementSystem.DAL.Repositories.Interface.IPropertyRepository" />
    public class PropertyRepository : IPropertyRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly AppDbContext _context;
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public PropertyRepository(AppDbContext context)
        {
            _context = context;
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
            property.Status = "Deleted";
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
                .Where(p => p.Status != "Deleted")
                .Include(p => p.Landlord)
                .Include(p => p.Images)
                .ToListAsync();
        }

        /// <summary>
        /// Gets the properties by landlord identifier asynchronous.
        /// </summary>
        /// <param name="landlordId">The landlord identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Property>> GetPropertiesByLandlordIdAsync(int landlordId)
        {
            return await _context.Properties
                .Where(p => p.LandlordId == landlordId && p.Status != "Deleted")
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
                .FirstOrDefaultAsync(p => p.PropertyId == id && p.Status != "Deleted");
        }

        /// <summary>
        /// Searches the properties asynchronous.
        /// </summary>
        /// <param name="city">The city.</param>
        /// <param name="propertyType">Type of the property.</param>
        /// <param name="minRent">The minimum rent.</param>
        /// <param name="maxRent">The maximum rent.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Property>> SearchPropertiesAsync(
            string? city = null,
            string? propertyType = null,
            decimal? minRent = null,
            decimal? maxRent = null)
        {
            var query = _context.Properties
                .Include(p => p.Landlord)
                .Where(p => p.Status == "Available"); // Chỉ show Available

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
