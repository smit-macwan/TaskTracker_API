using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Tasks;
using TaskManager.Infrastructure.Identity;
using TaskManager.Infrastructure.Persistence;
using TaskManager.Infrastructure.Tasks;

namespace TaskManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config,
        string contentRootPath)
    {
        services.AddDbContext<AppDbContext>(opt =>
        {
            var configuredConnectionString =
                config.GetConnectionString("Default") ?? "Data Source=Data/taskmanager.db";
            var normalizedConnectionString = NormalizeSqliteConnectionString(configuredConnectionString, contentRootPath);
            opt.UseSqlite(normalizedConnectionString);
        });

        services
            .AddIdentityCore<ApplicationUser>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 6;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>();

        services.AddScoped<ITaskRepository, EfTaskRepository>();

        return services;
    }

    private static string NormalizeSqliteConnectionString(string connectionString, string contentRootPath)
    {
        var builder = new SqliteConnectionStringBuilder(connectionString);
        var source = builder.DataSource;

        if (string.IsNullOrWhiteSpace(source))
        {
            source = "Data/taskmanager.db";
        }

        if (!Path.IsPathRooted(source))
        {
            source = Path.GetFullPath(Path.Combine(contentRootPath, source));
        }

        var directory = Path.GetDirectoryName(source);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        builder.DataSource = source;
        return builder.ToString();
    }
}

