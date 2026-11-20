using Amlex.Interfaces.Services;
using Amlex.Models.Core;
using Amlex.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlex.Services.Database
{
    public class EuroVocCodesService(AmlexDbContext dbContext) : IEuroVocCodesService
    {
        public async Task<List<EuroVocCodes>> GetEuroVocCodesAsync()
        {
            return await dbContext.EuroVocCodes.Where(e => e.IncludeInQuery).ToListAsync();
        }
    }
}
