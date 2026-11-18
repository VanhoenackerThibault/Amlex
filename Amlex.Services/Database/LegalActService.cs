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
            List<LegalAct> acts = await GetLegalActsAsync();

            List<SparqlBinding> sparqlBindings = new List<SparqlBinding>();

            foreach (SparqlBinding item in bindings)
            {
                bool found = false;

                foreach (LegalAct legalAct in acts)
                {
                    if (legalAct.Celex == item.Celex?.Value)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    sparqlBindings.Add(item);
                }
            }

            await dbContext.LegalActs.AddRangeAsync(sparqlBindings.ToEntity());

            await dbContext.SaveChangesAsync();
        }

        public async Task<List<LegalAct>> GetLegalActsAsync()
        {
            return await dbContext.LegalActs.AsNoTracking().ToListAsync();
        }

    }
}
