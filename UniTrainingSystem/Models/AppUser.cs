using Microsoft.AspNetCore.Identity;

namespace UniTrainingSystem.Models
{
    public class AppUser:IdentityUser
    {
        public string FullName { get; set; }
    }
}
