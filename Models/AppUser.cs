using Microsoft.AspNetCore.Identity;

namespace API.Models{


    public class AppUser : IdentityUser{
        public string? FullName {get; set;}

       public ICollection<TaskUser>? TaskUsers { get; set; }

    }
}