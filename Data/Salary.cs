namespace Factory360.Data;

public class Salary
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal SalaryAmount { get; set; }

    public AppUser User { get; set; } = null!;
}
