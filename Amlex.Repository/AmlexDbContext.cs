using Amlex.Models.Core;
using Amlex.Models.Models.LegalActs;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Amlex.Repository
{
    public class AmlexDbContext(DbContextOptions<AmlexDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<LegalAct> LegalActs { get; set; }
        public DbSet<LegalActsFetchHistory> LegalActsFetchHistory { get; set; }

        public DbSet<LegalActRelation> LegalActRelations { get; set; }

        public DbSet<LegalActTopic> LegalActTopics { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {


            builder.Entity<LegalActRelation>(legalActRelation =>
            {
                legalActRelation.HasKey(relation => relation.Id);

                legalActRelation
                    .HasOne(relation => relation.SourceLegalAct)
                    .WithMany()
                    .HasForeignKey(relation => relation.SourceLegalActId)
                    .OnDelete(DeleteBehavior.Cascade);

                legalActRelation
                    .HasOne(relation => relation.TargetLegalAct)
                    .WithMany()
                    .HasForeignKey(relation => relation.TargetLegalActId)
                    .OnDelete(DeleteBehavior.NoAction);

                legalActRelation
                    .Property(relation => relation.TargetCelex)
                    .IsRequired()
                    .HasMaxLength(450);

                legalActRelation
                    .HasIndex(relation => new
                    {
                        relation.SourceLegalActId,
                        relation.RelationType,
                        relation.TargetCelex
                    })
                    .IsUnique();
            });


            base.OnModelCreating(builder);
        }
    }
}
