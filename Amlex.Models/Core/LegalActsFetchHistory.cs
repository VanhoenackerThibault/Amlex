using System;
using System.Collections.Generic;
using System.Text;

namespace Amlex.Models.Core
{
    public class LegalActsFetchHistory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime FetchedAt { get; set; }
        public DateTime LastUpdateTime { get; set; }
    }
}
