using System.Text.Json;
using TrainingTrackerAPI.Models;

namespace TrainingTracker.DAL
{
    public class ActivityAPIManager
    {
        private static Uri BaseAddress = new Uri("https://localhost:7101");

        public static async Task SaveActivity(TrainingTrackerAPI.DTO.ActivitesCreateDto activity)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = BaseAddress;
                var json = JsonSerializer.Serialize(activity);

                StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("api/Activities/CreateRunning", httpContent);

            }
        }

        public static async Task<List<ActivityDto>> GetAllActivities()

        {

            List<ActivityDto> activities = new();

            using (var client = new HttpClient())

            {

                client.BaseAddress = BaseAddress;

                HttpResponseMessage response = await client.GetAsync("/api/Activities/GetAllActivities");

                if (response.IsSuccessStatusCode)

                {

                    string responseString = await response.Content.ReadAsStringAsync();

                    //activities = JsonSerializer.Deserialize<List<ActivityDto>>(responseString);
                    activities = JsonSerializer.Deserialize<List<ActivityDto>>(
                        responseString,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        }
                    );

                }

            }

            return activities;

        }
        public static async Task DeleteActivity(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = BaseAddress;

                HttpResponseMessage response = await client.DeleteAsync($"api/activities/DeleteActivityById/{id}");

            }
        }
    }
}
