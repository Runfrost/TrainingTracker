using TrainingTrackerAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrainingTracker.ViewModel;
using TrainingTracker.DAL;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Globalization;
using TrainingTracker.Service;
using TrainingTracker.FitConversion;

namespace TrainingTracker.Pages
{
    public class ActivitiesModel : PageModel
    {
        private readonly ActivityAPIManager _api;
        private readonly ILogger<IndexModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public ActivitiesModel(ILogger<IndexModel> logger, IHttpClientFactory factory, ActivityAPIManager api, UserManager<ApplicationUser> user)
        {
            _logger = logger;
            _api = api;
            _userManager = user;
        }

        //public List<SelectListItem> ActivityTypes { get; set; }
        public ViewModel.SportType ActivityType { get; set; }
        public IEnumerable<SelectListItem> ActivityTypeItems { get; set; }

        [BindProperty]
        public ViewModel.ActivityViewModel Activity { get; set; } = new();

        public List<ActivityViewModel> Activities { get; set; }
        public ActivityTotals ActivityTotal { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public bool ShowCycling { get; set; } = true;
        [BindProperty(SupportsGet = true)]
        public bool ShowRunning { get; set; } = true;
        [BindProperty(SupportsGet = true)]
        public bool ShowWalking { get; set; } = true;

        [BindProperty(SupportsGet = true)]
        [ValidateNever]
        public string SortColumn { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        [ValidateNever]
        public bool SortDescending { get; set; } = false;

        public async Task OnGetAsync(int deleteId, int editId)
        {
            
            var userId = _userManager.GetUserId(User);
            if (deleteId != 0)
            {
                await _api.DeleteActivity(deleteId);
            }

            if (editId != 0)
            {
                var activity = await _api.GetActivity(editId);
                if(activity.Id != 0)
                {
                    Activity = new();
                    Activity.Id = activity.Id;
                    Activity.Name = activity.Name;
                    Activity.Distance = activity.Distance;
                    Activity.Type = activity.Type;
                    Activity.ActivityDate = activity.ActivityDate;
                    Activity.TimeInput = activity.TimeInput;
                    Activity.SportType = activity.SportType;
                    Activity.TotalTimeInSeconds = (Activity.TimeInput.Hour * 3600) + (Activity.TimeInput.Minute * 60) + Activity.TimeInput.Second;
                    Activity.Calories = CalorieService.CalculateCalories(70, Activity.TotalTimeInSeconds, Activity.SportType);
                }
            }

            if(string.IsNullOrEmpty(SortColumn))
            {
                SortColumn = "Date";
                SortDescending = true;
            }

            await LoadFiltersAsync();

            Activities = SortColumn switch
            {
                "Type" => SortDescending ? Activities.OrderByDescending(a => a.Type).ToList() : Activities.OrderBy(a => a.Type).ToList(),
                "Title" => SortDescending ? Activities.OrderByDescending(a => a.Name).ToList() : Activities.OrderBy(a => a.Name).ToList(),
                "Distance" => SortDescending ? Activities.OrderByDescending(a => a.Distance).ToList() : Activities.OrderBy(a => a.Distance).ToList(),
                "Date" => SortDescending ? Activities.OrderByDescending(a => a.ActivityDate).ToList() : Activities.OrderBy(a => a.ActivityDate).ToList(),
                _ => Activities
            };
            ActivityTotal = CalculateTotalsToClass();
            LoadEnumList();
        }
        public async Task<IActionResult> OnPostFilterAsync()
        {
            await LoadFiltersAsync();
            ActivityTotal = CalculateTotalsToClass();
            LoadEnumList();
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var currentUser = _userManager.GetUserAsync(User).Result;
            Activity.Type = Activity.SportType.ToString();
            if (!ModelState.IsValid)
            {
                await LoadFiltersAsync();
                LoadEnumList();
                return Page();
            }

            if (Activity.Id == null)
            {
                Activity.TotalTimeInSeconds = (Activity.TimeInput.Hour * 3600) + (Activity.TimeInput.Minute * 60) + Activity.TimeInput.Second;

                if(currentUser != null)
                {
                    Activity.UserId = currentUser.Id;
                    Activity.Calories = CalorieService.CalculateCalories(currentUser.Weight ?? 0, Activity.TotalTimeInSeconds, Activity.SportType);
                }
                    
                await _api.SaveActivity(Activity);
            }
            else
            {
                Activity.TotalTimeInSeconds = (Activity.TimeInput.Hour * 3600) + (Activity.TimeInput.Minute * 60) + Activity.TimeInput.Second;
                if(currentUser != null)
                    Activity.Calories = CalorieService.CalculateCalories(currentUser.Weight ?? 0, Activity.TotalTimeInSeconds, Activity.SportType);
                await _api.UpdateActivity(Activity, (int)Activity.Id);
            }

            return RedirectToPage("./Activities");
        }

        private async Task LoadFiltersAsync()
        {
            var userId = _userManager.GetUserId(User);
            
            var allActivities = await _api.GetAllActivities(userId);

            var filtered = new List<ActivityViewModel>();

            if (ShowCycling)
                filtered.AddRange(allActivities.Where(a => a.SportType == ViewModel.SportType.Cycling));

            if (ShowRunning)
                filtered.AddRange(allActivities.Where(a => a.SportType == ViewModel.SportType.Running));

            if (ShowWalking)
                filtered.AddRange(allActivities.Where(a => a.SportType == ViewModel.SportType.Walking));

            Activities = filtered;
        }

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

        public class ActivityTotals
        {
            public double TotalDurationPreviousWeek { get; set; }
            public double TotalDurationThisWeek { get; set; }
            public double TotalDurationPreviousMonth { get; set; }
            public double TotalDurationThisMonth { get; set; }
            //public string? TotalDurationPreviousWeek { get; set; }
            //public string? TotalDurationThisWeek { get; set; }
            //public string? TotalDurationPreviousMonth { get; set; }
            //public string? TotalDurationThisMonth { get; set; }
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
        public class MetricRow
        {
            public string Label { get; set; } = "";
            //public string DurationThisPeriod { get; set; } = "";
            public double ThisPeriod { get; set; }
            public double PreviousPeriod { get; set; }

            public bool IsUp => ThisPeriod >= PreviousPeriod;

            public string Unit { get; set; } = ""; // optional, e.g. "km"
        }

        public void LoadEnumList()
        {
            ActivityTypeItems = Enum.GetValues(typeof(ViewModel.SportType))
                .Cast<ViewModel.SportType>()
                .Select(a => new SelectListItem
                {
                    Value = ((int)a).ToString(),
                    Text = a.ToString()
                });
        }

    }
}
