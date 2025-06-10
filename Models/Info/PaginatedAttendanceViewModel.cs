using Factory360.Data;

namespace Factory360.Models;

public class PaginatedAttendanceViewModel
{
    public List<AttendanceRecord> Records { get; set; } = null!;
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}