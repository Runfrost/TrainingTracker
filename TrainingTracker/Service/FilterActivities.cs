using Microsoft.AspNetCore.Identity;
using TrainingTracker.Shared.Enums;
using TrainingTracker.ViewModel;
using TrainingTrackerAPI.Models;

namespace TrainingTracker.Service
{
    public class FilterActivities
    {
        private readonly ActivityAPIManager _api;
        public FilterActivities(ActivityAPIManager api)
        {
            _api = api;
        }
        public async Task<List<ActivityViewModel>> LoadFilteredActivitiesAsync(bool ShowCycling, bool ShowRunning, bool ShowWalking, string userId)
        {            
            var allActivities = await _api.GetAllActivities(userId);

            var filteredActivities = new List<ActivityViewModel>();

            if (ShowCycling)
                filteredActivities.AddRange(allActivities.Where(a => a.SportType == SportType.Cycling));

            if (ShowRunning)
                filteredActivities.AddRange(allActivities.Where(a => a.SportType == SportType.Running));

            if (ShowWalking)
                filteredActivities.AddRange(allActivities.Where(a => a.SportType == SportType.Walking));

            return filteredActivities;
        }
    }
}
