using System.Globalization;
using TrainingTracker.Models;
using TrainingTracker.ViewModel;


namespace TrainingTrackerAPI.Services
{
    public class ActivitySummaryService
    {
        public ActivityTotals CalculateActivityIntervals(List<ActivityViewModel> activities)
        {
            var now = DateTime.Now;

            var thisWeekNumber = ISOWeek.GetWeekOfYear(now);
            var previousWeekNumber = ISOWeek.GetWeekOfYear(now.AddDays(-7));

            var thisMonth = now.Month;
            var previousMonth = now.AddMonths(-1).Month;

            var thisWeek = activities.Where(a => ISOWeek.GetWeekOfYear(a.ActivityDate) == thisWeekNumber).ToList();
            var previousWeek = activities.Where(a => ISOWeek.GetWeekOfYear(a.ActivityDate) == previousWeekNumber).ToList();

            var thisMonthActs = activities.Where(a => a.ActivityDate.Month == thisMonth).ToList();
            var previousMonthActs = activities.Where(a => a.ActivityDate.Month == previousMonth).ToList();

            return new ActivityTotals
            {
                TotalActivitiesThisWeek = thisWeek.Count,
                TotalDistanceThisWeek = thisWeek.Sum(a => a.Distance),
                TotalCaloriesBurntThisWeek = (double)thisWeek.Sum(a => a.Calories ?? 0),
                TotalDurationThisWeek = thisWeek.Sum(a => a.TotalTimeInSeconds),

                TotalActivitiesPreviousWeek = previousWeek.Count,
                TotalDistancePreviousWeek = previousWeek.Sum(a => a.Distance),
                TotalCaloriesBurntPreviousWeek = (double)previousWeek.Sum(a => a.Calories ?? 0),
                TotalDurationPreviousWeek = previousWeek.Sum(a => a.TotalTimeInSeconds),

                TotalActivitiesThisMonth = thisMonthActs.Count,
                TotalDistanceThisMonth = thisMonthActs.Sum(a => a.Distance),
                TotalCaloriesBurntThisMonth = (double)thisMonthActs.Sum(a => a.Calories ?? 0),
                TotalDurationThisMonth = thisMonthActs.Sum(a => a.TotalTimeInSeconds),

                TotalActivitiesPreviousMonth = previousMonthActs.Count,
                TotalDistancePreviousMonth = previousMonthActs.Sum(a => a.Distance),
                TotalCaloriesBurntPreviousMonth = (double)previousMonthActs.Sum(a => a.Calories ?? 0),
                TotalDurationPreviousMonth = previousMonthActs.Sum(a => a.TotalTimeInSeconds),

            };
        }
    }
}
