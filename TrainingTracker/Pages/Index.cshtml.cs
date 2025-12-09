using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using TrainingTracker.Service;
using TrainingTracker.ViewModel;
using TrainingTrackerAPI.Models;


public class IndexModel : PageModel
    {
        private readonly ActivityAPIManager _api;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ActivityAPIManager api, UserManager<ApplicationUser> user)
        {
            _api = api;
            _userManager = user;
        }

        public List<ActivityViewModel> Activities { get; set; } = new();
        public List<DateTime> ActivityDates { get; set; } = new();
        public ActivityTotals ActivityTotal { get; set; } = new();

        public async Task OnGetAsync()
        {
            var userId = _userManager.GetUserId(User);
            Activities = await _api.GetAllActivities(userId);

            // Lista med datum för kalender
            ActivityDates = Activities
                .Select(a => a.ActivityDate.Date)
                .Distinct()
                .ToList();

            ActivityTotal = CalculateTotalsToClass();
        }

        // ===== ActivityTotals och metoder =====
        public ActivityTotals CalculateTotalsToClass()
        {
            var now = DateTime.Now;
            var thisWeekNumber = ISOWeek.GetWeekOfYear(now);
            var previousWeekNumber = ISOWeek.GetWeekOfYear(now.AddDays(-7));
            var thisMonth = now.Month;
            var previousMonth = now.AddMonths(-1).Month;

            var thisWeek = Activities.Where(a => ISOWeek.GetWeekOfYear(a.ActivityDate) == thisWeekNumber).ToList();
            var previousWeek = Activities.Where(a => ISOWeek.GetWeekOfYear(a.ActivityDate) == previousWeekNumber).ToList();
            var thisMonthActs = Activities.Where(a => a.ActivityDate.Month == thisMonth).ToList();
            var previousMonthActs = Activities.Where(a => a.ActivityDate.Month == previousMonth).ToList();

            return new ActivityTotals
            {
                TotalActivitiesThisWeek = thisWeek.Count,
                TotalDistanceThisWeek = thisWeek.Sum(a => a.Distance),
                TotalCaloriesBurntThisWeek = thisWeek.Sum(a => a.CaloriesBurnt),
                TotalDurationThisWeek = thisWeek.Sum(a => a.TotalTimeInSeconds),

                TotalActivitiesPreviousWeek = previousWeek.Count,
                TotalDistancePreviousWeek = previousWeek.Sum(a => a.Distance),
                TotalCaloriesBurntPreviousWeek = previousWeek.Sum(a => a.CaloriesBurnt),
                TotalDurationPreviousWeek = previousWeek.Sum(a => a.TotalTimeInSeconds),

                TotalActivitiesThisMonth = thisMonthActs.Count,
                TotalDistanceThisMonth = thisMonthActs.Sum(a => a.Distance),
                TotalCaloriesBurntThisMonth = thisMonthActs.Sum(a => a.CaloriesBurnt),
                TotalDurationThisMonth = thisMonthActs.Sum(a => a.TotalTimeInSeconds),

                TotalActivitiesPreviousMonth = previousMonthActs.Count,
                TotalDistancePreviousMonth = previousMonthActs.Sum(a => a.Distance),
                TotalCaloriesBurntPreviousMonth = previousMonthActs.Sum(a => a.CaloriesBurnt),
                TotalDurationPreviousMonth = previousMonthActs.Sum(a => a.TotalTimeInSeconds),
            };
        }

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

            public List<MetricRow> WeeklyRows => new()
            {
                new MetricRow { Label = "Activities", ThisPeriod = TotalActivitiesThisWeek, PreviousPeriod = TotalActivitiesPreviousWeek },
                new MetricRow { Label = "Distance", ThisPeriod = TotalDistanceThisWeek, PreviousPeriod = TotalDistancePreviousWeek, Unit = "km" },
                new MetricRow { Label = "Duration", ThisPeriod = TotalDurationThisWeek, PreviousPeriod = TotalDurationPreviousWeek, Unit = "s" },
                new MetricRow { Label = "Calories", ThisPeriod = TotalCaloriesBurntThisWeek, PreviousPeriod = TotalCaloriesBurntPreviousWeek, Unit = "cal" }
            };

            public List<MetricRow> MonthlyRows => new()
            {
                new MetricRow { Label = "Activities", ThisPeriod = TotalActivitiesThisMonth, PreviousPeriod = TotalActivitiesPreviousMonth },
                new MetricRow { Label = "Distance", ThisPeriod = TotalDistanceThisMonth, PreviousPeriod = TotalDistancePreviousMonth, Unit = "km" },
                new MetricRow { Label = "Duration", ThisPeriod = TotalDurationThisMonth, PreviousPeriod = TotalDurationPreviousMonth, Unit = "s" },
                new MetricRow { Label = "Calories", ThisPeriod = TotalCaloriesBurntThisMonth, PreviousPeriod = TotalCaloriesBurntPreviousMonth, Unit = "cal" }
            };
        }

        public class MetricRow
        {
            public string Label { get; set; } = "";
            public double ThisPeriod { get; set; }
            public double PreviousPeriod { get; set; }
            public bool IsUp => ThisPeriod >= PreviousPeriod;
            public string Unit { get; set; } = "";
        }
    }


public class Workout
{
    public DateTime Date { get; set; }
    public string Type { get; set; }
}
