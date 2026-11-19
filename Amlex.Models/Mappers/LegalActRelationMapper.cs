using Amlex.Models.Core;
using Amlex.Models.Enums;
using Amlex.Models.Models.SparqlModels;
using System;
using System.Collections.Generic;

namespace Amlex.Models.Mappers
{
    public static class LegalActRelationMapper
    {
        public static List<LegalActRelation> ToRelationEntities(this List<SparqlBinding> bindings, Dictionary<string, Guid> sourceLegalActIdsByCelex)
        {
            List<LegalActRelation> relations = new List<LegalActRelation>(bindings.Count);

            foreach (SparqlBinding binding in bindings)
            {
                string sourceCelex = binding.Celex?.Value?? throw new InvalidOperationException("The relation binding is missing its source CELEX.");

                if (!sourceLegalActIdsByCelex.TryGetValue(sourceCelex, out Guid sourceLegalActId))
                {
                    throw new InvalidOperationException($"No local legal act was found for source CELEX '{sourceCelex}'.");
                }

                relations.Add(binding.ToRelationEntity(sourceLegalActId));
            }

            return relations;
        }

        public static LegalActRelation ToRelationEntity(this SparqlBinding binding, Guid sourceLegalActId)
        {
            if (sourceLegalActId == Guid.Empty)
            {
                throw new ArgumentException("The source legal act ID must not be empty.", nameof(sourceLegalActId));
            }

            string relationValue = binding.Relation?.Value ?? throw new InvalidOperationException("The relation binding is missing its relation type.");

            string targetCelex = binding.OtherCelex?.Value ?? throw new InvalidOperationException("The relation binding is missing its target CELEX.");

            if (string.IsNullOrWhiteSpace(targetCelex))
            {
                throw new InvalidOperationException("The relation binding contains an empty target CELEX.");
            }

            LegalActRelationType relationType = relationValue switch
            {
                "amends" => LegalActRelationType.Amends,
                "repeals" => LegalActRelationType.Repeals,
                "proposes_to_amend" => LegalActRelationType.ProposesToAmend,
                _ => throw new InvalidOperationException(
                    $"Unknown CELLAR legal-act relation type: '{relationValue}'.")
            };

            return new LegalActRelation
            {
                SourceLegalActId = sourceLegalActId,
                RelationType = relationType,
                TargetCelex = targetCelex
            };
        }
    }
}