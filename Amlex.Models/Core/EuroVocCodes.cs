using Amlex.Models.Models.LegalActs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlex.Models.Core
{
    public class EuroVocCodes
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public required string EuroVocCode { get; set; }

        public required string EuroVocUri { get; set; }

        public required string SectionName { get; set; }

        public required bool IncludeInQuery { get; set; }
    }
}
