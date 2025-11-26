

using System.Net.Http.Json;
using TrainingTrackerAPI.DTO;
using TrainingTrackerAPI.Models;

namespace TrainingTracker.Tests
{
    public class TrainingTrackerApiHealthCheckerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _httpClient;
        public TrainingTrackerApiHealthCheckerTests(CustomWebApplicationFactory<Program> factory)
        {
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task HealthCheck_ReturnSuccess_AndJSON()
        {
            //Arrange
            var requestUri = "/api/activities";

            //Act
            var response = await _httpClient.GetAsync(requestUri);

            //Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
            Assert.True(response.Content.Headers.ContentLength > 0);

        }
        [Fact]
        public async Task CreateActivity_ShouldReturnOkAndActivity()
        {
            //Arrange
            var requestUri = "/api/activities";
            ActivitesCreateDto activity = new ActivitesCreateDto
            {
                Name = "Night run",
                Distance = 5,
                ActivityDate = DateTime.Now,
                Type = "Running"
            };

            //Act
            var response = await _httpClient.PostAsJsonAsync(requestUri, activity);
            var created = await response.Content.ReadFromJsonAsync<ActivitesCreateDto>();

            //Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(created.Name, activity.Name);

            
        }

        [Theory]
        [InlineData("Running")]
        [InlineData("Swim")]
        public async Task CreateRunningActivity_ShouldReturnRunning(string typeOfActivity)
        {
            //Arrange
            var activity = new Activity();
            var requestUri = "/api/activities";

            if(typeOfActivity == "Running")
            {
                activity = new Running
                {
                    Name = "Run",
                    Distance = 5,
                    ActivityDate = DateTime.Now,
                };
            }

            //Act
            var response = await _httpClient.PostAsJsonAsync(requestUri, activity);
            var createdActivity = response.Content.ReadFromJsonAsync<Activity>();

            //Assert
            response.EnsureSuccessStatusCode();
        }
    }
}
