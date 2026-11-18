using Amlex.Models.Core;
using Amlex.Models.Models.LegalActs;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Amlex.Repository
{
    public class AmlexDbContext(DbContextOptions<AmlexDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<LegalAct> LegalActs { get; set; }
        public DbSet<LegalActsFetchHistory> LegalActsFetchHistory { get; set; }
    }
}
