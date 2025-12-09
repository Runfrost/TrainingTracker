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
        public IEnumerable<SelectListItem> SportTypeOptions { get; set; }

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
                    var currentUser = await GetCurrentUserAsync();
                    Activity = new();
                    Activity.Id = activity.Id;
                    Activity.Name = activity.Name;
                    Activity.Distance = activity.Distance;
                    Activity.Type = activity.Type;
                    Activity.ActivityDate = activity.ActivityDate;
                    Activity.TimeInput = activity.TimeInput;
                    Activity.SportType = activity.SportType;
                    Activity.TotalTimeInSeconds = (Activity.TimeInput.Hour * 3600) + (Activity.TimeInput.Minute * 60) + Activity.TimeInput.Second;
                    Activity.Calories = CalorieService.CalculateCalories(currentUser.Weight ?? 0, Activity.TotalTimeInSeconds, Activity.SportType);
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
            ActivityTotal = _activitySummaryService.CalculateActivityIntervals(Activities);
            LoadEnumList();
        }
        public async Task<IActionResult> OnPostFilterAsync()
        {
            await LoadFiltersAsync();
            ActivityTotal = _activitySummaryService.CalculateActivityIntervals(Activities);
            LoadEnumList();
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var currentUser = await GetCurrentUserAsync();
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
