namespace API.Models
{
    public class TaskUser
    {
        public int TaskId { get; set; }
        public task Task { get; set; } = null!;

        public string UserId { get; set; }
        public AppUser User { get; set; } = null!;
    }
}