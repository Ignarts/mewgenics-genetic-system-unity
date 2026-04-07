using MewgenicsGenetics.ConsoleTests.Tests;
using MewgenicsGenetics.Domain;

Console.WriteLine("╔══════════════════════════════════════════╗");
Console.WriteLine("║   Mewgenics Genetics — Console Test Run  ║");
Console.WriteLine("╚══════════════════════════════════════════╝");

// ── Unit tests ───────────────────────────────────────────────────────────────
int passed = 0;
int failed = 0;

RunSuite("BreedingServiceTests", BreedingServiceTests.Run, ref passed, ref failed);
RunSuite("CatStatResolverTests", CatStatResolverTests.Run, ref passed, ref failed);
RunSuite("GenomeRulesTests",     GenomeRulesTests.Run,     ref passed, ref failed);

// ── Integration demo ─────────────────────────────────────────────────────────
Console.WriteLine("\n=== Integration Demo: 4-Generation Lineage ===");
RunIntegrationDemo();

// ── Summary ──────────────────────────────────────────────────────────────────
Console.WriteLine("\n══════════════════════════════════════════════");
Console.ForegroundColor = failed == 0 ? ConsoleColor.Green : ConsoleColor.Red;
Console.WriteLine($"  Results: {passed} passed, {failed} failed");
Console.ResetColor();
Console.WriteLine("══════════════════════════════════════════════");

// ── Helpers ──────────────────────────────────────────────────────────────────

static void RunSuite(string name, Action suite, ref int passed, ref int failed)
{
    try
    {
        suite();
        passed++;
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[SUITE FAILED] {name}: {ex.Message}");
        Console.ResetColor();
        failed++;
    }
}

static void RunIntegrationDemo()
{
    var rng      = new SystemRng(seed: 42);   // fixed seed → reproducible output
    var svc      = new BreedingService(rng);

    var parentA  = CatGenome.CreateRandom(rng);
    parentA.latentTraits.Add(new TraitGene("iron_hide",   recessive: true,  potency: 0.9f));

    var parentB  = CatGenome.CreateRandom(rng);
    parentB.latentTraits.Add(new TraitGene("iron_hide",   recessive: true,  potency: 0.9f));
    parentB.latentTraits.Add(new TraitGene("speed_burst", recessive: false, potency: 0.8f));

    PrintCat("Parent A", parentA);
    PrintCat("Parent B", parentB);

    var current = parentA;
    for (int i = 1; i <= 4; i++)
    {
        var log   = new BreedingLog();
        var child = svc.Breed(current, parentB, log);

        Console.WriteLine($"\n── Gen {i} ──────────────────────────────────");
        PrintCat($"Gen {i}", child);
        Console.WriteLine("  Breeding log:");
        foreach (var entry in log.events)
            Console.WriteLine($"    {entry}");

        current = child;
    }
}

static void PrintCat(string label, CatGenome genome)
{
    var p = CatStatResolver.Resolve(genome);
    Console.WriteLine($"\n[{label}]  Gen:{genome.generation}");
    Console.WriteLine($"  {p}");
    Console.WriteLine($"  Vitality:{genome.vitality}  Agility:{genome.agility}  Aggression:{genome.aggression}");
    Console.WriteLine($"  BodySize:{genome.bodySize}  CoatColor:{genome.coatColor}");
    if (genome.mutations.Count > 0)
        Console.WriteLine($"  Mutations: {string.Join(", ", genome.mutations)}");
    if (genome.latentTraits.Count > 0)
        Console.WriteLine($"  Latent Traits: {string.Join(", ", genome.latentTraits)}");
}
