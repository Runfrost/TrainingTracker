using Microsoft.AspNetCore.Identity;

namespace TrainingTrackerAPI.Models;

public class ApplicationUser : IdentityUser
{
    public string? Name { get; set; }
}