using Microsoft.AspNetCore.Identity;

namespace Rocky_Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
