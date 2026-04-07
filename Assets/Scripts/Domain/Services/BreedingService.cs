using System;
using System.Collections.Generic;

namespace MewgenicsGenetics.Domain
{
    /// <summary>
    /// Core breeding logic — isolated service with no MonoBehaviour dependency.
    /// Inject an IRng to make breeding fully deterministic in tests.
    ///
    /// Each breed() call:
    ///   1. Inherits one allele per gene from each parent (random pick from each pair).
    ///   2. Propagates latent traits with some inheritance chance.
    ///   3. Evaluates the mutation pool and may inject new mutations.
    /// </summary>
    public class BreedingService
    {
        private readonly IRng _rng;
        private readonly List<MutationGene> _mutationPool;

        public BreedingService(IRng rng = null, List<MutationGene> mutationPool = null)
        {
            _rng = rng ?? new SystemRng();
            _mutationPool = mutationPool ?? BuildDefaultMutationPool();
        }

        public CatGenome Breed(CatGenome parentA, CatGenome parentB, BreedingLog log = null)
        {
            var child = new CatGenome();

            child.coatColor  = InheritPair(parentA.coatColor,  parentB.coatColor,  "coatColor",  log);
            child.bodySize   = InheritPair(parentA.bodySize,   parentB.bodySize,   "bodySize",   log);
            child.vitality   = InheritPair(parentA.vitality,   parentB.vitality,   "vitality",   log);
            child.agility    = InheritPair(parentA.agility,    parentB.agility,    "agility",    log);
            child.aggression = InheritPair(parentA.aggression, parentB.aggression, "aggression", log);

            child.generation = Math.Max(parentA.generation, parentB.generation) + 1;
            log?.Record($"[Generation] Child is gen {child.generation}");

            InheritLatentTraits(child, parentA, parentB, log);
            MaybeInjectMutation(child, log);

            return child;
        }

        private GenePair InheritPair(GenePair a, GenePair b, string geneName, BreedingLog log)
        {
            bool pickAFromA = _rng.Value() > 0.5f;
            bool pickBFromB = _rng.Value() > 0.5f;

            var pair = new GenePair
            {
                alleleA = pickAFromA ? a.alleleA : a.alleleB,
                alleleB = pickBFromB ? b.alleleA : b.alleleB,
            };

            log?.Record($"  [{geneName}] alleleA ← parent A.{(pickAFromA ? "alleleA" : "alleleB")} ({pair.alleleA})");
            log?.Record($"  [{geneName}] alleleB ← parent B.{(pickBFromB ? "alleleA" : "alleleB")} ({pair.alleleB})");

            return pair;
        }

        private void InheritLatentTraits(CatGenome child, CatGenome parentA, CatGenome parentB, BreedingLog log)
        {
            var inherited = new HashSet<string>();

            void TryInherit(TraitGene trait, string parentLabel)
            {
                if (inherited.Contains(trait.id)) return;
                if (_rng.Value() > 0.4f)
                {
                    child.latentTraits.Add(new TraitGene(trait.id, trait.recessive, trait.potency));
                    inherited.Add(trait.id);
                    log?.Record($"  [Trait] '{trait.id}' inherited from {parentLabel}");
                }
                else
                {
                    log?.Record($"  [Trait] '{trait.id}' from {parentLabel} — not inherited");
                }
            }

            foreach (var trait in parentA.latentTraits) TryInherit(trait, "parent A");
            foreach (var trait in parentB.latentTraits) TryInherit(trait, "parent B");
        }

        private void MaybeInjectMutation(CatGenome child, BreedingLog log)
        {
            foreach (var mutation in _mutationPool)
            {
                if (_rng.Value() < mutation.potency)
                {
                    child.mutations.Add(new MutationGene(mutation.id, mutation.potency, child.generation));
                    log?.Record($"  [Mutation] '{mutation.id}' triggered at gen {child.generation} (chance:{mutation.potency:F2})");
                }
            }
        }

        private static List<MutationGene> BuildDefaultMutationPool() => new()
        {
            new MutationGene("giant_paws",     0.05f, 0),
            new MutationGene("hollow_bones",   0.08f, 0),
            new MutationGene("extra_tail",     0.03f, 0),
            new MutationGene("vibrant_coat",   0.10f, 0),
            new MutationGene("feral_instinct", 0.06f, 0),
        };
    }
}
