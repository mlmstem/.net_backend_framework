using API.Data;
using API.Dtos;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    // [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public TasksController(AppDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // New endpoint: Create a new task
        [HttpPost("create")]
       public async Task<ActionResult<TaskDto>> CreateTask(CreateTaskDto createTaskDto)
        {
            var relevantEmployees = await _userManager.Users
                .Where(u => createTaskDto.RelevantEmployeeEmails.Contains(u.Email))
                .ToListAsync();

            // Verify if all relevant employees are found
            if (relevantEmployees.Count != createTaskDto.RelevantEmployeeEmails.Count)
            {
                return BadRequest("One or more specified employees were not found.");
            }

            var task = new task
            {
                Title = createTaskDto.Title,
                Description = createTaskDto.Description,
                ExpectedHours = createTaskDto.ExpectedHours,
                Deadline = createTaskDto.Deadline,
                Roles = createTaskDto.Roles,
                RelevantEmployeeEmails = createTaskDto.RelevantEmployeeEmails,
                HighPriority = createTaskDto.HighPriority

            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            foreach (var email in createTaskDto.RelevantEmployeeEmails)
            {
                var user = relevantEmployees.FirstOrDefault(u => u.Email == email);
                if (user != null)
                {
                    var taskUser = new TaskUser
                    {
                        TaskId = task.Id,
                        UserId = user.Id
                    };
                    _context.TaskUsers.Add(taskUser);
                }
            }
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                ExpectedHours = task.ExpectedHours,
                Deadline = task.Deadline,
                CompletionDate = task.CompletionDate,
                Roles = task.Roles,
                RelevantEmployees = task.RelevantEmployeeEmails,
                RelevantEmployeeEmails = task.RelevantEmployeeEmails,
                HighPriority = task.HighPriority, // Added highPriority
                Rating = task.Rating, // Added rating
                Feedback = task.Feedback // Added Feedback


            });
        }

        
       [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks()
        {
            var tasks = await _context.Tasks
                .Include(t => t.TaskUsers)
                    .ThenInclude(tu => tu.User)
                .ToListAsync();


            return Ok(tasks.Select(task => new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                ExpectedHours = task.ExpectedHours,
                Deadline = task.Deadline,
                CompletionDate = task.CompletionDate,
                Roles = task.Roles,
                RelevantEmployeeEmails = task.RelevantEmployeeEmails ?? new List<string> { "No emails found" },
                RelevantEmployees = task.TaskUsers.Select(tu => tu.User.Email).ToList(),
                HighPriority = task.HighPriority, // Added highPriority
                Rating = task.Rating, // Added rating
                Feedback = task.Feedback // Added Feedback

            }).ToList());
        }


        
       [HttpGet("{id}")]
        public async Task<ActionResult<TaskDto>> GetTask(int id)
        {
            var task = await _context.Tasks
                .Include(t => t.TaskUsers)
                    .ThenInclude(tu => tu.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null) return NotFound();

            return Ok(new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                ExpectedHours = task.ExpectedHours,
                Deadline = task.Deadline,
                CompletionDate = task.CompletionDate,
                Roles = task.Roles,
                RelevantEmployees = task.TaskUsers.Select(tu => tu.User.Email).ToList(),
                RelevantEmployeeEmails = task.RelevantEmployeeEmails,

                HighPriority = task.HighPriority, // Added highPriority
                Rating = task.Rating, // Added rating
                Feedback = task.Feedback, // Added Feedback
            });
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound("Task not found.");
            }

            task.IsCompleted = true;
            task.CompletionDate = DateTime.UtcNow;

            _context.Entry(task).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPost("{id}/rate")]
        public async Task<IActionResult> AddRatingAndFeedback(int id, [FromBody] AddRatingAndFeedbackDto ratingDto)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound("Task not found.");
            }

            task.Rating = ratingDto.Rating;
            task.Feedback = ratingDto.Feedback;

            _context.Entry(task).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }



        // Update a task
        // [HttpPut("{id}")]
        // public async Task<IActionResult> UpdateTask(int id, UpdateTaskDto updateTaskDto)
        // {
        //     var task = await _context.Tasks
        //         .Include(t => t.RelevantEmployees)
        //         .FirstOrDefaultAsync(t => t.Id == id);

        //     if (task == null) return NotFound();

        //     var relevantEmployees = await _userManager.Users
        //         .Where(u => updateTaskDto.RelevantEmployees.Contains(u.Id))
        //         .ToListAsync();

        //     task.Title = updateTaskDto.Title;
        //     task.Description = updateTaskDto.Description;
        //     task.IsCompleted = updateTaskDto.IsCompleted;
        //     task.ExpectedHours = updateTaskDto.ExpectedHours;
        //     task.Deadline = updateTaskDto.Deadline;
        //     task.CompletionDate = updateTaskDto.CompletionDate;
        //     task.Roles = updateTaskDto.Roles;
        //     task.RelevantEmployees = relevantEmployees;
        //     task.RelevantEmployeeEmails = updateTaskDto.RelevantEmployeeEmails;

        //     _context.Entry(task).State = EntityState.Modified;
        //     await _context.SaveChangesAsync();

        //     return NoContent();
        // }

        // Delete a task
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteTask(int id)
        // {
        //     var task = await _context.Tasks.FindAsync(id);
        //     if (task == null) return NotFound();

        //     _context.Tasks.Remove(task);
        //     await _context.SaveChangesAsync();

        //     return NoContent();
        // }

        // Mark task as complete
        // [HttpPatch("{id}/complete")]
        // public async Task<IActionResult> MarkTaskAsComplete(int id)
        // {
        //     var task = await _context.Tasks.FindAsync(id);
        //     if (task == null) return NotFound();

        //     task.IsCompleted = true;
        //     task.CompletionDate = DateTime.UtcNow;

        //     _context.Entry(task).State = EntityState.Modified;
        //     await _context.SaveChangesAsync();

        //     return NoContent();
        // }
    }
}
