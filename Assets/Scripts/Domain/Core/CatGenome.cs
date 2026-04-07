using System;
using System.Collections.Generic;

namespace MewgenicsGenetics.Domain
{
    /// <summary>
    /// The heritable information — what gets passed down across generations.
    /// Source of truth for a cat's genetics. Final stats are always derived from this,
    /// never stored separately.
    /// </summary>
    [Serializable]
    public class CatGenome
    {
        public GenePair coatColor;
        public GenePair bodySize;
        public GenePair vitality;
        public GenePair agility;
        public GenePair aggression;

        public List<MutationGene> mutations  = new();
        public List<TraitGene>   latentTraits = new();

        public int generation;

        /// <summary>
        /// Creates a random Generation-0 genome using the provided RNG.
        /// </summary>
        public static CatGenome CreateRandom(IRng rng = null)
        {
            rng ??= new SystemRng();
            return new CatGenome
            {
                coatColor  = new GenePair((byte)rng.Range(0, 8), (byte)rng.Range(0, 8)),
                bodySize   = new GenePair((byte)rng.Range(0, 4), (byte)rng.Range(0, 4)),
                vitality   = new GenePair((byte)rng.Range(0, 4), (byte)rng.Range(0, 4)),
                agility    = new GenePair((byte)rng.Range(0, 4), (byte)rng.Range(0, 4)),
                aggression = new GenePair((byte)rng.Range(0, 4), (byte)rng.Range(0, 4)),
                generation = 0,
            };
        }

        /// <summary>
        /// Creates a manually specified genome — useful for tests and controlled scenarios.
        /// </summary>
        public static CatGenome CreateManual(
            GenePair vitality,
            GenePair agility,
            GenePair aggression,
            GenePair bodySize,
            GenePair coatColor,
            int generation = 0)
        {
            return new CatGenome
            {
                vitality   = vitality,
                agility    = agility,
                aggression = aggression,
                bodySize   = bodySize,
                coatColor  = coatColor,
                generation = generation,
            };
        }
    }
}
