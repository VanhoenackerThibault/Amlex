using Amlex.Models.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlex.Interfaces.Services
{
    public interface ILegalActTopicService
    {
        public Task CreateLegalActTopic(List<LegalActTopic> legalActTopics);
    }
}
