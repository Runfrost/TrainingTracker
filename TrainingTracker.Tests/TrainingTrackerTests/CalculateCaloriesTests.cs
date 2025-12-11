using Microsoft.AspNetCore.Routing.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingTracker.Service;
using TrainingTracker.Shared.Enums;
using TrainingTracker.ViewModel;
using TrainingTrackerAPI.Models;

namespace TrainingTracker.Tests.TrainingTrackerTests
{
    public class CalculateCaloriesTests
    {
        [Theory]
        [InlineData(70, 3600, SportType.Running, 560)]
        [InlineData(80, 1800, SportType.Cycling, 400)]
        [InlineData(60, 2700, SportType.Walking, 157.8)]
        public void CalculateCalories_ValidActivity_ReturnsExpectedCalories(double weight, double duration, SportType activity, double expected)
        {
            // Arrange
            var calorieService = new CalorieService();
       

            // Act
            double calories = CalorieService.CalculateCalories(weight, duration, activity);

            // Assert
            Assert.Equal(expected, calories, 0);
        }

        

        [Theory]
        [InlineData(-70, 3600, SportType.Running)]
        [InlineData(70, -3600, SportType.Cycling)]
        [InlineData(251, 3600, SportType.Walking)]
        [InlineData(29, 3600, SportType.Generic)]
        [InlineData(70, 0, SportType.Running)]
        [InlineData(70, 86400, SportType.Cycling)]

        public void CalculateCalories_WithInvalidData_ReturnsZero(double weight, double duration, SportType activity)
        {
            var calorieService = new CalorieService();

            double calories = CalorieService.CalculateCalories(weight, duration, activity);

            Assert.Equal(0, calories);
        } 
    }
}
