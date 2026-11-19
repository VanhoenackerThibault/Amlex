using Amlex.Interfaces.Services;
using Amlex.Models.Core;
using Amlex.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlex.Services.Database
{
    public class LegalActTopicService(AmlexDbContext dbContext) : ILegalActTopicService
    {
        public async Task CreateLegalActTopic(List<LegalActTopic> legalActTopics)
        {
            foreach (LegalActTopic item in legalActTopics)
            {
                LegalActTopic? legalActTopic = await dbContext.LegalActTopics.FirstOrDefaultAsync(l => l.EuroVocCode == item.EuroVocCode && l.LegalActId == item.LegalActId);

                if (legalActTopic is null)
                {
                    await dbContext.AddAsync(item);
                }
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
