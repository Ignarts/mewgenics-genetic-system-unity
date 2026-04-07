using System;

namespace MewgenicsGenetics.Domain
{
    /// <summary>
    /// Derives a CatPhenotype from a CatGenome.
    ///
    /// Never store computed stats directly on the cat — always recalculate here.
    /// If you change a formula, all cats update automatically with no data migration.
    /// </summary>
    public static class CatStatResolver
    {
        public static CatPhenotype Resolve(CatGenome genome)
        {
            var phenotype = new CatPhenotype
            {
                maxHp     = ResolveMaxHp(genome),
                attack    = ResolveAttack(genome),
                speed     = ResolveSpeed(genome),
                sizeScale = ResolveSizeScale(genome),
            };

            // Express latent traits that meet activation conditions
            foreach (var trait in genome.latentTraits)
            {
                if (GenomeRules.ShouldExpressTrait(trait, genome))
                    phenotype.expressedTraits.Add(trait.id);
            }

            // Mutations always express — they've already broken through
            foreach (var mutation in genome.mutations)
                phenotype.expressedTraits.Add(mutation.id);

            return phenotype;
        }

        public static int ResolveMaxHp(CatGenome genome)
        {
            int hp = 8;
            hp += AlleleMath.SumPair(genome.vitality, 0, 4);
            return Math.Max(1, hp);
        }

        public static int ResolveAttack(CatGenome genome)
        {
            int atk = 3;
            atk += AlleleMath.SumPair(genome.aggression, 0, 4);
            return Math.Max(1, atk);
        }

        public static int ResolveSpeed(CatGenome genome)
        {
            int spd = 4;
            spd += AlleleMath.SumPair(genome.agility, 0, 4);
            return Math.Max(1, spd);
        }

        public static float ResolveSizeScale(CatGenome genome)
        {
            float scale = 0.8f + AlleleMath.AveragePair(genome.bodySize) * 0.15f;
            return (float)Math.Round(scale, 2);
        }
    }
}
