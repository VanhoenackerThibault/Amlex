using Amlex.Models.Models.SparqlModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlex.Interfaces.Services
{
    public interface ILegalActService
    {
        public Task CreateLegalActsAsync(List<SparqlBinding> bindings);
    }
}
