using Microsoft.AspNetCore.Identity;

namespace datingapp.api.Models
{
    public class UserRole: IdentityUserRole<int>
    {
        public virtual User Users { get; set; }
        public virtual Role Roles { get; set; }
    }
}