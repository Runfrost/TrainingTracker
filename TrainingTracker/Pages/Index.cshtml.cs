using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Infrastructure;

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

        [BindProperty]
        public TrainingTrackerAPI.DTO.ActivitesCreateDto Activity { get; set; }

        public void OnGet()
        {

        }
        public async Task<IActionResult> OnPostAsync()
        {
            await DAL.ActivityAPIManager.SaveActivity(Activity);

            return RedirectToPage("./Index");

        }
    }
}
