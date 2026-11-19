using Amlex.Models.Models.LegalActs;
using Amlex.Models.Models.SparqlModels;

namespace Amlex.Models.Mappers
{
    public static class LegalActMapper
    {
        private static readonly Dictionary<string, string> LanguageMap = new()
        {
            ["http://publications.europa.eu/resource/authority/language/NLD"] = "nld",
            ["http://publications.europa.eu/resource/authority/language/ENG"] = "eng",
            ["http://publications.europa.eu/resource/authority/language/FRA"] = "fra"
        };

        public static List<LegalAct> ToEntityList(this List<SparqlBinding> bindings)
        {

            Dictionary<string, List<SparqlBinding>> groupedBindings = bindings.ToGroups();

            List<LegalAct> legalActs = new List<LegalAct>();

            foreach (List<SparqlBinding> bindingsForCelex in groupedBindings.Values)
            {
                SparqlBinding firstBinding = bindingsForCelex.First();

                string? celex = firstBinding.Celex?.Value;
                string? cellarId = firstBinding.Work?.Value;
                string? docDate = firstBinding.DocumentDate?.Value;
                string? lastModifiedAt = firstBinding.LastModified?.Value;

                bool? inForce = firstBinding.InForce?.Value switch
                {
                    "1" => true,
                    "0" => false,
                    _ => null
                };

                Dictionary<string, string?> titles = bindingsForCelex.GetTitles();

                if (celex is null || cellarId is null)
                {
                    continue;
                }

                LegalAct legalAct = new LegalAct
                {
                    CellarId = cellarId,
                    Celex = celex,
                    TitleNl = titles["nld"],
                    TitleEn = titles["eng"],
                    TitleFr = titles["fra"],
                    InForce = inForce,

                    DocumentDate = firstBinding.DocumentDate?.Value.ToDateTime(),
                    EntryIntoForce = firstBinding.EntryIntoForce?.Value.ToDateTime(),
                    Deadline = firstBinding.Deadline?.Value.ToDateTime(),
                    EndOfValidity = firstBinding.EndOfValidity?.Value.ToDateTime(),
                    LastModifiedAt = firstBinding.LastModified?.Value.ToDateTime() ?? DateTime.UtcNow,

                    ResourceType = GetResourceTypeCode(firstBinding.ResourceType?.Value),

                    //// These two are the most important missing pieces
                    //EuroVocUris = bindingsForCelex.GetEuroVocUris(),
                    //Amends = bindingsForCelex.GetRelationValues(b => b.Amends),
                    //// Repeals / ProposesToAmend the same way

                    //Repeals = bindingsForCelex.GetRelationValues(b => b.Repeals),
                    //ProposesToAmend = bindingsForCelex.GetRelationValues(b => b.ProposesToAmend),
                };

                legalActs.Add(legalAct);
            }

            return legalActs;
        }

        private static Dictionary<string, List<SparqlBinding>> ToGroups(this List<SparqlBinding> bindings)
        {
            Dictionary<string, List<SparqlBinding>> groupedBindings = new Dictionary<string, List<SparqlBinding>>();

            foreach (SparqlBinding item in bindings)
            {
                if (item.Celex?.Value is null)
                {
                    continue;
                }

                if (!groupedBindings.TryGetValue(item.Celex.Value, out var bindingsForCelex))
                {
                    bindingsForCelex = new List<SparqlBinding>();
                    groupedBindings[item.Celex.Value] = bindingsForCelex;
                }

                bindingsForCelex.Add(item);
            }

            return groupedBindings;
        }

        private static Dictionary<string, string?> GetTitles(this List<SparqlBinding> bindings)
        {
            Dictionary<string, string?> result = new Dictionary<string, string?>
            {
                ["nld"] = null,
                ["eng"] = null,
                ["fra"] = null
            };

            foreach (var item in bindings)
            {
                if (item.Language?.Value is null || item.Title?.Value is null)
                    continue;

                if (LanguageMap.TryGetValue(item.Language.Value, out var code) && result[code] is null)
                {
                    result[code] = item.Title.Value;
                }
            }

            return result;
        }

        private static DateTime? ToDateTime(this string? date)
        {
            if (string.IsNullOrEmpty(date) || string.IsNullOrWhiteSpace(date))
            {
                return null;
            }

            return DateTime.Parse(date);
        }

        private static List<string> GetEuroVocUris(this List<SparqlBinding> bindings)
        {
            return bindings
                .Select(b => b.EuroVoc?.Value)
                .Where(v => !string.IsNullOrEmpty(v))
                .Distinct()
                .Cast<string>()
                .ToList();
        }

        private static List<string> GetRelationValues(this List<SparqlBinding> bindings, Func<SparqlBinding, SparqlValue?> selector)
        {
            return bindings
                .Select(selector)
                .Select(v => v?.Value)
                .Where(v => !string.IsNullOrEmpty(v))
                .Distinct()
                .Cast<string>()
                .ToList();
        }

        private static string? GetResourceTypeCode(string? type)
        {
            if (string.IsNullOrEmpty(type) || string.IsNullOrWhiteSpace(type))
            {
                return null;
            }

            int lastSlashIndex = type.LastIndexOf('/');

            // No slash or trailing slash → return original
            if (lastSlashIndex < 0 || lastSlashIndex == type.Length - 1)
            {
                return type;
            }

            return type.Substring(lastSlashIndex + 1);

        }
    }
}
