using System.ComponentModel.DataAnnotations;
using API.Models;

namespace API.Dtos
{
    public class TaskDto
    {
        public int Id { get; set; }
        
        public string Title { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        public bool IsCompleted { get; set; }
        
        public int ExpectedHours { get; set; }
        
        public DateTime Deadline { get; set; }
        
        public DateTime? CompletionDate { get; set; }
        
        public List<string>? Roles { get; set; } = new List<string>();
        
        public List<string>? RelevantEmployees { get; set; } = new List<string>();
        
        public List<string>? RelevantEmployeeEmails { get; set; } = new List<string>();


        public float? Rating{get; set;}

        public string? Feedback{get; set;}= string.Empty;


        public bool HighPriority {get; set;}

        public ICollection<TaskUser>? TaskUsers { get; set; }
    }

}

