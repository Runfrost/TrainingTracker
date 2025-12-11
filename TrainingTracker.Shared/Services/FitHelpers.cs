using TrainingTracker.Shared.Enums;

namespace TrainingTracker.Shared.Services
{
    public static class FitHelpers
    {
        private static readonly DateTime FitEpoch =
            new DateTime(1989, 12, 31, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime FromFitTimestamp(long fitTimestamp)
        {
            return FitEpoch.AddSeconds(fitTimestamp);
        }
        public static double MetersToKm(double meters)
        {
            return meters / 1000.0;
        }
        public static double SpeedToKmH(double speedMps)
        {
            return speedMps * 3.6;
        }
        public static string AdjustedAvgCadence(int cadence, SportType sport)
        {
            if (sport == SportType.Running)
            {
                return $"{cadence * 2} spm"; // running cadence → steps per minute
            }
            else
            {
                return $"{cadence} rpm"; // cycling or other sports → leave as-is
            }
        }
        public static string SpeedToPace(double speedMps)
        {
            if (speedMps <= 0) return "-";

            double paceSeconds = 1000 / speedMps; // seconds per km
            int minutes = (int)(paceSeconds / 60);
            int seconds = (int)(paceSeconds % 60);

            return $"{minutes}:{seconds:00} min/km";
        }
        public static string SecondsToHms(double seconds)
        {
            TimeSpan t = TimeSpan.FromSeconds(seconds);

            if (t.TotalHours >= 1)
                return $"{(int)t.TotalHours}:{t.Minutes:00}:{t.Seconds:00}";

            return $"{t.Minutes}:{t.Seconds:00}";
        }
    }
}
