using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainingTracker.Tests
{
    public class CalculateCaloriesTests
    {
        [Fact]
        public void CalculateCalories_RunningActivity_ReturnsExpectedCalories()
        {
            // Arrange
            var calorieService = new TrainingTracker.Service.CalorieService();
            double weight = 70; // in kg
            double duration = 3600; // in seconds

            var activity = TrainingTracker.FitConversion.FitSport.Running;

            // Act
            double calories = calorieService.CalculateCalories(weight, duration, activity);

            // Assert
            double expectedCalories = 8.0 * weight * (duration / 3600.0); // MET for running is 8.0
            Assert.Equal(expectedCalories, calories);
        }

    }
}
