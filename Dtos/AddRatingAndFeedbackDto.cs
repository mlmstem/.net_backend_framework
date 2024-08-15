namespace API.Dtos
{
    public class AddRatingAndFeedbackDto
    {
        public float Rating { get; set; }
        public string Feedback { get; set; } = string.Empty;
    }
}
