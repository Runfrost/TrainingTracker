using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        [BindProperty]
        public UserSettingsDto UserSettings { get; set; }
        public List<SelectListItem> YearOptions { get; set; }
        public List<SelectListItem> GenderOption { get; set; }

        public void BuildYearOptions()
        {
            var currentYear = DateTime.UtcNow.Year;
            YearOptions = new List<SelectListItem>();

            for (int year = currentYear; year >= (currentYear - 90); year--)
            {
                YearOptions.Add(new SelectListItem
                {
                    Value = year.ToString(),
                    Text = year.ToString()
                });
            }

        }
        public void BuildGenderOption()
        {
            GenderOption = new List<SelectListItem>();
            foreach (GenderType gender in Enum.GetValues(typeof(GenderType)))
            {
                GenderOption.Add(new SelectListItem
                {
                    Value = gender.ToString(),
                    Text = gender.ToString()
                });
            }
        }
        public async Task OnGetAsync()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                BuildYearOptions();
                BuildGenderOption();
                var user = await _userManager.GetUserAsync(User);
                UserSettings = new UserSettingsDto
                {
                    Name = user?.Name,
                    YearOfBirth = user?.YearOfBirth,
                    Weight = user?.Weight,
                    Height = user?.Height,
                    gender = user?.Gender,
                };
            }
            else
            {
                UserSettings = null;
            }




        }

        public async Task<IActionResult> OnPostAsync(UserSettingsDto userSettings)
        {

            if (!ModelState.IsValid)
            {
                BuildYearOptions();
                BuildGenderOption();
                return Page();
            }
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    user.Name = userSettings.Name;
                    user.YearOfBirth = userSettings.YearOfBirth;
                    user.Weight = userSettings.Weight;
                    user.Height = userSettings.Height;
                    user.Gender = userSettings.gender;
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToPage();
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            return Page();
        }


    }
}
