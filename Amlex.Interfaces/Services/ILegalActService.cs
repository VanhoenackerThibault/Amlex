using Amlex.Models.Models.LegalActs;
using Amlex.Models.Models.SparqlModels;

namespace Amlex.Interfaces.Services
{
    public interface ILegalActService
    {
        public Task CreateLegalActsAsync(List<SparqlBinding> bindings);

        public Task<List<LegalAct>> GetLegalActsAsync(int take = int.MaxValue);
    }
}
