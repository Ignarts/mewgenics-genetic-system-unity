using MewgenicsGenetics.Domain;

namespace MewgenicsGenetics.ConsoleTests.Tests
{
    public static class CatStatResolverTests
    {
        public static void Run()
        {
            Console.WriteLine("\n=== CatStatResolverTests ===");

            Test_MinimumHpIsOne();
            Test_MaxVitalityGivesMaxHp();
            Test_ZeroAgilityGivesBaseSpeed();
            Test_FullAggressionGivesMaxAttack();
            Test_SizeScaleIsWithinExpectedRange();
            Test_MutationsAlwaysExpress();
            Test_DominantTraitExpressesWithoutDoubleAllele();
            Test_RecessiveTraitRequiresDoubleAllele();
            Test_LowPotencyTraitDoesNotExpress();

            Console.WriteLine("All CatStatResolverTests passed.\n");
        }

        private static void Test_MinimumHpIsOne()
        {
            // vitality (0,0) → SumPair=0 → hp = 8 + 0 = 8, still well above 1
            // Force minimum: override with negative-like concept isn't possible with byte,
            // so verify the floor is never breached.
            var genome = CatGenome.CreateManual(
                vitality:   new GenePair(0, 0),
                agility:    new GenePair(0, 0),
                aggression: new GenePair(0, 0),
                bodySize:   new GenePair(0, 0),
                coatColor:  new GenePair(0, 0));

            int hp = CatStatResolver.ResolveMaxHp(genome);
            Assert(hp >= 1, $"HP should never be below 1, got {hp}");
            Assert(hp == 8, $"HP with zero vitality should be base 8, got {hp}");
            Console.WriteLine("  [PASS] MinimumHpIsOne");
        }

        private static void Test_MaxVitalityGivesMaxHp()
        {
            // vitality (3,3) → SumPair clamped to 4 → hp = 8 + 4 = 12
            var genome = CatGenome.CreateManual(
                vitality:   new GenePair(3, 3),
                agility:    new GenePair(0, 0),
                aggression: new GenePair(0, 0),
                bodySize:   new GenePair(0, 0),
                coatColor:  new GenePair(0, 0));

            int hp = CatStatResolver.ResolveMaxHp(genome);
            Assert(hp == 12, $"Max vitality should give HP=12, got {hp}");
            Console.WriteLine("  [PASS] MaxVitalityGivesMaxHp");
        }

        private static void Test_ZeroAgilityGivesBaseSpeed()
        {
            var genome = CatGenome.CreateManual(
                vitality:   new GenePair(0, 0),
                agility:    new GenePair(0, 0),
                aggression: new GenePair(0, 0),
                bodySize:   new GenePair(0, 0),
                coatColor:  new GenePair(0, 0));

            int speed = CatStatResolver.ResolveSpeed(genome);
            Assert(speed == 4, $"Zero agility should give base speed=4, got {speed}");
            Console.WriteLine("  [PASS] ZeroAgilityGivesBaseSpeed");
        }

        private static void Test_FullAggressionGivesMaxAttack()
        {
            // aggression (3,3) → SumPair clamped to 4 → atk = 3 + 4 = 7
            var genome = CatGenome.CreateManual(
                vitality:   new GenePair(0, 0),
                agility:    new GenePair(0, 0),
                aggression: new GenePair(3, 3),
                bodySize:   new GenePair(0, 0),
                coatColor:  new GenePair(0, 0));

            int atk = CatStatResolver.ResolveAttack(genome);
            Assert(atk == 7, $"Max aggression should give ATK=7, got {atk}");
            Console.WriteLine("  [PASS] FullAggressionGivesMaxAttack");
        }

        private static void Test_SizeScaleIsWithinExpectedRange()
        {
            // Minimum bodySize (0,0) → average=0 → scale = 0.8
            // Maximum bodySize (3,3) → average=3 → scale = 0.8 + 3*0.15 = 1.25
            var genomeMin = CatGenome.CreateManual(
                vitality:   new GenePair(0, 0),
                agility:    new GenePair(0, 0),
                aggression: new GenePair(0, 0),
                bodySize:   new GenePair(0, 0),
                coatColor:  new GenePair(0, 0));

            var genomeMax = CatGenome.CreateManual(
                vitality:   new GenePair(0, 0),
                agility:    new GenePair(0, 0),
                aggression: new GenePair(0, 0),
                bodySize:   new GenePair(3, 3),
                coatColor:  new GenePair(0, 0));

            float scaleMin = CatStatResolver.ResolveSizeScale(genomeMin);
            float scaleMax = CatStatResolver.ResolveSizeScale(genomeMax);

            Assert(Math.Abs(scaleMin - 0.8f) < 0.001f, $"Min scale should be 0.8, got {scaleMin}");
            Assert(Math.Abs(scaleMax - 1.25f) < 0.001f, $"Max scale should be 1.25, got {scaleMax}");
            Console.WriteLine("  [PASS] SizeScaleIsWithinExpectedRange");
        }

