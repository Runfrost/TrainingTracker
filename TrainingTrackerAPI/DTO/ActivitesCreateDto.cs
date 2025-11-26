using TrainingTrackerAPI.Models;

namespace TrainingTrackerAPI.DTO
{
    public class ActivitesCreateDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public double Distance { get; set; }
        public string Type { get; set; }
        public DateTime ActivityDate { get; set; }

        //public ApplicationUser User { get; set; }
    }
}
