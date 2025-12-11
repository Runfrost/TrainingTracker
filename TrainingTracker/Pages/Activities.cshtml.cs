using TrainingTrackerAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrainingTracker.ViewModel;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Globalization;
using TrainingTracker.Service;
using TrainingTrackerAPI.Services;
using TrainingTracker.Models;
using TrainingTracker.Shared.Enums;


namespace TrainingTracker.Pages
{
    public class ActivitiesModel : PageModel
    {
        private readonly ActivitySummaryService _activitySummaryService;
        private readonly ActivityAPIManager _api;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly FilterActivities _filterActivities;

        public ActivitiesModel(IHttpClientFactory factory, ActivityAPIManager api, UserManager<ApplicationUser> user, ActivitySummaryService activitySummaryService, FilterActivities filterActivities)
        {
            _api = api;
            _userManager = user;
            _activitySummaryService = activitySummaryService;
            _filterActivities = filterActivities;
        }
        public static readonly Dictionary<SortField, string> SortFieldDisplay = new()
{
            { SortField.Type, "Type" },
            { SortField.Title, "Title" },
            { SortField.Distance, "Distance" },
            { SortField.Date, "Date" },
            { SortField.Duration, "Duration" },
            { SortField.Calories, "Calories" },
        };
        public IEnumerable<SelectListItem> SportTypeOptions { get; set; }

        [BindProperty]
        public ActivityViewModel Activity { get; set; } = new();

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
        public SortField SortColumn { get; set; }

        [BindProperty(SupportsGet = true)]
        [ValidateNever]
        public bool SortDescending { get; set; } = false;

        public async Task OnGetAsync(int deleteId, int editId)
        {
            if (deleteId != 0)
                await _api.DeleteActivity(deleteId);

            if (editId != 0)
                Activity = await _api.GetActivity(editId);

            var user = await GetCurrentUserAsync();
            Activities = await _filterActivities.LoadFilteredActivitiesAsync(ShowCycling, ShowRunning, ShowWalking, user.Id);
            Activities = SortColumns.ReturnSortedActivities(Activities, SortDescending, SortColumn);
            ActivityTotal = _activitySummaryService.CalculateActivityIntervals(Activities);
            LoadEnumList();
        }
        public async Task<IActionResult> OnPostFilterAsync()
        {
            var user = await GetCurrentUserAsync();
            Activities = await _filterActivities.LoadFilteredActivitiesAsync(ShowCycling, ShowRunning, ShowWalking, user.Id);
            ActivityTotal = _activitySummaryService.CalculateActivityIntervals(Activities);
            LoadEnumList();
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var currentUser = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync();
                Activities = await _filterActivities.LoadFilteredActivitiesAsync(ShowCycling, ShowRunning, ShowWalking, user.Id);
                LoadEnumList();
                return Page();
            }

            // Add new activity
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
            // Edit new activity
            else
            {
                Activity.TotalTimeInSeconds = (Activity.TimeInput.Hour * 3600) + (Activity.TimeInput.Minute * 60) + Activity.TimeInput.Second;
                if(currentUser != null)
                    Activity.Calories = CalorieService.CalculateCalories(currentUser.Weight ?? 0, Activity.TotalTimeInSeconds, Activity.SportType);
                await _api.UpdateActivity(Activity, (int)Activity.Id);
            }

            return RedirectToPage("./Activities");
        }
        private void LoadEnumList()
        {
            SportTypeOptions = Enum.GetValues(typeof(SportType))
                .Cast<SportType>()
                .Select(a => new SelectListItem
                {
                    Value = ((int)a).ToString(),
                    Text = a.ToString()
                });
        }
        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(User);
        }
    }
}
