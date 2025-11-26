using System.Text.Json;
using TrainingTrackerAPI.DTO;
using TrainingTrackerAPI.Models;

namespace TrainingTracker.DAL
{
    public class ActivityAPIManager
    {
        private readonly HttpClient _http;

        private static Uri BaseAddress = new Uri("https://localhost:7101");
        public ActivityAPIManager(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("Backend");
        }

        public static async Task SaveActivity(TrainingTrackerAPI.DTO.ActivitesCreateDto activity)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = BaseAddress;
                var json = JsonSerializer.Serialize(activity);

                StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("api/Activities", httpContent);

            }
        }

        public async Task<List<ActivityDto>> GetAllActivities()

        {
            var activities = await _http.GetFromJsonAsync<List<ActivityDto>>("/api/Activities");
            return activities;

            //List<ActivityDto> activities = new();

            //using (var client = new HttpClient())

            //{

            //    client.BaseAddress = BaseAddress;

            //    HttpResponseMessage response = await client.GetAsync("/api/Activities/GetAllActivities");
            //    var test = await client.GetFromJsonAsync<List<ActivityDto>>("/api/Activities/GetAllActivities");
            //    return test;
            //}
            //    if (response.IsSuccessStatusCode)

            //    {

            //        string responseString = await response.Content.ReadAsStringAsync();

            //        //activities = JsonSerializer.Deserialize<List<ActivityDto>>(responseString);
            //        activities = JsonSerializer.Deserialize<List<ActivityDto>>(
            //            responseString,
            //            new JsonSerializerOptions
            //            {
            //                PropertyNameCaseInsensitive = true
            //            }
            //        );

            //    }

            //}

            //return activities;

        }
        public async Task<ActivityDto> GetActivity(int id)
        { 
            var activity = await _http.GetFromJsonAsync<ActivityDto>($"/api/Activities/{id}");
            return activity;

            //ActivityDto activity = new();
            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = BaseAddress;
            //    HttpResponseMessage response = await client.GetAsync($"api/activities/GetActivityById/{id}");
            //    if (response.IsSuccessStatusCode)
            //    {
            //        string responseString = await response.Content.ReadAsStringAsync();
            //        activity = JsonSerializer.Deserialize<ActivityDto>(
            //            responseString,
            //            new JsonSerializerOptions
            //            {
            //                PropertyNameCaseInsensitive = true
            //            }
            //        );
            //    }
            //}


            //return activity;
        }

        public static async Task UpdateActivity(ActivitesCreateDto activity, int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = BaseAddress;
                var json = JsonSerializer.Serialize(activity);

                StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync($"api/activities/{id}", httpContent);

            }
        }

        public  async Task DeleteActivity(int id)
        {
            var response = await _http.DeleteAsync($"api/activities/{id}");
            response.EnsureSuccessStatusCode();
            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = BaseAddress;

            //    HttpResponseMessage response = await client.DeleteAsync($"api/activities/DeleteActivityById/{id}");

            //}
        }
    }
}
