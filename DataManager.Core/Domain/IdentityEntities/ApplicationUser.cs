using Microsoft.AspNetCore.Identity;

namespace DataManager.Core.Domain.IdentityEntities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
       public string? Name { get; set; }
    }
}
