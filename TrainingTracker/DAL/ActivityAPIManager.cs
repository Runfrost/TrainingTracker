using System.Text.Json;

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
    }
}
