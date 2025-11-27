using System.ComponentModel.DataAnnotations;

namespace TrainingTrackerAPI.Models
{
    public class Activity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime ActivityDate { get; set; }
        public int? AverageHeartRate { get; set; }
        public int? MaxHeartRate { get; set; }
        public double Distance { get; set; }
        public int TotalTimeInSeconds { get; set; }
        public int? CaloriesBurned { get; set; }
        //public ApplicationUser? User { get; set; }
    }

}
