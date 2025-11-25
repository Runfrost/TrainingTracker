using TrainingTrackerAPI.Models;

namespace TrainingTrackerAPI.DTO
{
    public class ActivitesCreateDto
    {
        public string Name { get; set; }
        public double Distance { get; set; }
        public string Type { get; set; }

        //public ApplicationUser User { get; set; }
    }
}
