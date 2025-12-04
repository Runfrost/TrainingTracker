using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingTracker.Service;

namespace TrainingTracker.Tests
{
    public class CalculateCaloriesTests
    {
        //private readonly CalorieService _calorieService;
        //public CalculateCaloriesTests(CalorieService calorieService)
        //{
        //    _calorieService = calorieService;
        //}
        [Fact]
        public void CalculateCalories_RunningActivity_ReturnsExpectedCalories()
        {
            // Arrange
            var calorieService = new CalorieService();
            double weight = 70; // in kg
            double duration = 3600; // in seconds

            var activity = TrainingTracker.ViewModel.SportType.Running;

            // Act
            double calories = CalorieService.CalculateCalories(weight, duration, activity);

            // Assert
            double expectedCalories = 8.0 * weight * (duration / 3600.0); // MET for running is 8.0
            Assert.Equal(expectedCalories, calories);
        }

    }
}
