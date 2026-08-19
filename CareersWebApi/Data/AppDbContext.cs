using CareersWebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace CareersWebApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<JobEntity> Jobs => Set<JobEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<JobEntity>(b =>
        {
            b.ToTable("Jobs");
            b.HasKey(x => x.Id);
            b.Property(x => x.Title).IsRequired().HasMaxLength(250);
            b.Property(x => x.Location).HasMaxLength(200);
            b.Property(x => x.Department).HasMaxLength(200);
            b.Property(x => x.PublishedAt).IsRequired();
            b.Property(x => x.AbsoluteUrl).HasMaxLength(1000);
            b.Property(x => x.Content).HasColumnType("TEXT");
        });
    }
}
