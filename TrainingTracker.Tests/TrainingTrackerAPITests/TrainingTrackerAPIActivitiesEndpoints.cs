using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;
using System.Net.Http.Json;
using TrainingTrackerAPI.DTO;
using TrainingTrackerAPI.Models;

namespace TrainingTracker.Tests.TrainingTrackerAPITests
{
    public class TrainingTrackerAPIActivitiesEndpoints : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _httpClient;
        public TrainingTrackerAPIActivitiesEndpoints(CustomWebApplicationFactory<Program> factory)
        {
            _httpClient = factory.CreateClient();
        }
        [Fact]
        public async Task CreateRunningActivity_ShouldReturnOkAndActivity()
        {
            //Arrange
            var typeOfActivity = SportType.Running;
            var requestUri = "/api/activities";
            ActivitiesDTO activity = new ActivitiesDTO
            {
                Name = "Night run",
                Distance = 5,
                ActivityDate = DateTime.Now,
                SportType = typeOfActivity,
            };

            //Act
            var response = await _httpClient.PostAsJsonAsync(requestUri, activity);
            var created = await response.Content.ReadFromJsonAsync<ActivitiesDTO>();

            //Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(created?.Name, activity.Name);
        }

        [Fact]
        public async Task EditActivity_ShouldReturnOkAndEditedActivity()
        {
            //Arrange
            double expectedDistance = 10;
            var typeOfActivity = SportType.Running;
            var requestUri = "/api/activities";
            ActivitiesDTO activity = new ActivitiesDTO
            {
                Name = "Night run",
                Distance = 5,
                ActivityDate = DateTime.Now,
                SportType = typeOfActivity,
            };

            //Act
            var response = await _httpClient.PostAsJsonAsync(requestUri, activity);
            var created = await response.Content.ReadFromJsonAsync<ActivitiesDTO>();

            created.Distance = expectedDistance; // Edit distance

            var editResponse = await _httpClient.PutAsJsonAsync($"{requestUri}/{created.Id}", created);
            var edited = await editResponse.Content.ReadFromJsonAsync<ActivitiesDTO>();

            //Assert
            Assert.Equal(HttpStatusCode.OK, editResponse.StatusCode);
            editResponse.EnsureSuccessStatusCode();
            Assert.Equal(edited?.Distance, expectedDistance);
        }

        [Fact]
        public async Task DeleteActivity_ShouldReturnOk()
        {
            //Arrange
            var typeOfActivity = SportType.Running;
            var requestUri = "/api/activities";
            ActivitiesDTO activity = new ActivitiesDTO
            {
                Name = "Night run",
                Distance = 5,
                ActivityDate = DateTime.Now,
                SportType = typeOfActivity,
            };

            //Act
            var response = await _httpClient.PostAsJsonAsync(requestUri, activity);
            var created = await response.Content.ReadFromJsonAsync<ActivitiesDTO>();

            var deleteResponse = await _httpClient.DeleteAsync($"{requestUri}/{created.Id}");

            //Assert
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
            deleteResponse.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetActivity_ShouldReturnActivityAndOk()
        {
            //Arrange
            var typeOfActivity = SportType.Running;
            var requestUri = "/api/activities";
            ActivitiesDTO activity = new ActivitiesDTO
            {
                Name = "Night run",
                Distance = 5,
                ActivityDate = DateTime.Now,
                SportType = typeOfActivity,
            };

            //Act
            var response = await _httpClient.PostAsJsonAsync(requestUri, activity);
            var created = await response.Content.ReadFromJsonAsync<ActivitiesDTO>();

            var getResponse = await _httpClient.GetFromJsonAsync<ActivitiesDTO>($"{requestUri}/{created?.Id}");

            //Assert
            Assert.Equal(created?.Id, getResponse?.Id);
        }

        [Theory]
        [InlineData(SportType.Running)]
        [InlineData(SportType.Cycling)]
        [InlineData(SportType.Walking)]
        public async Task CreateValidActivity_ShouldReturnOkAndActivity(SportType typeOfActivity)
        {
            //Arrange
            var requestUri = "/api/activities";
            ActivitiesDTO activity = new()
            {
                Name = "Run",
                Distance = 5,
                ActivityDate = DateTime.Now,
                SportType = typeOfActivity
            };

            //Act
            var response = await _httpClient.PostAsJsonAsync(requestUri, activity);
            var createdActivity = response.Content.ReadFromJsonAsync<ActivitiesDTO>();

            //Assert
            response.EnsureSuccessStatusCode();
            Assert.True(createdActivity != null);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData(SportType.Running)]
        [InlineData(SportType.Walking)]
        [InlineData(SportType.Cycling)]
        public async Task UploadActivity_ShouldReturnOk(SportType typeOfActivity)
        {
            //Arrange
            var requestUri = "/api/activities/Upload";
            SessionInfo activity = new()
            {
                ActivityName = "Name",
                TotalDistance = 5,
                StartTime = new(),
                Sport = (int)typeOfActivity,
                TotalTimerTime = 5,
                TotalCalories = 100,
            };


            //Act
            var response = await _httpClient.PostAsJsonAsync(requestUri, activity);
            var createdActivity = await response.Content.ReadFromJsonAsync<ActivitiesDTO>();

            //Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
