using Amlex.Interfaces.Services;
using Amlex.Models.Mappers;
using Amlex.Models.Models.LegalActs;
using Amlex.Models.Models.SparqlModels;
using Amlex.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlex.Services.Database
{
    public class LegalActService(AmlexDbContext dbContext) : ILegalActService
    {
        public async Task CreateLegalActsAsync(List<SparqlBinding> bindings)
        {
            List<LegalAct> legalActs = bindings.ToEntityList();

            foreach (LegalAct item in legalActs)
            {
                var existing = await dbContext.LegalActs.FirstOrDefaultAsync(x => x.Celex == item.Celex);

                if (existing is null)
                {
                    dbContext.LegalActs.Add(item);
                }
                else if (item.LastModifiedAt > existing.LastModifiedAt)
                {
                    // update fields
                    existing.TitleNl = item.TitleNl;
                    existing.TitleEn = item.TitleEn;
                    existing.TitleFr = item.TitleFr;

                    existing.LastModifiedAt = item.LastModifiedAt;
                    existing.UpdatedAt = DateTime.UtcNow;
                }
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task<List<LegalAct>> GetLegalActsAsync(int take = int.MaxValue)
        {
            return await dbContext.LegalActs.AsNoTracking().OrderByDescending(a => a.LastModifiedAt).Take(take).ToListAsync();
        }

    }
}
