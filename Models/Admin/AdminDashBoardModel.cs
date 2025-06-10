using Factory360.Data;

namespace Factory360.Models;

public class AdminDashboardModel
{
    public AppUser CurrentUser { get; set; } = null!; // Ensure this is initialized properly in the controller
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public string? MostWorkingUserFullName { get; set; }
    public string? LastWorker { get; set; }
}