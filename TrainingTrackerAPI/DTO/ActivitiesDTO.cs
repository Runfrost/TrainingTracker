using TrainingTrackerAPI.Models;

namespace TrainingTrackerAPI.DTO
{
    public class ActivitiesDTO
    {
        public int? Id { get; set; }
        public string Name { get; set; } = "";
        public double Distance { get; set; }
        public DateTime ActivityDate { get; set; }
        public string? UserId { get; set; }
        public int TotalTimeInSeconds { get; set; }
        public TimeOnly TimeInput { get; set; }
        public double Calories { get; set; }
        public SportType SportType { get; set; }
        public int? AverageCadence { get; set; }



        //public ApplicationUser User { get; set; }
    }
}
