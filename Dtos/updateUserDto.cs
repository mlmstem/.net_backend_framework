using API.Dtos;

public class UpdateUserDetailDto{

        public string UserId { get; set; } = string.Empty;

        public string? FullName {get; set;}
        public string? Email {get; set;}
        public string[]? Roles{get; set;}
        public string? PhoneNumber {get; set;}
}