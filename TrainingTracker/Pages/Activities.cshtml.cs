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

namespace TrainingTracker.Pages
{
    public class ActivitiesModel : PageModel
    {
        private readonly ActivitySummaryService _activitySummaryService;
        private readonly ActivityAPIManager _api;
        private readonly ILogger<IndexModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public ActivitiesModel(ILogger<IndexModel> logger, IHttpClientFactory factory, ActivityAPIManager api, UserManager<ApplicationUser> user, ActivitySummaryService activitySummaryService)
        {
            _logger = logger;
            _api = api;
            _userManager = user;
            _activitySummaryService = activitySummaryService;
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

            await LoadFilteredActivitiesAsync();
            SortColumns(Activities);
            ActivityTotal = _activitySummaryService.CalculateActivityIntervals(Activities);
            LoadEnumList();
        }
        public async Task<IActionResult> OnPostFilterAsync()
        {
            await LoadFilteredActivitiesAsync();
            ActivityTotal = _activitySummaryService.CalculateActivityIntervals(Activities);
            LoadEnumList();
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var currentUser = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                await LoadFilteredActivitiesAsync();
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
        private void SortColumns(List<ActivityViewModel> activities)
        {
            Activities = SortColumn switch
            {
                SortField.Type => SortDescending
                    ? activities.OrderByDescending(a => a.SportType.ToString()).ToList()
                    : activities.OrderBy(a => a.SportType.ToString()).ToList(),

                SortField.Title => SortDescending
                    ? activities.OrderByDescending(a => a.Name).ToList()
                    : activities.OrderBy(a => a.Name).ToList(),

                SortField.Date => SortDescending
                    ? activities.OrderByDescending(a => a.ActivityDate).ToList()
                    : activities.OrderBy(a => a.ActivityDate).ToList(),

                SortField.Distance => SortDescending
                    ? activities.OrderByDescending(a => a.Distance).ToList()
                    : activities.OrderBy(a => a.Distance).ToList(),

                SortField.Duration => SortDescending
                    ? activities.OrderByDescending(a => a.TotalTimeInSeconds).ToList()
                    : activities.OrderBy(a => a.TotalTimeInSeconds).ToList(),

                SortField.Calories => SortDescending
                    ? activities.OrderByDescending(a => a.Calories).ToList()
                    : activities.OrderBy(a => a.Calories).ToList(),

                _ => Activities
            };
        }
        private async Task LoadFilteredActivitiesAsync()
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
        public void LoadEnumList()
        {
            SportTypeOptions = Enum.GetValues(typeof(ViewModel.SportType))
                .Cast<ViewModel.SportType>()
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
