using Microsoft.AspNetCore.Identity;

namespace UniqloMVC.Models
{
    public class User:IdentityUser
    {
        public string Fullname { get; set; } = null!;
        public string ProfileImageUrl { get; set; } = null!;
        public ICollection<ProductRating>? Ratings { get; set; }
        public ICollection<ProductComment>? Comments { get; set; }
    }
}
