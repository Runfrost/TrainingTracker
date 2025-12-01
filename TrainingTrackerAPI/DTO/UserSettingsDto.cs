using TrainingTrackerAPI.Models;

namespace TrainingTrackerAPI.DTO
{
    public class UserSettingsDto
    {
        public string? Name { get; set; }
        public int? YearOfBirth { get; set; }
        public double? Weight { get; set; }
        public double? Height { get; set; }
        public GenderType? gender { get; set; }

    }
}
