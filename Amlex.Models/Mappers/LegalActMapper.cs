using Amlex.Models.Models.LegalActs;
using Amlex.Models.Models.SparqlModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Amlex.Models.Mappers
{
    public static class LegalActMapper
    {
        public static List<LegalAct> ToEntity(this List<SparqlBinding> bindings)
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

            List<LegalAct> legalActs = new List<LegalAct>();

            const string languageUrlBase = "http://publications.europa.eu/resource/authority/language/";

            foreach (List<SparqlBinding> bindingsForCelex in groupedBindings.Values)
            {
                SparqlBinding firstBinding = bindingsForCelex.First();

                string? nlTitle = null;
                string? frTitle = null;
                string? enTitle = null;

                string? celex = firstBinding.Celex?.Value;
                string? cellarId = firstBinding.Work?.Value;
                string? docDate = firstBinding.DocumentDate?.Value;
                string? lastModifiedAt = firstBinding.LastModified?.Value;
                bool? inForce = null;

                if(firstBinding.InForce?.Value == "1")
                {
                    inForce = true;
                }

                if (firstBinding.InForce?.Value == "0")
                {
                    inForce = false;
                }

                foreach (SparqlBinding item in bindingsForCelex)
                {
                    if (languageUrlBase + "ENG" == item.Language?.Value)
                    {
                        enTitle = item.Title?.Value;
                    }

                    if (languageUrlBase + "NLD" == item.Language?.Value)
                    {
                        nlTitle = item.Title?.Value;
                    }

                    if (languageUrlBase + "FRA" == item.Language?.Value)
                    {
                        frTitle = item.Title?.Value;
                    }
                }

                if (celex is null || cellarId is null || docDate is null)
                {
                    continue;
                }

                LegalAct legalAct = new LegalAct
                {
                    CellarId = cellarId,
                    Celex = celex,
                    TitleNl = nlTitle,
                    TitleEn = enTitle,
                    TitleFr = frTitle,
                    InForce = inForce,
                    DocumentDate = DateTime.Parse(docDate),
                    LastModifiedAt = DateTime.Parse(lastModifiedAt ?? DateTime.UtcNow.ToString("O")),
                };

                legalActs.Add(legalAct);
            }

            return legalActs;
        }
    }
}
