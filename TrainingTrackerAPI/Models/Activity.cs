using System.ComponentModel.DataAnnotations;

namespace TrainingTrackerAPI.Models
{
    public class Activity
    {
        public int Id { get; set; }
        [Required]
        [StringLength(30, ErrorMessage = "namnet är för långt")]
        public string Name { get; set; }
        public DateTime ActivityDate { get; set; }
        public int? AverageHeartRate { get; set; }
        public int? MaxHeartRate { get; set; }
        [Range (0,2-200, ErrorMessage = "Du är dum i huvudet om du knappat in över 20 mil!")]
        public double Distance { get; set; }
        public int TotalTimeInSeconds { get; set; }
        public int? CaloriesBurned { get; set; }
        //public ApplicationUser? User { get; set; }
    }

}
