using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class task
    {
        public int Id { get; set; }
        
        public string Title { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        public bool IsCompleted { get; set; } = false;
        
        public int ExpectedHours { get; set; }
        
        public DateTime Deadline { get; set; }
        
        public DateTime? CompletionDate { get; set; }
        
        public List<string>? Roles { get; set; } = new List<string>();
        
   
        public List<string>? RelevantEmployees { get; set; } = new List<string>();
        
        public List<string>? RelevantEmployeeEmails { get; set; } = new List<string>();

        public ICollection<TaskUser>? TaskUsers { get; set; }

        public bool HighPriority {get; set;} = false;

        public float? Rating{get; set;}

        public string? Feedback{get; set;}= string.Empty;


    }
}
