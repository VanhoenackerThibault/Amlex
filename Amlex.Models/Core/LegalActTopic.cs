using Amlex.Models.Models.LegalActs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlex.Models.Core
{
    public class LegalActTopic
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid LegalActId { get; set; }
        public LegalAct LegalAct { get; set; } = null!;

        public string EuroVocUri { get; set; } = string.Empty;   // full URI
        public string? EuroVocCode { get; set; }                 // e.g. "6569" or "c_60d786af"
    }
}
