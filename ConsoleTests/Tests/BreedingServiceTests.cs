using MewgenicsGenetics.Domain;

namespace MewgenicsGenetics.ConsoleTests.Tests
{
    public static class BreedingServiceTests
    {
        public static void Run()
        {
            Console.WriteLine("\n=== BreedingServiceTests ===");

            Test_ChildGenerationIsParentPlusOne();
            Test_ChildAllelesComefromParents();
            Test_BreedingLogRecordsEvents();
            Test_MutationsCanBeInjected();
            Test_LatentTraitsCanBeInherited();
            Test_MultipleGenerationsIncrementCorrectly();

            Console.WriteLine("All BreedingServiceTests passed.\n");
        }

        private static void Test_ChildGenerationIsParentPlusOne()
        {
            var svc = new BreedingService(new FixedRng());
            var a   = CatGenome.CreateRandom(new SystemRng(1));
            var b   = CatGenome.CreateRandom(new SystemRng(2));

            var child = svc.Breed(a, b);

            Assert(child.generation == 1, "Child of gen-0 parents should be gen 1");
            Console.WriteLine("  [PASS] ChildGenerationIsParentPlusOne");
        }

        private static void Test_ChildAllelesComefromParents()
        {
            // FixedRng(0.9) always picks alleleA from parent, so child.alleleA = parentA.alleleA
            var svc = new BreedingService(new FixedRng(fixedValue: 0.9f));
            var a   = CatGenome.CreateManual(
                vitality:   new GenePair(3, 1),
                agility:    new GenePair(2, 0),
                aggression: new GenePair(1, 0),
                bodySize:   new GenePair(2, 1),
                coatColor:  new GenePair(5, 3));
            var b   = CatGenome.CreateManual(
                vitality:   new GenePair(0, 2),
                agility:    new GenePair(1, 3),
                aggression: new GenePair(2, 2),
                bodySize:   new GenePair(0, 1),
                coatColor:  new GenePair(7, 4));

            var child = svc.Breed(a, b);

            // With FixedRng(0.9): always picks alleleA from each parent
            Assert(child.vitality.alleleA == a.vitality.alleleA, "vitality.alleleA should come from parent A's alleleA");
            Assert(child.vitality.alleleB == b.vitality.alleleA, "vitality.alleleB should come from parent B's alleleA");
            Console.WriteLine("  [PASS] ChildAllelesComefromParents");
        }

        private static void Test_BreedingLogRecordsEvents()
        {
            var svc = new BreedingService(new FixedRng());
            var log = new BreedingLog();
            var a   = CatGenome.CreateRandom(new SystemRng(42));
            var b   = CatGenome.CreateRandom(new SystemRng(99));

            svc.Breed(a, b, log);

            Assert(log.Count > 0, "Breeding log should have recorded events");
            // Each gene pair generates 2 entries (alleleA + alleleB) × 5 genes = at least 10
            Assert(log.Count >= 10, $"Expected at least 10 log entries, got {log.Count}");
            Console.WriteLine("  [PASS] BreedingLogRecordsEvents");
        }

        private static void Test_MutationsCanBeInjected()
        {
            // FixedRng(0.9) → value=0.9, which is > any mutation potency (max ~0.10)
            // So mutations should NOT trigger with high fixed value
            var svcNoMutation = new BreedingService(new FixedRng(fixedValue: 0.99f));
            var a = CatGenome.CreateRandom(new SystemRng(1));
            var b = CatGenome.CreateRandom(new SystemRng(2));
            var childNoMut = svcNoMutation.Breed(a, b);
            Assert(childNoMut.mutations.Count == 0, "No mutations should trigger when RNG value is always high");

            // FixedRng(0.0) → value=0.0, which is < all mutation potencies → all trigger
            var svcAllMutations = new BreedingService(new FixedRng(fixedValue: 0.0f));
            var childAllMut = svcAllMutations.Breed(a, b);
            Assert(childAllMut.mutations.Count > 0, "All mutations should trigger when RNG value is always 0");

            Console.WriteLine("  [PASS] MutationsCanBeInjected");
        }

        private static void Test_LatentTraitsCanBeInherited()
        {
            // FixedRng(0.9) > 0.4 threshold → traits ARE inherited
            var svc = new BreedingService(new FixedRng(fixedValue: 0.9f));
            var a   = CatGenome.CreateRandom(new SystemRng(1));
            var b   = CatGenome.CreateRandom(new SystemRng(2));
            a.latentTraits.Add(new TraitGene("berserker", recessive: false, potency: 0.8f));
            b.latentTraits.Add(new TraitGene("swift_feet", recessive: false, potency: 0.9f));

            var child = svc.Breed(a, b);

            Assert(child.latentTraits.Exists(t => t.id == "berserker"),  "berserker should be inherited (rng > threshold)");
            Assert(child.latentTraits.Exists(t => t.id == "swift_feet"), "swift_feet should be inherited (rng > threshold)");
            Console.WriteLine("  [PASS] LatentTraitsCanBeInherited");
        }

        private static void Test_MultipleGenerationsIncrementCorrectly()
        {
            var svc = new BreedingService(new FixedRng());
            var a   = CatGenome.CreateRandom(new SystemRng(10));
            var b   = CatGenome.CreateRandom(new SystemRng(20));

            var gen1 = svc.Breed(a, b);
            var gen2 = svc.Breed(gen1, b);
            var gen3 = svc.Breed(gen2, b);

            Assert(gen1.generation == 1, $"gen1 should be 1, was {gen1.generation}");
            Assert(gen2.generation == 2, $"gen2 should be 2, was {gen2.generation}");
            Assert(gen3.generation == 3, $"gen3 should be 3, was {gen3.generation}");
            Console.WriteLine("  [PASS] MultipleGenerationsIncrementCorrectly");
        }

        private static void Assert(bool condition, string message)
        {
            if (!condition) throw new Exception($"[FAIL] {message}");
        }
    }
}
