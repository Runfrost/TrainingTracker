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
        [HttpPost]
        public async Task<IActionResult> CreateRunning([FromBody] DTO.ActivitesCreateDto newActivity)
        {
            Activity activity = new();
            //activity.UserId = newActivity.UserId;

            switch (newActivity.Type)
            {
                case "Running":
                    activity = new Running
                    {
                        Name = newActivity.Name,
                        Distance = newActivity.Distance,
                        ActivityDate = newActivity.ActivityDate,
                        TotalTimeInSeconds = 0,
                        AverageCadence = 0,
                        UserId = newActivity.UserId
                    };
                    break;

                case "Walking":
                    activity = new Walking
                    {
                        Name = newActivity.Name,
                        Distance = newActivity.Distance,
                        ActivityDate = newActivity.ActivityDate,
                        TotalTimeInSeconds = 0,
                        UserId = newActivity.UserId
                    };
                    break;

                case "Cycling":
                    activity = new Cycling
                    {
                        Name = newActivity.Name,
                        Distance = newActivity.Distance,
                        ActivityDate = newActivity.ActivityDate,
                        TotalTimeInSeconds = 0,
                        UserId = newActivity.UserId
                        // Lägg till andra properties om du har dem, t.ex. AverageSpeed
                    };
                    break;

                default:
                    return BadRequest("Unsupported activity type");
            }

            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();

            return Ok(activity);
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> EditActivityById(int id, ActivitiesEditDto editDto)
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
            //activityToEdit.AverageCadence = editDto.AverageCadence;

            await _context.SaveChangesAsync();
            return Ok();
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAllActivitiesByUserId([FromQuery] string userId)
        {
            var activities = await _context.Activities
                .Where(a => a.UserId == userId)
                .Select(a => new ActivityDto
                {
                    Id = a.Id,
                    Type = EF.Property<string>(a, "ActivityType"),
                    Name = a.Name,
                    Distance = a.Distance,
                    ActivityDate = a.ActivityDate,
                    //UserId = a.UserId,
                    TotalTimeInSeconds = a.TotalTimeInSeconds,




                    // Only for Running
                    //AverageCadence = a is Running r ? r.AverageCadence : null
                })
                .ToListAsync();

            return Ok(activities);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetActivityById(int id)
        {
            var activity = await _context.Activities
                .Where(a => a.Id == id)
                .Select(a => new ActivityDto
                {
                    Id = a.Id,
                    Type = EF.Property<string>(a, "ActivityType"),
                    Name = a.Name,
                    Distance = a.Distance,
                    ActivityDate = a.ActivityDate,
                    TotalTimeInSeconds = a.TotalTimeInSeconds,
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



