using GondolinWeb.Areas.Application.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GondolinWeb.Areas.Identity.Data;

public class GondolinWebIdentityDBContext : IdentityDbContext<ApplicationUser>
{
    public GondolinWebIdentityDBContext(DbContextOptions<GondolinWebIdentityDBContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

        builder.ApplyConfiguration(new ApplicationUserEntityConfiguration());
    }

    public DbSet<Application.Models.Task>? Tasks { get; set; }
    public DbSet<UserTask> UserTasks { get; set; }
    public DbSet<UserProject> UserProjects { get; set; }
    public DbSet<GondolinWeb.Areas.Application.Models.Project>? Project { get; set; }
    public DbSet<TaskCategory> TaskCategories { get; set; }
}

public class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.FirstName).HasMaxLength(255);
        builder.Property(u => u.LastName).HasMaxLength(255);
    }
}