namespace API.Dtos
{
  public class CreatePostDto
    {
        public string Title { get; set; } = string.Empty; // Title of the post
        public string Author { get; set; } = string.Empty; // Author's name
        public string Category { get; set; } = string.Empty; // Category of the post
        public string Content { get; set; } = string.Empty; // Content of the post
    }
}
