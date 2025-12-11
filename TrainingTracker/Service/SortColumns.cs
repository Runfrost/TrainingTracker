using TrainingTracker.Models;
using TrainingTracker.ViewModel;

namespace TrainingTracker.Service
{
    public class SortColumns
    {
        public static List<ActivityViewModel> ReturnSortedActivities(List<ActivityViewModel> activities, bool sortDescending, SortField sortColumn)
        {
            var sortedActivities = sortColumn switch
            {
                SortField.Type => sortDescending
                    ? activities.OrderByDescending(a => a.SportType.ToString()).ToList()
                    : activities.OrderBy(a => a.SportType.ToString()).ToList(),

                SortField.Title => sortDescending
                    ? activities.OrderByDescending(a => a.Name).ToList()
                    : activities.OrderBy(a => a.Name).ToList(),

                SortField.Date => sortDescending
                    ? activities.OrderByDescending(a => a.ActivityDate).ToList()
                    : activities.OrderBy(a => a.ActivityDate).ToList(),

                SortField.Distance => sortDescending
                    ? activities.OrderByDescending(a => a.Distance).ToList()
                    : activities.OrderBy(a => a.Distance).ToList(),

                SortField.Duration => sortDescending
                    ? activities.OrderByDescending(a => a.TotalTimeInSeconds).ToList()
                    : activities.OrderBy(a => a.TotalTimeInSeconds).ToList(),

                SortField.Calories => sortDescending
                    ? activities.OrderByDescending(a => a.Calories).ToList()
                    : activities.OrderBy(a => a.Calories).ToList(),

                _ => new List<ActivityViewModel>()
                };
            return sortedActivities;
        }
    }
}
