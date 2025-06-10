using System.ComponentModel.DataAnnotations;

namespace Factory360.Models;


public class EmployeeCreateModel
{
    [Required]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    public string Password { get; set; } = null!;

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Please enter a valid wage.")]
    [Display(Name = "Hourly Wage")]
    public int HourlyWage { get; set; }

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Roles")]
    public IList<string> SelectedRoles { get; set; } = new List<string>();

}