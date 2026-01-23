using PropertyManagementSystem.DAL.Data;
using PropertyManagementSystem.DAL.Repositories.Interface;

namespace PropertyManagementSystem.DAL.Repositories.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IUserRepository? _users;
        private IRoleRepository? _roles;
        private IUserRoleRepository? _userRoles;
        private IPropertyRepository? _properties;
        private IPropertyViewingRepository? _propertyViewings;
        private IDocumentRepository? _documents;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IUserRepository Users => _users ??= new UserRepository(_context);
        public IRoleRepository Roles => _roles ??= new RoleRepository(_context);
        public IUserRoleRepository UserRoles => _userRoles ??= new UserRoleRepository(_context);
        public IPropertyRepository Properties => _properties ??= new PropertyRepository(_context);
        public IPropertyViewingRepository PropertyViewings => _propertyViewings ??= new PropertyViewingRepository(_context);

        public IDocumentRepository Documents => _documents ??= new DocumentRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
