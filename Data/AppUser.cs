using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Factory360.Data;

public class AppUser : IdentityUser<int>
{
    public string FullName { get; set; } = null!;
    public int HourlyWage { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
    
    [NotMapped]
    public List<decimal> Salaries { get; set; } = new();
    public bool MustChangePassword { get; set; } = true;

}
