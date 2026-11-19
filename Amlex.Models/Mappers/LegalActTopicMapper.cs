using Amlex.Models.Core;
using Amlex.Models.Models.SparqlModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlex.Models.Mappers
{
    public static class LegalActTopicMapper
    {
        public static List<LegalActTopic> ToTopicEntities(this List<SparqlBinding> bindings, Dictionary<string, Guid> sourceLegalActIdsByCelex)
        {
            List<LegalActTopic> entities = new List<LegalActTopic>();

            foreach (SparqlBinding item in bindings)
            {
                entities.Add(item.ToTopicEntity(sourceLegalActIdsByCelex[item.Celex.Value]));
            }

            return entities;
        }


        public static LegalActTopic ToTopicEntity(this SparqlBinding binding, Guid LegalActId)
        {
            
            
            return new LegalActTopic
            {
                EuroVocUri = binding.EuroVoc.Value,
                EuroVocCode = binding.EuroVoc.Value.Substring(binding.EuroVoc.Value.LastIndexOf("/") + 1),
                LegalActId = LegalActId
            };
        }
    }
}
