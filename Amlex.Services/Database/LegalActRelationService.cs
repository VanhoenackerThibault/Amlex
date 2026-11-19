using Amlex.Interfaces.Services;
using Amlex.Models.Core;
using Amlex.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlex.Services.Database
{
    public class LegalActRelationService(AmlexDbContext dbContext) : ILegalActRelationService
    {
        public async Task CreateLegalActRelationAsync(List<LegalActRelation> legalActRelations)
        {

            foreach (LegalActRelation item in legalActRelations)
            {
                LegalActRelation? relation = await dbContext.LegalActRelations.FirstOrDefaultAsync(l => l.TargetCelex == item.TargetCelex &&
                    l.SourceLegalActId == item.SourceLegalActId &&
                    l.RelationType == item.RelationType);

                if (relation is null)
                {
                    dbContext.LegalActRelations.Add(item);
                }
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
