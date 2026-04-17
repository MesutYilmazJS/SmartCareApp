using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SmartCareApp.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var contentRoot = Directory.GetCurrentDirectory();
        var defaultDatabasePath = Path.Combine(contentRoot, "App_Data", "smartcareapp.db");
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__SmartCareDb")
            ?? $"Data Source={defaultDatabasePath}";

        Directory.CreateDirectory(Path.Combine(contentRoot, "App_Data"));
        optionsBuilder.UseSqlite(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
