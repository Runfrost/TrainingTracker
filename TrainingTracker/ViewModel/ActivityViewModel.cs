using System.ComponentModel.DataAnnotations;

namespace TrainingTracker.ViewModel
{
    public class ActivityViewModel
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "A name is required for the activity")]
        [StringLength(20, ErrorMessage = "Name is too long, limit is 20")]
        public string Name { get; set; }
        [Required]
        [Range(0.1, 250, ErrorMessage = "Activity too short or too long (max 250km)")]
        public double Distance { get; set; }
        [Required(ErrorMessage = "Type of activity needs to be selected")]
        public string Type { get; set; }
        [Required(ErrorMessage = "Date is required")]
        public DateTime ActivityDate { get; set; } = DateTime.Today;

        public int TotalTime { get; set; }
        public int TotalTimeInSeconds { get; set; }

        public string? UserId { get; set; }
    }
}
