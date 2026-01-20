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

            // ==================== USER & AUTHENTICATION ====================

            // User - self-referencing for UserRole.AssignedBy
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.AssignedByUser)
                .WithMany()
                .HasForeignKey(ur => ur.AssignedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // PasswordResetToken
            modelBuilder.Entity<PasswordResetToken>()
                .HasOne(prt => prt.User)
                .WithMany()
                .HasForeignKey(prt => prt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==================== PROPERTY ====================

            modelBuilder.Entity<Property>()
                .HasOne(p => p.Landlord)
                .WithMany(u => u.Properties)
                .HasForeignKey(p => p.LandlordId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PropertyImage>()
                .HasOne(pi => pi.Property)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FavoriteProperty>()
                .HasOne(fp => fp.User)
                .WithMany(u => u.FavoriteProperties)
                .HasForeignKey(fp => fp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FavoriteProperty>()
                .HasOne(fp => fp.Property)
                .WithMany(p => p.FavoritedBy)
                .HasForeignKey(fp => fp.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==================== LEASE/CONTRACT ====================

            modelBuilder.Entity<Lease>()
                .HasOne(l => l.Property)
                .WithMany(p => p.Leases)
                .HasForeignKey(l => l.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Lease>()
                .HasOne(l => l.Tenant)
                .WithMany(u => u.TenantLeases)
                .HasForeignKey(l => l.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Lease>()
                .HasOne(l => l.RentalApplication)
                .WithOne(ra => ra.Lease)
                .HasForeignKey<Lease>(l => l.ApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Self-referencing for PreviousLease
            modelBuilder.Entity<Lease>()
                .HasOne(l => l.PreviousLease)
                .WithMany()
                .HasForeignKey(l => l.PreviousLeaseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LeaseSignature>()
                .HasOne(ls => ls.Lease)
                .WithMany(l => l.Signatures)
                .HasForeignKey(ls => ls.LeaseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LeaseSignature>()
                .HasOne(ls => ls.User)
                .WithMany()
                .HasForeignKey(ls => ls.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==================== RENTAL APPLICATION ====================

            modelBuilder.Entity<RentalApplication>()
                .HasOne(ra => ra.Property)
                .WithMany(p => p.RentalApplications)
                .HasForeignKey(ra => ra.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RentalApplication>()
                .HasOne(ra => ra.Applicant)
                .WithMany(u => u.RentalApplications)
                .HasForeignKey(ra => ra.ApplicantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RentalApplication>()
                .HasOne(ra => ra.ReviewedByUser)
                .WithMany()
                .HasForeignKey(ra => ra.ReviewedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // ==================== INVOICE & PAYMENT ====================

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Lease)
                .WithMany(l => l.Invoices)
                .HasForeignKey(i => i.LeaseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Invoice)
                .WithMany(i => i.Payments)
                .HasForeignKey(p => p.InvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.ProcessedByUser)
                .WithMany()
                .HasForeignKey(p => p.ProcessedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PaymentDispute>()
                .HasOne(pd => pd.Invoice)
                .WithMany(i => i.Disputes)
                .HasForeignKey(pd => pd.InvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PaymentDispute>()
                .HasOne(pd => pd.RaisedByUser)
                .WithMany()
                .HasForeignKey(pd => pd.RaisedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PaymentDispute>()
                .HasOne(pd => pd.ResolvedByUser)
                .WithMany()
                .HasForeignKey(pd => pd.ResolvedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Refund>()
                .HasOne(r => r.Payment)
                .WithOne(p => p.Refund)
                .HasForeignKey<Refund>(r => r.PaymentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Refund>()
                .HasOne(r => r.ProcessedByUser)
                .WithMany()
                .HasForeignKey(r => r.ProcessedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // ==================== MAINTENANCE ====================

            modelBuilder.Entity<MaintenanceRequest>()
                .HasOne(mr => mr.Property)
                .WithMany(p => p.MaintenanceRequests)
                .HasForeignKey(mr => mr.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MaintenanceRequest>()
                .HasOne(mr => mr.Tenant)
                .WithMany(u => u.TenantRequests)
                .HasForeignKey(mr => mr.RequestedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MaintenanceRequest>()
                .HasOne(mr => mr.Technician)
                .WithMany(u => u.TechnicianRequests)
                .HasForeignKey(mr => mr.AssignedTo)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MaintenanceRequest>()
                .HasOne(mr => mr.ClosedByUser)
                .WithMany()
                .HasForeignKey(mr => mr.ClosedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MaintenanceImage>()
                .HasOne(mi => mi.MaintenanceRequest)
                .WithMany(mr => mr.Images)
                .HasForeignKey(mi => mi.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MaintenanceImage>()
                .HasOne(mi => mi.UploadedByUser)
                .WithMany()
                .HasForeignKey(mi => mi.UploadedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MaintenanceComment>()
                .HasOne(mc => mc.MaintenanceRequest)
                .WithMany(mr => mr.Comments)
                .HasForeignKey(mc => mc.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MaintenanceComment>()
                .HasOne(mc => mc.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(mc => mc.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==================== PROPERTY VIEWING ====================

            modelBuilder.Entity<PropertyViewing>()
                .HasOne(pv => pv.Property)
                .WithMany(p => p.Viewings)
                .HasForeignKey(pv => pv.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PropertyViewing>()
                .HasOne(pv => pv.Tenant)
                .WithMany(u => u.ViewingRequests)
                .HasForeignKey(pv => pv.RequestedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // ==================== COMMUNICATION / CHAT ====================

            modelBuilder.Entity<ChatRoom>()
                .HasOne(cr => cr.Property)
                .WithMany()
                .HasForeignKey(cr => cr.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChatRoom>()
                .HasOne(cr => cr.Lease)
                .WithMany(l => l.ChatRooms)
                .HasForeignKey(cr => cr.LeaseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChatParticipant>()
                .HasOne(cp => cp.ChatRoom)
                .WithMany(cr => cr.Participants)
                .HasForeignKey(cp => cp.ChatRoomId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ChatParticipant>()
                .HasOne(cp => cp.User)
                .WithMany(u => u.ChatParticipants)
                .HasForeignKey(cp => cp.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm => cm.ChatRoom)
                .WithMany(cr => cr.Messages)
                .HasForeignKey(cm => cm.ChatRoomId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm => cm.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(cm => cm.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==================== NOTIFICATION ====================

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NotificationPreference>()
                .HasOne(np => np.User)
                .WithOne(u => u.NotificationPreference)
                .HasForeignKey<NotificationPreference>(np => np.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==================== AI SERVICE ====================

            modelBuilder.Entity<AITenantScore>()
                .HasOne(ats => ats.RentalApplication)
                .WithOne(ra => ra.TenantScore)
                .HasForeignKey<AITenantScore>(ats => ats.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AIPricePrediction>()
                .HasOne(app => app.Property)
                .WithMany(p => p.PricePredictions)
                .HasForeignKey(app => app.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AIPropertyRecommendation>()
                .HasOne(apr => apr.User)
                .WithMany()
                .HasForeignKey(apr => apr.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AIPropertyRecommendation>()
                .HasOne(apr => apr.Property)
                .WithMany()
                .HasForeignKey(apr => apr.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==================== ANALYTICS ====================

            modelBuilder.Entity<PropertyAnalytics>()
                .HasOne(pa => pa.Property)
                .WithOne(p => p.Analytics)
                .HasForeignKey<PropertyAnalytics>(pa => pa.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==================== TENANT PREFERENCES ====================

            modelBuilder.Entity<TenantPreference>()
                .HasOne(tp => tp.User)
                .WithOne(u => u.TenantPreference)
                .HasForeignKey<TenantPreference>(tp => tp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==================== DOCUMENT / FILE ====================

            modelBuilder.Entity<Document>()
                .HasOne(d => d.UploadedByUser)
                .WithMany()
                .HasForeignKey(d => d.UploadedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.DeletedByUser)
                .WithMany()
                .HasForeignKey(d => d.DeletedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // ==================== ADMIN / SYSTEM ====================

            modelBuilder.Entity<AuditLog>()
                .HasOne(al => al.User)
                .WithMany()
                .HasForeignKey(al => al.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SystemSetting>()
                .HasOne(ss => ss.UpdatedByUser)
                .WithMany()
                .HasForeignKey(ss => ss.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BackupHistory>()
                .HasOne(bh => bh.CreatedByUser)
                .WithMany()
                .HasForeignKey(bh => bh.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // ==================== INDEXES ====================

            // User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Property
            modelBuilder.Entity<Property>()
                .HasIndex(p => p.Status);

            modelBuilder.Entity<Property>()
                .HasIndex(p => new { p.City, p.District });

            // Lease
            modelBuilder.Entity<Lease>()
                .HasIndex(l => l.LeaseNumber)
                .IsUnique();

            modelBuilder.Entity<Lease>()
                .HasIndex(l => l.Status);

            // Invoice
            modelBuilder.Entity<Invoice>()
                .HasIndex(i => i.InvoiceNumber)
                .IsUnique();

            modelBuilder.Entity<Invoice>()
                .HasIndex(i => i.Status);

            modelBuilder.Entity<Invoice>()
                .HasIndex(i => i.DueDate);

            // Payment
            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.PaymentNumber)
                .IsUnique();

            // RentalApplication
            modelBuilder.Entity<RentalApplication>()
                .HasIndex(ra => ra.ApplicationNumber)
                .IsUnique();

            modelBuilder.Entity<RentalApplication>()
                .HasIndex(ra => ra.Status);

            // MaintenanceRequest
            modelBuilder.Entity<MaintenanceRequest>()
                .HasIndex(mr => mr.RequestNumber)
                .IsUnique();

            modelBuilder.Entity<MaintenanceRequest>()
                .HasIndex(mr => mr.Status);

            modelBuilder.Entity<MaintenanceRequest>()
                .HasIndex(mr => mr.Priority);

            // Notification
            modelBuilder.Entity<Notification>()
                .HasIndex(n => new { n.UserId, n.IsRead });

            // ChatMessage
            modelBuilder.Entity<ChatMessage>()
                .HasIndex(cm => cm.CreatedAt);

            // SystemSetting
            modelBuilder.Entity<SystemSetting>()
                .HasIndex(ss => ss.SettingKey)
                .IsUnique();
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