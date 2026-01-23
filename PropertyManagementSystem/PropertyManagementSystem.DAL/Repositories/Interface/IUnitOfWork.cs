namespace PropertyManagementSystem.DAL.Repositories.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        IUserRoleRepository UserRoles { get; }
        IPropertyRepository Properties { get; }
        IPropertyViewingRepository PropertyViewings { get; }
        IRentalApplicationRepository RentalApplications { get; }
        ILeaseRepository Leases { get; }
        ILeaseSignatureRepository LeaseSignatures { get; }
        Task<int> SaveChangesAsync();
        int SaveChanges();
    }
}
