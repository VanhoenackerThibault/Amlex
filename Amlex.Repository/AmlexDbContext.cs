using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Amlex.Repository
{
    public class AmlexDbContext(DbContextOptions<AmlexDbContext> options) : IdentityDbContext(options)
    {
        
    }
}
