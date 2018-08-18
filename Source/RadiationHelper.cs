using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    public static class RadiationHelper
    {
        public static List<MutationDef> Mutations
        {
            get
            {
                if (allMutations != null) return allMutations;

                allMutations = new List<MutationDef>();
                allMutations.AddRange(GenDefDatabase.GetAllDefsInDatabaseForDef(typeof(MutationDef)).Cast<MutationDef>());
                return allMutations;
            }
        }

        static List<MutationDef> allMutations;

        private static IEnumerable<BodyPartRecord> GetChildBodyParts(BodyPartRecord part)
        {
            if (part == null) yield break;

            yield return part;

            foreach (BodyPartRecord childPart in part.GetDirectChildParts())
            {
                foreach (var x in GetChildBodyParts(childPart))
                {
                    yield return x;
                }
            }

            yield break;
        }


        private static IEnumerable<BodyPartRecord> WhichPartsMutationIsAllowedOn(
            Pawn pawn,
            MutationDef def,
            BodyPartRecord part,
            Dictionary<string, HashSet<BodyPartDef>> excludedPartDefsForTag,
            Dictionary<string, HashSet<BodyPartRecord>> excludedPartsForTag
        )
        {
            IEnumerable<BodyPartRecord> allParts;
            if (def.affectedParts == null)
            {
                List<BodyPartRecord> list = new List<BodyPartRecord>();
                list.AddRange(GetChildBodyParts(part));
                allParts = list;
            }
            else
            {
                allParts = pawn.health.hediffSet.GetNotMissingParts().Where(x => def.affectedParts.Contains(x.def));
            }

            foreach (string tag in def.exclusives)
            {
                if (def.affectsAllParts)
                {
                    HashSet<BodyPartDef> excluded;
                    if (excludedPartDefsForTag.TryGetValue(tag, out excluded))
                    {
                        allParts = allParts.Where(x => !excluded.Contains(x.def));
                    }
                }
                else
                {
                    HashSet<BodyPartRecord> excluded;
                    if (excludedPartsForTag.TryGetValue(tag, out excluded))
                    {
                        allParts = allParts.Except(excluded);
                    }
                }
            }

            return allParts;
        }

        public static IEnumerable<BodyPartRecord> MutatePawn(Pawn pawn, HediffRadiation radiation, float mutateAmount, float rareRatio, out Mutation mutationResult)
        {
            mutationResult = null;

            BodyPartRecord part = radiation.Part;
            Debug.Log("Finding mutation for part: " + part);
            Debug.Log("Rare ratio: " + rareRatio);

            Dictionary<string, HashSet<BodyPartDef>> excludedPartDefsForTag = new Dictionary<string, HashSet<BodyPartDef>>();
            Dictionary<string, HashSet<BodyPartRecord>> excludedPartsForTag = new Dictionary<string, HashSet<BodyPartRecord>>();
            foreach (Mutation existingMutation in pawn.health.hediffSet.GetHediffs<Mutation>())
            {
                foreach (string tag in existingMutation.def.exclusives)
                {
                    HashSet<BodyPartDef> setDef = excludedPartDefsForTag.TryGetValue(tag);
                    if (setDef == null)
                    {
                        excludedPartDefsForTag[tag] = setDef = new HashSet<BodyPartDef>();
                    }
                    setDef.Add(existingMutation.Part.def);

                    HashSet<BodyPartRecord> set = excludedPartsForTag.TryGetValue(tag);
                    if (set == null)
                    {
                        excludedPartsForTag[tag] = set = new HashSet<BodyPartRecord>();
                    }
                    set.Add(existingMutation.Part);
                }
            }

            Debug.Log("Excluded parts for tags: " + Debug.AsText(excludedPartsForTag));
            Debug.Log("Excluded defs for tags: " + Debug.AsText(excludedPartDefsForTag));

            var mutations = Mutations.Where(x => x.relatedParts==null || x.relatedParts.Contains(part.def));

            Debug.Log("All applicable mutations: " + Debug.AsText(mutations));

            Dictionary<MutationDef, IEnumerable<BodyPartRecord>> allowedMutations = new Dictionary<MutationDef, IEnumerable<BodyPartRecord>>();
            foreach (var mutation in mutations)
            {
                var parts = WhichPartsMutationIsAllowedOn(pawn, mutation, part, excludedPartDefsForTag, excludedPartsForTag);
                Debug.Log("  " + mutation + ": to " + Debug.AsText(parts));

                if (parts.Count() == 0) continue;
                allowedMutations[mutation] = parts;
            }

            Debug.Log("All allowed mutations: " + Debug.AsText(allowedMutations));
            if (allowedMutations.Count() == 0)
            {
                Debug.Log("No mutation possible!");
                return null;
            }

            MutationDef mutationDef = allowedMutations.Keys.RandomElementByWeight(x => (float)Math.Pow(x.likelihood, 1f - rareRatio));
            Debug.Log("Chose mutation: " + mutationDef);

            var applicableParts = allowedMutations[mutationDef];
            Debug.Log("Can be applied to body parts: " + Debug.AsText(applicableParts));

            if (allowedMutations.Count() == 0)
            {
                Debug.Log("No matching body parts!");
                return null;
            }

            var chosenPart = applicableParts.RandomElement();
            Debug.Log("Chose part: " + chosenPart);

            var chosenParts = pawn.health.hediffSet.GetNotMissingParts()
                .Where(x => mutationDef.affectsAllParts ? x.def == chosenPart.def : x == chosenPart);

            foreach (var partToMutate in chosenParts)
            {
                Debug.Log("Adding to part: " + partToMutate);

                if(pawn.health.hediffSet.GetHediffs<Mutation>().Where(x => x.def == mutationDef && x.Part == partToMutate).Any())
                {
                    Debug.Log("  But it already has this mutation!");
                    continue;
                }

                Mutation mutation = HediffMaker.MakeHediff(mutationDef, pawn, partToMutate) as Mutation;
                if (mutation == null) continue;

                mutationResult = mutation;
                pawn.health.AddHediff(mutation, partToMutate, null, null);
            }

            
            return chosenParts;
        }

    }
}
