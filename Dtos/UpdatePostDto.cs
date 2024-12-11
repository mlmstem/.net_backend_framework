namespace API.Dtos
{
    public class UpdatePostDto
    {
        public List<UpdateCommentDto> Comments { get; set; } = new List<UpdateCommentDto>(); // List of comments to update
    }

    public class UpdateCommentDto
    {
        public string content { get; set; } = string.Empty; // Updated comment content
        public string date { get; set; } = string.Empty;   // Date of the updated comment
    }
}