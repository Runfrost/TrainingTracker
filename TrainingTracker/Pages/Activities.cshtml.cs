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

namespace TrainingTracker.Pages
{
    public class ActivitiesModel : PageModel
    {
        private readonly HttpClient _http;
        private readonly ActivityAPIManager _api;
        private readonly ILogger<IndexModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public ActivitiesModel(ILogger<IndexModel> logger, IHttpClientFactory factory, ActivityAPIManager api, UserManager<ApplicationUser> user)
        {
            _logger = logger;
            _http = factory.CreateClient("Backend");
            _api = api;
            _userManager = user;
        }

        public List<SelectListItem> ActivityTypes { get; set; }

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
            LoadActivityTypes();
        }
        public async Task<IActionResult> OnPostFilterAsync()
        {
            await LoadFiltersAsync();
            ActivityTotal = CalculateTotalsToClass();
            LoadActivityTypes();
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            
            if (!ModelState.IsValid)
            {
                await LoadFiltersAsync();
                LoadActivityTypes();
                return Page();
            }

            if (Activity.Id == null)
            {
                Activity.UserId = _userManager.GetUserId(User);
                await _api.SaveActivity(Activity);
            }
            else
            {
                await _api.UpdateActivity(Activity, (int)Activity.Id);
            }

            return RedirectToPage("./Activities");
        }

        private void LoadActivityTypes()
        {
            ActivityTypes = new List<SelectListItem>
            {
                new SelectListItem { Value = "Running", Text = "Running" },
                new SelectListItem { Value = "Walking", Text = "Walking" },
                new SelectListItem { Value = "Cycling", Text = "Cycling" }
            };
        }

        private async Task LoadFiltersAsync()
        {
            var userId = _userManager.GetUserId(User);
            var allActivities = await _api.GetAllActivities(userId);

            var filtered = new List<ActivityViewModel>();

            if (ShowCycling)
                filtered.AddRange(allActivities.Where(a => a.Type == "Cycling"));

            if (ShowRunning)
                filtered.AddRange(allActivities.Where(a => a.Type == "Running"));

            if (ShowWalking)
                filtered.AddRange(allActivities.Where(a => a.Type == "Walking"));

            Activities = filtered;
        }
        //public void CalculateTotals()
        //{
        //    int currentWeek = ISOWeek.GetWeekOfYear(DateTime.Now);
        //    int previousWeek = ISOWeek.GetWeekOfYear(DateTime.Now.AddDays(-7));

        //    int currentMonth = DateTime.Now.Month;
        //    int previousMonth = DateTime.Now.AddMonths(-1).Month;

        //    //Vecka
        //    var weekActivities = Activities.Where(a => ISOWeek.GetWeekOfYear(a.ActivityDate) == currentWeek);
        //    var prevWeekActivities = Activities.Where(a => ISOWeek.GetWeekOfYear(a.ActivityDate) == previousWeek);

        //    TotalActivitiesThisWeek = weekActivities.Count();
        //    TotalActivitiesPreviousWeek = prevWeekActivities.Count();

        //    TotalDistanceThisWeek = weekActivities.Sum(a => a.Distance);
        //    TotalDistancePreviousWeek = prevWeekActivities.Sum(a => a.Distance);

        //    //Månad
        //    var monthActivities = Activities.Where(a => a.ActivityDate.Month == currentMonth);
        //    var prevMonthActivities = Activities.Where(a => a.ActivityDate.Month == previousMonth);

        //    TotalActivitiesThisMonth = monthActivities.Count();
        //    TotalActivitiesPreviousMonth = prevMonthActivities.Count();

        //    TotalDistanceThisMonth = monthActivities.Sum(a => a.Distance);
        //    TotalDistancePreviousMonth = prevMonthActivities.Sum(a => a.Distance);
        //}

        public ActivityTotals CalculateTotalsToClass()
        {
            var now = DateTime.Now;

            var thisWeekNumber = ISOWeek.GetWeekOfYear(now);
            var previousWeekNumber = ISOWeek.GetWeekOfYear(now.AddDays(-7));
            var thisMonth = now.Month;
            var previousMonth = now.AddMonths(-1).Month;

            var totals = new ActivityTotals
            {
                TotalActivitiesThisWeek = Activities.Count(a => ISOWeek.GetWeekOfYear(a.ActivityDate) == thisWeekNumber),
                TotalActivitiesPreviousWeek = Activities.Count(a => ISOWeek.GetWeekOfYear(a.ActivityDate) == previousWeekNumber),
                TotalDistanceThisWeek = Activities.Where(a => ISOWeek.GetWeekOfYear(a.ActivityDate) == thisWeekNumber)
                                                  .Sum(a => a.Distance),
                TotalDistancePreviousWeek = Activities.Where(a => ISOWeek.GetWeekOfYear(a.ActivityDate) == previousWeekNumber)
                                                      .Sum(a => a.Distance),
                TotalActivitiesThisMonth = Activities.Count(a => a.ActivityDate.Month == thisMonth),
                TotalActivitiesPreviousMonth = Activities.Count(a => a.ActivityDate.Month == previousMonth),
                TotalDistanceThisMonth = Activities.Where(a => a.ActivityDate.Month == thisMonth)
                                                   .Sum(a => a.Distance),
                TotalDistancePreviousMonth = Activities.Where(a => a.ActivityDate.Month == previousMonth)
                                                       .Sum(a => a.Distance),
                TotalCaloriesBurntThisMonth = Activities.Where(a => a.ActivityDate.Month == thisMonth)
                                                       .Sum(a => a.CaloriesBurnt),
                TotalCaloriesBurntPreviousMonth = Activities.Where(a => a.ActivityDate.Month == previousMonth)
                                                       .Sum(a => a.CaloriesBurnt),
                TotalCaloriesBurntThisWeek = Activities.Where(a => ISOWeek.GetWeekOfYear(a.ActivityDate) == thisWeekNumber)
                                                       .Sum(a => a.CaloriesBurnt),
                TotalCaloriesBurntPreviousWeek = Activities.Where(a => ISOWeek.GetWeekOfYear(a.ActivityDate) == previousWeekNumber)
                                                       .Sum(a => a.CaloriesBurnt),
            };

            return totals;
        }

        public class ActivityTotals
        {
            public double TotalCaloriesBurntPreviousWeek { get; set; }
            public double TotalCaloriesBurntThisWeek { get; set; }
            public double TotalCaloriesBurntPreviousMonth { get; set; }
            public double TotalCaloriesBurntThisMonth { get; set; }
            public int TotalActivitiesThisWeek { get; set; }
            public int TotalActivitiesPreviousWeek { get; set; }
            public double TotalDistanceThisWeek { get; set; }
            public double TotalDistancePreviousWeek { get; set; }
            public int TotalActivitiesThisMonth { get; set; }
            public int TotalActivitiesPreviousMonth { get; set; }
            public double TotalDistanceThisMonth { get; set; }
            public double TotalDistancePreviousMonth { get; set; }
        }

    }
}