        private static void Test_MutationsAlwaysExpress()
        {
            var genome = CatGenome.CreateManual(
                vitality:   new GenePair(0, 0),
                agility:    new GenePair(0, 0),
                aggression: new GenePair(0, 0),
                bodySize:   new GenePair(0, 0),
                coatColor:  new GenePair(0, 0));
            genome.mutations.Add(new MutationGene("extra_tail", 0.03f, 1));

            var phenotype = CatStatResolver.Resolve(genome);

            Assert(phenotype.expressedTraits.Contains("extra_tail"), "Mutations should always appear in expressed traits");
            Console.WriteLine("  [PASS] MutationsAlwaysExpress");
        }

        private static void Test_DominantTraitExpressesWithoutDoubleAllele()
        {
            // Non-recessive trait with high potency → should express even without homozygous alleles
            var genome = CatGenome.CreateManual(
                vitality:   new GenePair(1, 2),  // heterozygous
                agility:    new GenePair(0, 0),
                aggression: new GenePair(0, 0),
                bodySize:   new GenePair(0, 0),
                coatColor:  new GenePair(0, 0));
            genome.latentTraits.Add(new TraitGene("speed_burst", recessive: false, potency: 0.9f));

            var phenotype = CatStatResolver.Resolve(genome);

            Assert(phenotype.expressedTraits.Contains("speed_burst"), "Dominant trait with high potency should express");
            Console.WriteLine("  [PASS] DominantTraitExpressesWithoutDoubleAllele");
        }

        private static void Test_RecessiveTraitRequiresDoubleAllele()
        {
            // Recessive trait — heterozygous vitality alleles → should NOT express
            var genomeHetero = CatGenome.CreateManual(
                vitality:   new GenePair(1, 2),   // different alleles → not homozygous
                agility:    new GenePair(0, 0),
                aggression: new GenePair(0, 0),
                bodySize:   new GenePair(0, 0),
                coatColor:  new GenePair(0, 0));
            genomeHetero.latentTraits.Add(new TraitGene("iron_hide", recessive: true, potency: 0.9f));

            var phenoHetero = CatStatResolver.Resolve(genomeHetero);
            Assert(!phenoHetero.expressedTraits.Contains("iron_hide"),
                "Recessive trait should NOT express when alleles are heterozygous");

            // Homozygous vitality alleles → SHOULD express
            var genomeHomo = CatGenome.CreateManual(
                vitality:   new GenePair(2, 2),   // same alleles → homozygous
                agility:    new GenePair(0, 0),
                aggression: new GenePair(0, 0),
                bodySize:   new GenePair(0, 0),
                coatColor:  new GenePair(0, 0));
            genomeHomo.latentTraits.Add(new TraitGene("iron_hide", recessive: true, potency: 0.9f));

            var phenoHomo = CatStatResolver.Resolve(genomeHomo);
            Assert(phenoHomo.expressedTraits.Contains("iron_hide"),
                "Recessive trait SHOULD express when alleles are homozygous");

            Console.WriteLine("  [PASS] RecessiveTraitRequiresDoubleAllele");
        }

        private static void Test_LowPotencyTraitDoesNotExpress()
        {
            var genome = CatGenome.CreateManual(
                vitality:   new GenePair(2, 2),  // homozygous — passes recessive check
                agility:    new GenePair(0, 0),
                aggression: new GenePair(0, 0),
                bodySize:   new GenePair(0, 0),
                coatColor:  new GenePair(0, 0));
            // potency 0.5 is below the 0.65 threshold → stays dormant
            genome.latentTraits.Add(new TraitGene("night_vision", recessive: false, potency: 0.5f));

            var phenotype = CatStatResolver.Resolve(genome);
            Assert(!phenotype.expressedTraits.Contains("night_vision"),
                "Trait with potency <= 0.65 should not express");
            Console.WriteLine("  [PASS] LowPotencyTraitDoesNotExpress");
        }

        private static void Assert(bool condition, string message)
        {
            if (!condition) throw new Exception($"[FAIL] {message}");
        }
    }
}
