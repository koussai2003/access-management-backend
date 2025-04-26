using Microsoft.EntityFrameworkCore;
using AccessManagementAPI.Models; 

namespace AccessManagementAPI.Data  
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<UserAccessRequest> UserAccessRequests { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<ModuleFunction> ModuleFunctions { get; set; }
        public DbSet<RequestValidation> RequestValidations { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<EmailNotification> EmailNotifications { get; set; }


    }
}
