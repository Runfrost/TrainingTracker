using TrainingTracker.FitConversion;

namespace TrainingTracker.Service
{
    public class CalorieService
    {
        public static double CalculateCalories(double weight, double duration, FitSport activity)
        {
            double met = activity switch
            {
                FitSport.Generic => 3.5,
                FitSport.Running => 8.0,
                FitSport.Cycling => 10.0,
                _ => 1.0
            };

            double hours = duration / 3600.0;
            return met * weight * hours;
        }
    }
}
