using TrainingTracker.Service;

namespace TrainingTracker.ViewModel
{
    public class SessionInfo
    {
        public string ActivityName { get; set; } = "New Activity";
        public long StartTime { get; set; }
        public double TotalTimerTime { get; set; }
        public double TotalDistance { get; set; }
        public int TotalCalories { get; set; }
        public double AvgSpeed { get; set; }
        public double AvgPower { get; set; }
        public int Sport { get; set; }
        public int AvgHeartRate { get; set; }
        public int AvgCadence { get; set; }
        public string? UserId { get; set; }


        public DateTime StartDateTime => FitHelpers.FromFitTimestamp(StartTime);

        public double DistanceKm => FitHelpers.MetersToKm(TotalDistance);

        public double AvgSpeedKmH => FitHelpers.SpeedToKmH(AvgSpeed);

        public string TotalTimeHms => FitHelpers.SecondsToHms(TotalTimerTime);

        public string AvgCadenceActual => FitHelpers.AdjustedAvgCadence(AvgCadence, SportEnum);

        public string AvgPace => FitHelpers.SpeedToPace(AvgSpeed);

        public SportType SportEnum => (SportType)Sport;
    }
}
