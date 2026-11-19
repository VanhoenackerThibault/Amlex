using Amlex.Models.Models.LegalActs;
using Amlex.Models.Models.SparqlModels;

namespace Amlex.Interfaces.Services
{
    public interface ILegalActService
    {
        public Task CreateLegalActsAsync(List<LegalAct> legalActs);

        public Task<List<LegalAct>> GetLegalActsAsync(int take = int.MaxValue);

        public Task<List<LegalAct>> GetLegalActsByListCelexAsync(List<string> celexes);
    }
}
