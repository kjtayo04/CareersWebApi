using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CareersWebApi.Data;

// Design-time factory for EF tools (migrations)
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<AppDbContext>();
        // use SQLite file in project folder by default for migrations
        builder.UseSqlite("Data Source=careers.db");
        return new AppDbContext(builder.Options);
    }
}
