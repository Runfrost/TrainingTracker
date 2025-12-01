using Microsoft.AspNetCore.Identity;

namespace TrainingTrackerAPI.Models;

public class ApplicationUser : IdentityUser
{
    public string? Name { get; set; }
    public int? YearOfBirth { get; set; }
    public double? Weight { get; set; }
    public double? Height { get; set; }
    public GenderType? Gender { get; set; }

}

public enum GenderType
{
    Male,
    Female,
    PreferNotToSay

}