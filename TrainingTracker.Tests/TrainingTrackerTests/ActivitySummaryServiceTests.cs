using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingTracker.Service;
using TrainingTracker.Shared.Enums;
using TrainingTracker.ViewModel;
using TrainingTrackerAPI.Services;

namespace TrainingTracker.Tests.TrainingTrackerTests
{
    public class ActivitySummaryServiceTests
    {
        [Theory]
        [InlineData(25, 3000, 400)]
        public void ActivitiesSummary_ActivitiesList_ReturnsSummary(double distance, int seconds, double calories)
        {
            // Arrange
            var activitySummaryService = new ActivitySummaryService();
            var activities = new List<ActivityViewModel>()
            {
                new ActivityViewModel()
            {
                Id = 1,
                Name = "Activity 1",
                ActivityDate = DateTime.Now,
                Distance = 5,
                Calories = 150,
                TotalTimeInSeconds = 600,
                SportType = SportType.Running,
            },
            new ActivityViewModel()
            {
                Id = 2,
                Name = "Activity 2",
                ActivityDate = DateTime.Now,
                Distance = 20,
                Calories = 250,
                TotalTimeInSeconds = 2400,
                SportType = SportType.Cycling,
            }
            };


            // Act
            var result = activitySummaryService.CalculateActivityIntervals(activities);
            

            // Assert
            Assert.Equal(distance, result.TotalDistanceThisWeek, 0);
            Assert.Equal(seconds, result.TotalDurationThisWeek, 0);
            Assert.Equal(calories, result.TotalCaloriesBurntThisWeek, 0);

        }

        [Theory]
        [InlineData(10, 15, 25, 50)]
        public void ActivitiesSummary_CalculateTotalDistance_ReturnsDistance(double distance1, double distance2, double distance3, double expected)
        {
            // Arrange
            var activitySummaryService = new ActivitySummaryService();
            var activities = new List<ActivityViewModel>()
            {
                new ActivityViewModel()
            {
                Id = 1,
                Name = "Activity 1",
                ActivityDate = DateTime.Now,
                Distance = distance1,
            },
            new ActivityViewModel()
            {
                Id = 2,
                Name = "Activity 2",
                ActivityDate = DateTime.Now,
                Distance = distance2,
            },
            new ActivityViewModel()
            {
                Id = 3,
                Name = "Activity 3",
                ActivityDate = DateTime.Now,
                Distance = distance3,
            }
            };

            // Act
            var result = activitySummaryService.CalculateActivityIntervals(activities);


            // Assert
            Assert.Equal(expected, result.TotalDistanceThisWeek, 0);

        }
    }
}
