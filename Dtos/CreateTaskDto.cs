using System.ComponentModel.DataAnnotations;

namespace API.Dtos
{
    public class CreateTaskDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        public int ExpectedHours { get; set; }
        
        public DateTime Deadline { get; set; }
        
        public List<string>? Roles { get; set; } = new List<string>();
        
        [Required]
        public List<string> RelevantEmployees { get; set; } = new List<string>();
        
        [Required]
        public List<string>? RelevantEmployeeEmails { get; set; } = new List<string>();


        public bool HighPriority{get; set;}
    }
}

