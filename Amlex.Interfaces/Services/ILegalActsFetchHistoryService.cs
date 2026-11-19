using Amlex.Models.Models.LegalActs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlex.Interfaces.Services
{
    public interface ILegalActsFetchHistoryService
    {
        public Task CreateWatchItemAsync(List<LegalAct> acts);

        public Task<DateTime> GetLastUpdateTimeAsync();

        public Task<DateTime> GetLastFetchTimeAsync();
    }
}
