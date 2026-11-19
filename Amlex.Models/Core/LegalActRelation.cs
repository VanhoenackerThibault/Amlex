using Amlex.Models.Enums;
using Amlex.Models.Models.LegalActs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlex.Models.Core
{
    public class LegalActRelation
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid SourceLegalActId { get; set; }

        public LegalAct SourceLegalAct { get; set; } = null!;

        public LegalActRelationType RelationType { get; set; }

        public string TargetCelex { get; set; } = string.Empty;

        public Guid? TargetLegalActId { get; set; }

        public LegalAct? TargetLegalAct { get; set; }
    }
}
