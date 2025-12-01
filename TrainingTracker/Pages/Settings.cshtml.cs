using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrainingTracker.DAL;
using TrainingTrackerAPI.Models;

namespace TrainingTracker.Pages
{
    public class SettingsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public SettingsModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public void OnGetAsync()
        {
        }
    }
}
