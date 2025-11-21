using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingTrackerAPI.Data;
using TrainingTrackerAPI.DTO;
using TrainingTrackerAPI.Models;

namespace TrainingTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
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
            var running = new Running
            {
                Name = newActivity.Name,
                Distance = newActivity.Distance,
                ActivityDate = DateTime.UtcNow,
                TotalTimeInSeconds = 0,
                AverageCadence = 0
            };
            _context.Activities.Add(running);
            await _context.SaveChangesAsync();

            return Ok(running);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditActivityById(int id, ActivitiesEditDto editDto)
        {
            var activityToEdit = (Running)_context.Activities.Where(a => a.Id == id).SingleOrDefaultAsync().Result;
            if(activityToEdit == null)
            {
                return NotFound();
            }

            activityToEdit.Name = editDto.Name;
            activityToEdit.Distance = editDto.Distance;
            activityToEdit.ActivityDate = editDto.ActivityDate;
            activityToEdit.TotalTimeInSeconds = editDto.TotalTimeInSeconds;
            activityToEdit.AverageCadence = editDto.AverageCadence;

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActivities()
        {
            var activities = await _context.Activities.ToListAsync();
            return Ok(activities);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetActivityById(int id)
        {
            var activity = await _context.Activities.FindAsync(id);
            return Ok(activity);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivityById(int id)
        {
            var activityToDelete = await _context.Activities.FindAsync(id);
            _context.Activities.Remove(activityToDelete);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
