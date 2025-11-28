using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrainingTracker.ViewModel;
using TrainingTracker.DAL;

namespace TrainingTracker.Pages
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _http;
        private readonly ActivityAPIManager _api;
        private readonly ILogger<IndexModel> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory factory, ActivityAPIManager api, UserManager<IdentityUser> user)
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
    }
}
