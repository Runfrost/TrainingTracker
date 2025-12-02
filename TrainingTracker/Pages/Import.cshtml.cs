using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dynastream.Fit;
using System.Text.Json;
using TrainingTracker.FitConversion;

namespace TrainingTracker.Pages
{
    public class ImportModel : PageModel
    {
        [BindProperty]
        public IFormFile? FitFile { get; set; }

        public string? JsonOutput { get; set; }

        public SessionInfo? Session { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (FitFile == null)
            {
                JsonOutput = "No file uploaded.";
                return Page();
            }

            using var stream = FitFile.OpenReadStream();

            JsonOutput = FitToJson.ExtractSessionToJson(stream);

            // JSON is an array ? so deserialize a list
            var list = JsonSerializer.Deserialize<List<SessionInfo>>(JsonOutput);

            Session = list?.FirstOrDefault();

            return Page();
        }
    }
}
