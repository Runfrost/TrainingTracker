using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrainingTracker.DAL;
using TrainingTrackerAPI.DTO;
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

        public UserSettingsDto UserSettings { get; set; }
        public async Task OnGetAsync()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                UserSettings = new UserSettingsDto
                {
                    Name = user?.Name,
                    YearOfBirth = user?.YearOfBirth,
                    Weight = user?.Weight,
                    Height = user?.Height,
                };
            }
            else
            {
                UserSettings = null;
            }
        }
    }
}
