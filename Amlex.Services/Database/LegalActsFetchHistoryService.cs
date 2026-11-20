using Amlex.Interfaces.Services;
using Amlex.Models.Core;
using Amlex.Models.Models.LegalActs;
using Amlex.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlex.Services.Database
{
    public class LegalActsFetchHistoryService(AmlexDbContext dbContext) : ILegalActsFetchHistoryService
    {
        public async Task CreateWatchItemAsync(List<LegalAct> acts)
        {
            LegalActsFetchHistory legalActsFetchHistory = new LegalActsFetchHistory
            {
                FetchedAt = DateTime.Now,
                LastUpdateTime = acts.Max(a => a.LastModifiedAt)
            };

            await dbContext.LegalActsFetchHistory.AddAsync(legalActsFetchHistory);

            await dbContext.SaveChangesAsync();
        }

        public async Task<DateTime?> GetLastUpdateTimeAsync()
        {
            if (dbContext.LegalActsFetchHistory.Any())
            {
                return dbContext.LegalActsFetchHistory.Max(f => f.LastUpdateTime);
            }

            return null;
        }

        public async Task<DateTime?> GetLastFetchTimeAsync()
        {
            if (dbContext.LegalActsFetchHistory.Any())
            {
                return dbContext.LegalActsFetchHistory.Max(f => f.FetchedAt);  
            }

            return null;
        }
    }
}
