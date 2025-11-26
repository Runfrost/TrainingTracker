using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Infrastructure;
using TrainingTracker.DAL;
using TrainingTrackerAPI.Models;

namespace TrainingTracker.Pages
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _http;
        private readonly ActivityAPIManager _api;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory factory, ActivityAPIManager api)
        {
            _logger = logger;
            _http = factory.CreateClient("Backend");
            _api = api;
        }

        public List<SelectListItem> ActivityTypes { get; set; }

        [BindProperty]
        public TrainingTrackerAPI.DTO.ActivitesCreateDto Activity { get; set; }

        public List<ActivityDto> Activities { get; set; }

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
                Activity.ActivityDate = DateTime.Now;
            }
            Activities = await _api.GetAllActivities();

            ActivityTypes = new List<SelectListItem>
            {
                new SelectListItem { Value = "Running", Text = "Running" },
                new SelectListItem { Value = "Walking", Text = "Walking" },
                new SelectListItem { Value = "Cycling", Text = "Cycling" }

            };

        }
        public async Task<IActionResult> OnPostAsync()
        {
            if(Activity.Id == null)
            {
                await DAL.ActivityAPIManager.SaveActivity(Activity);
            }
            else
            {
                await ActivityAPIManager.UpdateActivity(Activity, (int)Activity.Id);
            }



                return RedirectToPage("./Index");

        }
    }
}
