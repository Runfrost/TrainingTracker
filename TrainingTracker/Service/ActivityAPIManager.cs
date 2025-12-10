using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json;
using TrainingTracker.ViewModel;


namespace TrainingTracker.Service
{
    public class ActivityAPIManager
    {
        private readonly HttpClient _http;
        public ActivityAPIManager(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("Backend");
        }
        public async Task<ActivityViewModel> UploadActivity(SessionInfo activity)
        {
            var response = await _http.PostAsJsonAsync("/api/Activities/Upload", activity);
            if (response.IsSuccessStatusCode)
            {
                var createdActivity = response.Content.ReadFromJsonAsync<ActivityViewModel>().Result;
                return createdActivity ?? new ActivityViewModel();
            }
            else
            {
                return new ActivityViewModel();
            }
        }

        public async Task<ActivityViewModel> SaveActivity(ActivityViewModel activity)
        {
            var response = await _http.PostAsJsonAsync("/api/Activities", activity);
            if(response.IsSuccessStatusCode)
            {
                var createdActivity = response.Content.ReadFromJsonAsync<ActivityViewModel>().Result;
                return createdActivity ?? new ActivityViewModel();
            }
            else
            {
                return new ActivityViewModel();
            }
        }

        public async Task<List<ActivityViewModel>> GetAllActivities(string userid)
        {
            try
            {
                var activities = await _http.GetFromJsonAsync<List<ActivityViewModel>>($"/api/Activities?userId={userid}");
                return activities ?? new List<ActivityViewModel>();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<ActivityViewModel>();
            }

        }
        public async Task<ActivityViewModel> GetActivity(int id)
        {
            try
            {
                var activity = await _http.GetFromJsonAsync<ActivityViewModel>($"/api/Activities/{id}");
                return activity ?? new ActivityViewModel();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new ActivityViewModel();
            }
            
        }

        public async Task<bool> UpdateActivity(ActivityViewModel activity, int id)
        {
            var response = await _http.PutAsJsonAsync($"/api/Activities/{id}", activity);
            return response.IsSuccessStatusCode;
        }

        public  async Task DeleteActivity(int id)
        {
            var response = await _http.DeleteAsync($"api/activities/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
