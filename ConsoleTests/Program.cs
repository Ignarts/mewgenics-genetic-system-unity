using MewgenicsGenetics.ConsoleTests.Tests;
using MewgenicsGenetics.Domain;

int seed = args.Length > 0 && int.TryParse(args[0], out int s) ? s : Environment.TickCount;
var rng  = new SystemRng(seed);

Console.WriteLine("╔══════════════════════════════════════════╗");
Console.WriteLine("║   Mewgenics Genetics — Console Test Run  ║");
Console.WriteLine("╚══════════════════════════════════════════╝");
Console.WriteLine($"  Seed: {seed}  (reproduce this run with: dotnet run -- {seed})\n");

// ── Unit tests ───────────────────────────────────────────────────────────────
int passed = 0;
int failed = 0;

RunSuite("BreedingServiceTests", BreedingServiceTests.Run, ref passed, ref failed);
RunSuite("CatStatResolverTests", CatStatResolverTests.Run, ref passed, ref failed);
RunSuite("GenomeRulesTests",     GenomeRulesTests.Run,     ref passed, ref failed);

Console.WriteLine("\n══════════════════════════════════════════════");
Console.ForegroundColor = failed == 0 ? ConsoleColor.Green : ConsoleColor.Red;
Console.WriteLine($"  Unit tests: {passed} passed, {failed} failed");
Console.ResetColor();
Console.WriteLine("══════════════════════════════════════════════");

if (failed > 0) return;

// ── Demos (only run if all tests pass) ───────────────────────────────────────
RunLineageDemo(rng);
RunMutationStressTest(rng);
RunRecessiveTraitDemo(rng);

// ─────────────────────────────────────────────────────────────────────────────

static void RunSuite(string name, Action suite, ref int passed, ref int failed)
{
    try   { suite(); passed++; }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[SUITE FAILED] {name}: {ex.Message}");
        Console.ResetColor();
        failed++;
    }
}

// ── Demo 1: Lineage — breed the same pair across 6 generations ───────────────
static void RunLineageDemo(IRng rng)
{
    Console.WriteLine("\n╔══════════════════════════════════════════╗");
    Console.WriteLine("║  Demo 1 · 6-Generation Lineage           ║");
    Console.WriteLine("║  Same parents bred repeatedly.           ║");
    Console.WriteLine("║  Watch how stats drift across generations.║");
    Console.WriteLine("╚══════════════════════════════════════════╝");

    var svc     = new BreedingService(rng);
    var parentA = CatGenome.CreateRandom(rng);
    var parentB = CatGenome.CreateRandom(rng);

    parentA.latentTraits.Add(new TraitGene("iron_hide",   recessive: true,  potency: 0.9f));
    parentB.latentTraits.Add(new TraitGene("iron_hide",   recessive: true,  potency: 0.9f));
    parentB.latentTraits.Add(new TraitGene("speed_burst", recessive: false, potency: 0.8f));

    Console.WriteLine();
    PrintCat("Parent A", parentA);
    PrintCat("Parent B", parentB);

    var current = parentA;
    for (int i = 1; i <= 6; i++)
    {
        var log   = new BreedingLog();
        var child = svc.Breed(current, parentB, log);

        Console.WriteLine($"\n── Gen {i} ──────────────────────────");
        PrintCat($"Gen {i}", child);
        Console.WriteLine("  Breeding log:");
        foreach (var entry in log.events)
            Console.WriteLine($"    {entry}");

        current = child;
    }
}

