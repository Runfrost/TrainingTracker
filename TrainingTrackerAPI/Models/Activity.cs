using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using TrainingTracker.Shared.Enums;

namespace TrainingTrackerAPI.Models
{
    public class Activity
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime ActivityDate { get; set; }
        public int? AverageHeartRate { get; set; }
        public int? MaxHeartRate { get; set; }
        public double Distance { get; set; }
        public int TotalTimeInSeconds { get; set; }
        public TimeOnly TimeInput { get; set; }
        public SportType SportType { get; set; }
        public double? CaloriesBurned { get; set; }
        public ApplicationUser? User { get; set; }
        public int? AvgCadence { get; set; }
        public double? AvgPower { get; set; }

        public string? UserId { get; set; }
    }

}
