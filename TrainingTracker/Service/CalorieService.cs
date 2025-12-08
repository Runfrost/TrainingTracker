using TrainingTracker.ViewModel;

namespace TrainingTracker.Service
{
    public class CalorieService
    {
        public static double CalculateCalories(double weight, double duration, SportType activity)
        {
            if (weight > 30 && weight <= 250 && duration > 0 && duration < 86400)
            {
                double met = activity switch
                {
                    SportType.Generic => 3.5,
                    SportType.Walking => 3.5,
                    SportType.Running => 8.0,
                    SportType.Cycling => 10.0,

                    _ => 1.0

                };

                double hours = duration / 3600.0;
                return met * weight * hours;
            }
            else
            {
                return 0;
            }
        }
    }
}
