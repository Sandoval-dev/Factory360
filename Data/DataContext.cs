using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Factory360.Data;

public class DataContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {

    }

    public DbSet<AttendanceRecord> AttendanceRecords { get; set; } = null!;
    public DbSet<Salary> Salaries { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppUser>()
        .HasMany(u => u.AttendanceRecords)
        .WithOne(a => a.User)
        .HasForeignKey(a => a.UserId);

    }

}