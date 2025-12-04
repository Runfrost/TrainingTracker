namespace TrainingTracker.Models
{
    public class ActivityTotals
    {
        public double TotalDurationPreviousWeek { get; set; }
        public double TotalDurationThisWeek { get; set; }
        public double TotalDurationPreviousMonth { get; set; }
        public double TotalDurationThisMonth { get; set; }
        public double TotalCaloriesBurntPreviousWeek { get; set; }
        public double TotalCaloriesBurntThisWeek { get; set; }
        public double TotalCaloriesBurntPreviousMonth { get; set; }
        public double TotalCaloriesBurntThisMonth { get; set; }
        public int TotalActivitiesThisWeek { get; set; }
        public int TotalActivitiesPreviousWeek { get; set; }
        public int TotalActivitiesThisMonth { get; set; }
        public int TotalActivitiesPreviousMonth { get; set; }
        public double TotalDistanceThisWeek { get; set; }
        public double TotalDistancePreviousWeek { get; set; }
        public double TotalDistanceThisMonth { get; set; }
        public double TotalDistancePreviousMonth { get; set; }

        public List<MetricRow> WeeklyRows =>
    new()
    {
            new MetricRow { Label = "Activities", ThisPeriod = TotalActivitiesThisWeek, PreviousPeriod = TotalActivitiesPreviousWeek },
            new MetricRow { Label = "Distance", ThisPeriod = TotalDistanceThisWeek, PreviousPeriod = TotalDistancePreviousWeek, Unit = "km" },
            new MetricRow { Label = "Duration", ThisPeriod = TotalDurationThisWeek, PreviousPeriod = TotalDurationPreviousWeek, Unit = "s" },
            new MetricRow { Label = "Calories", ThisPeriod = TotalCaloriesBurntThisWeek, PreviousPeriod = TotalCaloriesBurntPreviousWeek, Unit = "cal" }
    };

        public List<MetricRow> MonthlyRows =>
            new()
            {
            new MetricRow { Label = "Activities", ThisPeriod = TotalActivitiesThisMonth, PreviousPeriod = TotalActivitiesPreviousMonth },
            new MetricRow { Label = "Distance", ThisPeriod = TotalDistanceThisMonth, PreviousPeriod = TotalDistancePreviousMonth, Unit = "km" },
            new MetricRow { Label = "Duration", ThisPeriod = TotalDurationThisMonth, PreviousPeriod = TotalDurationPreviousMonth, Unit = "s" },
            new MetricRow { Label = "Calories", ThisPeriod = TotalCaloriesBurntThisMonth, PreviousPeriod = TotalCaloriesBurntPreviousMonth, Unit = "cal" }
            };
    }
}
