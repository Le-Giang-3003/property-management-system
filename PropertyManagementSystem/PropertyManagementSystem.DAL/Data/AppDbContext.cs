using Microsoft.EntityFrameworkCore;
using PropertyManagementSystem.DAL.Entities;

namespace PropertyManagementSystem.DAL.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        // Property
        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyImage> PropertyImages { get; set; }
        public DbSet<FavoriteProperty> FavoriteProperties { get; set; }

        // Lease
        public DbSet<Lease> Leases { get; set; }
        public DbSet<LeaseSignature> LeaseSignatures { get; set; }

        // Application
        public DbSet<RentalApplication> RentalApplications { get; set; }

        // Payment
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentDispute> PaymentDisputes { get; set; }
        public DbSet<Refund> Refunds { get; set; }

        // Maintenance
        public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }
        public DbSet<MaintenanceImage> MaintenanceImages { get; set; }
        public DbSet<MaintenanceComment> MaintenanceComments { get; set; }

        // Viewing
        public DbSet<PropertyViewing> PropertyViewings { get; set; }

        // Chat
        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<ChatParticipant> ChatParticipants { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        // Notification
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationPreference> NotificationPreferences { get; set; }

        // AI
        public DbSet<AITenantScore> AITenantScores { get; set; }
        public DbSet<AIPricePrediction> AIPricePredictions { get; set; }
        public DbSet<AIVacancyPrediction> AIVacancyPredictions { get; set; }
        public DbSet<AIPropertyRecommendation> AIPropertyRecommendations { get; set; }
        public DbSet<AIMarketAnalysis> AIMarketAnalyses { get; set; }

        // Analytics
        public DbSet<PropertyAnalytics> PropertyAnalytics { get; set; }
        public DbSet<TenantPreference> TenantPreferences { get; set; }

        // Document
        public DbSet<Document> Documents { get; set; }

        // Admin
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<BackupHistory> BackupHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique constraints
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.RoleName)
                .IsUnique();

            modelBuilder.Entity<FavoriteProperty>()
                .HasIndex(f => new { f.UserId, f.PropertyId })
                .IsUnique();

            // Self-referencing Lease (Renew)
            modelBuilder.Entity<Lease>()
                .HasOne(l => l.PreviousLease)
                .WithMany()
                .HasForeignKey(l => l.PreviousLeaseId)
                .OnDelete(DeleteBehavior.Restrict);

            // User relationships - disable cascade delete
            modelBuilder.Entity<Property>()
                .HasOne(p => p.Landlord)
                .WithMany(u => u.Properties)
                .HasForeignKey(p => p.LandlordId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Lease>()
                .HasOne(l => l.Tenant)
                .WithMany(u => u.TenantLeases)
                .HasForeignKey(l => l.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MaintenanceRequest>()
                .HasOne(m => m.Tenant)
                .WithMany(u => u.TenantRequests)
                .HasForeignKey(m => m.RequestedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MaintenanceRequest>()
                .HasOne(m => m.Technician)
                .WithMany(u => u.TechnicianRequests)
                .HasForeignKey(m => m.AssignedTo)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChatMessage>()
                .HasOne(c => c.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(c => c.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Admin", Description = "System Administrator" },
                new Role { RoleId = 2, RoleName = "Landlord", Description = "Property Owner" },
                new Role { RoleId = 3, RoleName = "Tenant", Description = "Property Renter" },
                new Role { RoleId = 4, RoleName = "Technician", Description = "Maintenance Technician" }
            );
        }
    }
}
