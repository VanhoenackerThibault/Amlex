using Amlex.Models.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlex.Interfaces.Services
{
    public interface IEuroVocCodesService
    {
        public Task<List<EuroVocCodes>> GetEuroVocCodesAsync();
    }
}
