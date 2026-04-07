using MewgenicsGenetics.Domain;

namespace MewgenicsGenetics.ConsoleTests.Tests
{
    public static class GenomeRulesTests
    {
        public static void Run()
        {
            Console.WriteLine("\n=== GenomeRulesTests ===");

            Test_HomozygousVitalityIsDoubleAllele();
            Test_HeterozygousVitalityIsNotDoubleAllele();
            Test_DominantTraitExpressesWithHighPotency();
            Test_RecessiveWithDoubleAlleleAndHighPotencyExpresses();
            Test_RecessiveWithoutDoubleAlleleDoesNotExpress();
            Test_PotencyBelowThresholdPreventsExpression();

            Console.WriteLine("All GenomeRulesTests passed.\n");
        }

        private static void Test_HomozygousVitalityIsDoubleAllele()
        {
            var genome = BuildGenome(vitality: new GenePair(3, 3));
            Assert(GenomeRules.HasDoubleAllele(genome, "vitality"),
                "Homozygous vitality should be detected as double allele");
            Console.WriteLine("  [PASS] HomozygousVitalityIsDoubleAllele");
        }

        private static void Test_HeterozygousVitalityIsNotDoubleAllele()
        {
            var genome = BuildGenome(vitality: new GenePair(1, 3));
            Assert(!GenomeRules.HasDoubleAllele(genome, "vitality"),
                "Heterozygous vitality should NOT be a double allele");
            Console.WriteLine("  [PASS] HeterozygousVitalityIsNotDoubleAllele");
        }

        private static void Test_DominantTraitExpressesWithHighPotency()
        {
            var genome = BuildGenome(vitality: new GenePair(1, 2));  // heterozygous — irrelevant for dominant
            var trait  = new TraitGene("berserker", recessive: false, potency: 0.8f);

            Assert(GenomeRules.ShouldExpressTrait(trait, genome),
                "Dominant trait with potency=0.8 should express");
            Console.WriteLine("  [PASS] DominantTraitExpressesWithHighPotency");
        }

        private static void Test_RecessiveWithDoubleAlleleAndHighPotencyExpresses()
        {
            var genome = BuildGenome(vitality: new GenePair(2, 2));  // homozygous
            var trait  = new TraitGene("vitality", recessive: true, potency: 0.9f);

            Assert(GenomeRules.ShouldExpressTrait(trait, genome),
                "Recessive trait with double allele and high potency should express");
            Console.WriteLine("  [PASS] RecessiveWithDoubleAlleleAndHighPotencyExpresses");
        }

        private static void Test_RecessiveWithoutDoubleAlleleDoesNotExpress()
        {
            var genome = BuildGenome(vitality: new GenePair(1, 3));  // heterozygous
            var trait  = new TraitGene("vitality", recessive: true, potency: 0.9f);

            Assert(!GenomeRules.ShouldExpressTrait(trait, genome),
                "Recessive trait without double allele should NOT express");
            Console.WriteLine("  [PASS] RecessiveWithoutDoubleAlleleDoesNotExpress");
        }

        private static void Test_PotencyBelowThresholdPreventsExpression()
        {
            var genome = BuildGenome(vitality: new GenePair(2, 2));  // homozygous — passes recessive check
            var trait  = new TraitGene("iron_hide", recessive: true, potency: 0.65f);  // exactly at threshold, not above

            Assert(!GenomeRules.ShouldExpressTrait(trait, genome),
                "Potency at exactly 0.65 should NOT express (threshold is strictly above 0.65)");
            Console.WriteLine("  [PASS] PotencyBelowThresholdPreventsExpression");
        }

        private static CatGenome BuildGenome(GenePair? vitality = null)
        {
            return CatGenome.CreateManual(
                vitality:   vitality ?? new GenePair(0, 0),
                agility:    new GenePair(0, 0),
                aggression: new GenePair(0, 0),
                bodySize:   new GenePair(0, 0),
                coatColor:  new GenePair(0, 0));
        }

        private static void Assert(bool condition, string message)
        {
            if (!condition) throw new Exception($"[FAIL] {message}");
        }
    }
}