// ── Demo 2: Mutation stress — breed 30 cats, count how often each mutation appears
static void RunMutationStressTest(IRng rng)
{
    Console.WriteLine("\n╔══════════════════════════════════════════╗");
    Console.WriteLine("║  Demo 2 · Mutation Frequency (30 cats)   ║");
    Console.WriteLine("║  How often does each mutation trigger?   ║");
    Console.WriteLine("╚══════════════════════════════════════════╝\n");

    var svc    = new BreedingService(rng);
    var counts = new Dictionary<string, int>();
    int total  = 30;

    for (int i = 0; i < total; i++)
    {
        var a     = CatGenome.CreateRandom(rng);
        var b     = CatGenome.CreateRandom(rng);
        var child = svc.Breed(a, b);

        foreach (var m in child.mutations)
            counts[m.id] = counts.GetValueOrDefault(m.id) + 1;
    }

    if (counts.Count == 0)
    {
        Console.WriteLine("  No mutations in this run (try running again — mutations are rare).");
    }
    else
    {
        foreach (var (id, count) in counts.OrderByDescending(x => x.Value))
        {
            float pct = count / (float)total * 100f;
            string bar = new string('█', count);
            Console.WriteLine($"  {id,-20} {bar,-30} {count}/{total} ({pct:F0}%)");
        }
    }
}

// ── Demo 3: Recessive trait — breed until iron_hide expresses ────────────────
static void RunRecessiveTraitDemo(IRng rng)
{
    Console.WriteLine("\n╔══════════════════════════════════════════╗");
    Console.WriteLine("║  Demo 3 · Recessive Trait Expression     ║");
    Console.WriteLine("║  Both parents carry iron_hide (recessive)║");
    Console.WriteLine("║  Breed until it expresses in an offspring.║");
    Console.WriteLine("╚══════════════════════════════════════════╝\n");

    var svc     = new BreedingService(rng);
    var parentA = CatGenome.CreateManual(
        vitality:   new GenePair(2, 2),   // homozygous — iron_hide can express
        agility:    new GenePair(1, 2),
        aggression: new GenePair(0, 1),
        bodySize:   new GenePair(1, 1),
        coatColor:  new GenePair(3, 5));
    var parentB = CatGenome.CreateManual(
        vitality:   new GenePair(2, 1),   // heterozygous
        agility:    new GenePair(3, 0),
        aggression: new GenePair(2, 2),
        bodySize:   new GenePair(0, 2),
        coatColor:  new GenePair(6, 1));

    parentA.latentTraits.Add(new TraitGene("iron_hide", recessive: true, potency: 0.9f));
    parentB.latentTraits.Add(new TraitGene("iron_hide", recessive: true, potency: 0.9f));

    Console.WriteLine("  Looking for a cat that expresses iron_hide...\n");

    bool found = false;
    for (int attempt = 1; attempt <= 20; attempt++)
    {
        var child     = svc.Breed(parentA, parentB);
        var phenotype = CatStatResolver.Resolve(child);
        bool expresses = phenotype.expressedTraits.Contains("iron_hide");

        Console.Write($"  Attempt {attempt,2}: vitality {child.vitality}  ");
        if (expresses)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("iron_hide EXPRESSED");
            Console.ResetColor();
            PrintCat($"  Winner (attempt {attempt})", child);
            found = true;
            break;
        }
        else
        {
            Console.WriteLine("dormant");
        }
    }

    if (!found)
        Console.WriteLine("\n  iron_hide stayed dormant across all attempts this run — try again.");
}

// ── Shared printer ────────────────────────────────────────────────────────────
static void PrintCat(string label, CatGenome genome)
{
    var p = CatStatResolver.Resolve(genome);
    Console.WriteLine($"[{label}]  Gen:{genome.generation}");
    Console.WriteLine($"  {p}");
    Console.WriteLine($"  Vitality:{genome.vitality}  Agility:{genome.agility}  Aggression:{genome.aggression}");
    Console.WriteLine($"  BodySize:{genome.bodySize}  CoatColor:{genome.coatColor}");
    if (genome.mutations.Count > 0)
        Console.WriteLine($"  Mutations: {string.Join(", ", genome.mutations)}");
    if (genome.latentTraits.Count > 0)
        Console.WriteLine($"  Latent: {string.Join(", ", genome.latentTraits)}");
    Console.WriteLine();
}
