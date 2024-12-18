using UniqloMVC.Models;

namespace UniqloMVC.Helpers
{
    public class ConstantRoles
    {
        const string DefaultRole = "Admin,Moderator";
        public const string Product = DefaultRole;
        public const string Slider = DefaultRole;
        public const string Category = DefaultRole;
        public const string Dashboard = DefaultRole;
        public const string User = "Admin";
    }
}
