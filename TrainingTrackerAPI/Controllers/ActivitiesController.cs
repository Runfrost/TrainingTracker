using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TrainingTrackerAPI.Data;
using TrainingTrackerAPI.DTO;
using TrainingTrackerAPI.Models;

namespace TrainingTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivitiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ActivitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("Upload")]
        public async Task<IActionResult> UploadActivity([FromBody] SessionInfo sessionInfo)
        {
            string[] formats = { "ss", "mm:ss", "hh:mm:ss" };
            TimeOnly timeResult = new();
            var success = TimeOnly.TryParseExact(sessionInfo.TotalTimeHms, formats, out var result);
            if(success)
                timeResult = result;

            Activity activity = new Activity()
            {
                Name = sessionInfo.ActivityName ?? "New Activity",
                Distance = sessionInfo.DistanceKm,
                ActivityDate = sessionInfo.StartDateTime,
                TotalTimeInSeconds = (int)sessionInfo.TotalTimerTime,
                AverageHeartRate = sessionInfo.AvgHeartRate,
                TimeInput = timeResult,
                CaloriesBurned = sessionInfo.TotalCalories,
                AvgCadence = sessionInfo.AvgCadence,
                AvgPower = sessionInfo.AvgPower,
                UserId = sessionInfo.UserId,
                SportType = sessionInfo.SportEnum,
            };

            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();

            return Ok(activity);
        }

        [HttpPost]
        public async Task<IActionResult> CreateActivity([FromBody] DTO.ActivitiesDTO newActivity)
        {
            Activity activity = new()
            {
                Name = newActivity.Name,
                Distance = newActivity.Distance,
                ActivityDate = newActivity.ActivityDate,
                TotalTimeInSeconds = newActivity.TotalTimeInSeconds,
                TimeInput = newActivity.TimeInput,
                CaloriesBurned = newActivity.Calories,
                AvgCadence = newActivity.AverageCadence,
                UserId = newActivity.UserId,
                SportType = newActivity.SportType,
            };

            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();

            return Ok(activity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditActivityById(int id, ActivitiesDTO editDto)
        {
            var activityToEdit = _context.Activities.Where(a => a.Id == id).SingleOrDefaultAsync().Result;
            if (activityToEdit == null)
            {
                return NotFound();
            }

            activityToEdit.Name = editDto.Name;
            activityToEdit.Distance = editDto.Distance;
            activityToEdit.ActivityDate = editDto.ActivityDate;
            activityToEdit.TotalTimeInSeconds = editDto.TotalTimeInSeconds;
            activityToEdit.TimeInput = editDto.TimeInput;
            activityToEdit.CaloriesBurned = editDto.Calories;
            activityToEdit.SportType = editDto.SportType;

            await _context.SaveChangesAsync();
            return Ok(activityToEdit);
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAllActivitiesByUserId([FromQuery] string userId)
        {
            var activities = await _context.Activities
                .Where(a => a.UserId == userId)
                .Select(a => new ActivitiesDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                    Distance = a.Distance,
                    ActivityDate = a.ActivityDate,
                    TotalTimeInSeconds = a.TotalTimeInSeconds,
                    TimeInput = a.TimeInput,
                    Calories = a.CaloriesBurned ?? 0,
                    SportType = a.SportType,
                    AverageCadence = a.AvgCadence,

                })
                .ToListAsync();

            return Ok(activities);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetActivityById(int id)
        {
            var activity = await _context.Activities
                .Where(a => a.Id == id)
                .Select(a => new ActivitiesDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                    Distance = a.Distance,
                    ActivityDate = a.ActivityDate,
                    TotalTimeInSeconds = a.TotalTimeInSeconds,
                    TimeInput = a.TimeInput,
                    Calories = a.CaloriesBurned ?? 0,
                    SportType = a.SportType,
                    AverageCadence = a.AvgCadence,
                })
                .FirstOrDefaultAsync();

            return Ok(activity);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivityById(int id)
        {
            var activityToDelete = await _context.Activities.FindAsync(id);
            if(activityToDelete == null)
            {
                return NotFound();
            }
            _context.Activities.Remove(activityToDelete);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}



