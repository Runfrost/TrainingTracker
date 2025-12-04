using TrainingTrackerAPI.Models;

namespace TrainingTrackerAPI.DTO
{
    public class ActivitiesEditDto
    {
        public string Name { get; set; }
        public double Distance { get; set; }
        public DateTime ActivityDate { get; set; }
        public int TotalTimeInSeconds { get; set; }
        public TimeOnly TimeInput { get; set; }
        public int AverageCadence { get; set; }
        public double? Calories { get; set; }
        public SportType SportType { get; set; }
    }
}
