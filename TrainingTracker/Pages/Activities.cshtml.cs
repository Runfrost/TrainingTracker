using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrainingTracker.ViewModel;
using TrainingTracker.DAL;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TrainingTracker.Pages
{
    public class ActivitiesModel : PageModel
    {
        private readonly HttpClient _http;
        private readonly ActivityAPIManager _api;
        private readonly ILogger<IndexModel> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public ActivitiesModel(ILogger<IndexModel> logger, IHttpClientFactory factory, ActivityAPIManager api, UserManager<IdentityUser> user)
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
            
            LoadActivityTypes();
        }
        public async Task<IActionResult> OnPostFilterAsync()
        {
            await LoadFiltersAsync();
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

    }
}
