using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Tasks;
using TaskManager.Infrastructure.Identity;

namespace TaskManager.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole, string>(options)
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<TaskItem>(b =>
        {
            b.ToTable("Tasks");
            b.HasKey(x => x.Id);

            b.Property(x => x.OwnerUserId).IsRequired();

            b.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            b.Property(x => x.Description)
                .HasMaxLength(4000);

            b.Property(x => x.Status).IsRequired();

            b.Property(x => x.CreatedAt).IsRequired();
            b.Property(x => x.UpdatedAt).IsRequired();

            b.HasIndex(x => new { x.OwnerUserId, x.Status });
        });
    }
}

