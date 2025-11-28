

using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;
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
            var userId = "test-user-id";
            var requestUri = $"/api/activities?userId={userId}";

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
        [InlineData("Cycling")]
        [InlineData("Walking")]
        public async Task CreateValidActivity_ShouldReturnOkAndActivity(string typeOfActivity)
        {
            //Arrange
            var requestUri = "/api/activities";
            ActivitesCreateDto activity = new()
            {
                Name = "Run",
                Distance = 5,
                ActivityDate = DateTime.Now,
                Type = typeOfActivity
            };


            //Act
            var response = await _httpClient.PostAsJsonAsync(requestUri, activity);
            var createdActivity = response.Content.ReadFromJsonAsync<ActivitesCreateDto>();

            //Assert
            response.EnsureSuccessStatusCode();
            Assert.True(createdActivity != null);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("Swimming")]
        [InlineData("Skiing")]
        [InlineData("Other acitivity")]
        public async Task CreateInvalidActivity_ShouldReturnBadRequest(string typeOfActivity)
        {
            //Arrange
            var requestUri = "/api/activities";
            ActivitesCreateDto activity = new()
            {
                Name = "Name",
                Distance = 5,
                ActivityDate = DateTime.Now,
                Type = typeOfActivity
            };


            //Act
            var response = await _httpClient.PostAsJsonAsync(requestUri, activity);
            var createdActivity = response.Content.ReadFromJsonAsync<ActivitesCreateDto>();

            //Assert
            //Assert.True(createdActivity.Id == null);
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
