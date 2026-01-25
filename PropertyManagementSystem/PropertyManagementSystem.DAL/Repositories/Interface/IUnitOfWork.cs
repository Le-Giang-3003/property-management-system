namespace PropertyManagementSystem.DAL.Repositories.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        IUserRoleRepository UserRoles { get; }
        IPropertyRepository Properties { get; }
        IPropertyViewingRepository PropertyViewings { get; }
        IDocumentRepository Documents { get; }
        IRentalApplicationRepository RentalApplications { get; }
        ILeaseRepository Leases { get; }
        IMaintenanceRepository MaintenanceRequests { get; }
        IPropertyImageRepository PropertyImages { get; }
        Task<int> SaveChangesAsync();
        int SaveChanges();
    }
}
