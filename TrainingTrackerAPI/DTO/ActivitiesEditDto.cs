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
    }
}
