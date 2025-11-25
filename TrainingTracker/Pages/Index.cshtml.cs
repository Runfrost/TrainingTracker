using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Infrastructure;
using TrainingTrackerAPI.Models;

namespace TrainingTracker.Pages
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _http;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory factory)
        {
            _logger = logger;
            _http = factory.CreateClient("Backend");
        }

        public List<SelectListItem> ActivityTypes { get; set; }

        [BindProperty]
        public TrainingTrackerAPI.DTO.ActivitesCreateDto Activity { get; set; }

        [BindProperty]
        public List<Activity>Activities { get; set; }

        public async Task OnGetAsync()
        {
            Activities = await DAL.ActivityAPIManager.GetAllActivities();

            ActivityTypes = new List<SelectListItem>
            {
                new SelectListItem { Value = "Running", Text = "Running" },
                new SelectListItem { Value = "Walking", Text = "Walking" },
                new SelectListItem { Value = "Cycling", Text = "Cycling" }

            }; 

        }
        public async Task<IActionResult> OnPostAsync()
        {
           
            await DAL.ActivityAPIManager.SaveActivity(Activity);

            return RedirectToPage("./Index");

        }
    }
}
