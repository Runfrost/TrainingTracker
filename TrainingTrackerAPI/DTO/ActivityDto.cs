namespace TrainingTrackerAPI.DTO
{
    public class ActivityDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public double Distance { get; set; }
        public DateTime ActivityDate { get; set; }

        // Running-specific props
        public int? TotalTimeInSeconds { get; set; }
        public int? AverageCadence { get; set; }
    }
}
