namespace MewgenicsGenetics.Domain
{
    /// <summary>
    /// Business rules for evaluating genome state.
    /// Centralises trait expression logic so it can be reused and tested independently.
    /// </summary>
    public static class GenomeRules
    {
        /// <summary>
        /// Returns true if the genome carries the same allele on both copies
        /// of the gene associated with the given trait id — homozygous expression.
        /// </summary>
        public static bool HasDoubleAllele(CatGenome genome, string traitId)
        {
            var pair = GetPairForTrait(genome, traitId);
            return AlleleMath.IsHomozygous(pair);
        }

        /// <summary>
        /// Determines whether a latent trait becomes expressed in the phenotype.
        ///
        /// Rules:
        ///   - Recessive traits require homozygous alleles to express.
        ///   - Any trait with potency <= 0.65 stays dormant regardless of zygosity.
        /// </summary>
        public static bool ShouldExpressTrait(TraitGene trait, CatGenome genome)
        {
            if (trait.recessive && !HasDoubleAllele(genome, trait.id))
                return false;

            return trait.potency > 0.65f;
        }

        // Maps trait IDs to the gene pair they're associated with.
        // Extend this switch when adding new genes.
        private static GenePair GetPairForTrait(CatGenome genome, string traitId) =>
            traitId switch
            {
                "vitality"   => genome.vitality,
                "agility"    => genome.agility,
                "aggression" => genome.aggression,
                "bodySize"   => genome.bodySize,
                "coatColor"  => genome.coatColor,
                _            => genome.vitality,
            };
    }
}
