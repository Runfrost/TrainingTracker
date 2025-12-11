using System.ComponentModel.DataAnnotations;
using TrainingTracker.Service;
using TrainingTracker.Shared.Enums;

namespace TrainingTracker.ViewModel
{
    public class ActivityViewModel
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "A name is required for the activity")]
        [StringLength(20, ErrorMessage = "Name is too long, limit is 20")]
        public string? Name { get; set; }
        [Required]
        [Range(0.1, 400, ErrorMessage = "Activity too short or too long (max 400km)")]
        public double Distance { get; set; }
        [Required(ErrorMessage = "Date is required")]
        public DateTime ActivityDate { get; set; } = DateTime.Today;
        [Required]
        [Range(typeof(TimeOnly), "00:00:01", "23:59:59", ErrorMessage = "Activity needs to be of a format between 00:00:01 and 23:59:59")]
        public TimeOnly TimeInput { get; set; }
        public SportType SportType { get; set; }
        public int TotalTimeInSeconds { get; set; }
        public double? Calories { get; set; }
        public string? UserId { get; set; }
    }
}
