namespace TrainingTracker.FitConversion
{
    public class SessionInfo
    {
        public long StartTime { get; set; }
        public double TotalTimerTime { get; set; }
        public double TotalDistance { get; set; }
        public int TotalCalories { get; set; }
        public double AvgSpeed { get; set; }
        public double AvgPower { get; set; }
        public int Sport { get; set; }
        public int AvgHeartRate { get; set; }
        public int AvgCadence { get; set; }


        public DateTime StartDateTime => FitHelpers.FromFitTimestamp(StartTime);

        public double DistanceKm => FitHelpers.MetersToKm(TotalDistance);

        public double AvgSpeedKmH => FitHelpers.SpeedToKmH(AvgSpeed);

        public string TotalTimeHms => FitHelpers.SecondsToHms(TotalTimerTime);

        public string AvgCadenceActual => FitHelpers.AdjustedAvgCadence(AvgCadence, SportEnum);

        public string AvgPace => FitHelpers.SpeedToPace(AvgSpeed);

        public FitSport SportEnum => (FitSport)Sport;
    }
}
