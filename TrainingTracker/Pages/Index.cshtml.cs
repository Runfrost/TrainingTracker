using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrainingTracker.DAL;
using TrainingTracker.ViewModel;
using TrainingTrackerAPI.Models;
using static TrainingTracker.Pages.ActivitiesModel;

namespace TrainingTracker.Pages
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _http;
        private readonly ActivityAPIManager _api;
        private readonly ILogger<IndexModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory factory, ActivityAPIManager api, UserManager<ApplicationUser> user)
        {
            _logger = logger;
            _http = factory.CreateClient("Backend");
            _api = api;
            _userManager = user;
        }

        public List<SelectListItem> ActivityTypes { get; set; }

        [BindProperty]
        public ViewModel.ActivityViewModel Activity { get; set; }

        public List<ActivityViewModel> Activities { get; set; }
        public ActivityTotals ActivityTotal { get; set; } = new();

        public async Task OnGetAsync(int deleteId, int editId)
        {
            if (deleteId != 0)
            {
                await _api.DeleteActivity(deleteId);
            }

            if (editId != 0)
            {
                Activity = new();
                var activity = await _api.GetActivity(editId);
                Activity.Id = activity.Id;
                Activity.Name = activity.Name;
                Activity.Distance = activity.Distance;
                Activity.Type = activity.Type;
                Activity.ActivityDate = activity.ActivityDate;
            }
            var userId = _userManager.GetUserId(User);
            Activities = await _api.GetAllActivities(userId);

            ActivityTypes = new List<SelectListItem>
            {
                new SelectListItem { Value = "Running", Text = "Running" },
                new SelectListItem { Value = "Walking", Text = "Walking" },
                new SelectListItem { Value = "Cycling", Text = "Cycling" }

            };

        }
        public async Task<IActionResult> OnPostAsync()
        {

            if (!ModelState.IsValid)
            {
                // Re-populate Activities and ActivityTypes before returning
                var userId = _userManager.GetUserId(User);
                Activities = await _api.GetAllActivities(userId);
                ActivityTypes = new List<SelectListItem>
            {
                new SelectListItem { Value = "Running", Text = "Running" },
                new SelectListItem { Value = "Walking", Text = "Walking" },
                new SelectListItem { Value = "Cycling", Text = "Cycling" }

            };

                return Page();
            }

            if (Activity.Id == null)
            {
                if (_userManager.GetUserId(User) != null)
                {
                    Activity.UserId = _userManager.GetUserId(User);

                }
                //Returns the created actívity
                var response = await _api.SaveActivity(Activity);
            }
            else
            {
                //Returns true/false if it can update
                var response = await _api.UpdateActivity(Activity, (int)Activity.Id);
            }



            return RedirectToPage("./Index");




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
    }
}
