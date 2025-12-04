namespace TrainingTracker.Models
{
    public class MetricRow
    {
        public string Label { get; set; } = "";
        public double ThisPeriod { get; set; }
        public double PreviousPeriod { get; set; }
        public bool IsUp => ThisPeriod >= PreviousPeriod;

        public string Unit { get; set; } = ""; // optional, e.g. "km"
    }
}
