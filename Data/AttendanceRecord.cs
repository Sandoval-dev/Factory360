namespace Factory360.Data;

public class AttendanceRecord
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public decimal? TotalHours { get; set; }
    public AppUser User { get; set; } = null!;
}