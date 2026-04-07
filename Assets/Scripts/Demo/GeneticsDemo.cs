using UnityEngine;
using MewgenicsGenetics.Domain;

namespace MewgenicsGenetics.Demo
{
    /// <summary>
    /// Drop this on any GameObject to run a multi-generation breeding demo in the console.
    ///
    /// Setup:
    ///   1. Create an empty GameObject in the scene.
    ///   2. Attach this component.
    ///   3. Press Play and open the Console window.
    ///
    /// What it shows:
    ///   - Two random Gen-0 parents, each with a latent trait.
    ///   - A configurable number of offspring generations bred sequentially.
    ///   - Each cat's phenotype (resolved stats + expressed traits).
    ///   - Optional full breeding log showing allele inheritance and mutation events.
    /// </summary>
    public class GeneticsDemo : MonoBehaviour
    {
        [Header("Breeding")]
        [SerializeField] private int  generations    = 5;
        [SerializeField] private bool showBreedingLog = true;

        [Header("Seed (-1 = random)")]
        [SerializeField] private int randomSeed = -1;

        private BreedingService _breedingService;

        private void Start()
        {
            var rng = randomSeed >= 0 ? new SystemRng(randomSeed) : new SystemRng();
            _breedingService = new BreedingService(rng);

            RunDemo(rng);
        }

        private void RunDemo(IRng rng)
        {
            Debug.Log("══════════════════════════════════════");
            Debug.Log("   Mewgenics Genetics Demo");
            Debug.Log("══════════════════════════════════════\n");

            // Build two Gen-0 parents with preset latent traits for interesting output
            var parentA = CatGenome.CreateRandom(rng);
            parentA.latentTraits.Add(new TraitGene("iron_hide",   recessive: true,  potency: 0.9f));
            parentA.latentTraits.Add(new TraitGene("night_vision", recessive: false, potency: 0.5f)); // potency too low — stays dormant

            var parentB = CatGenome.CreateRandom(rng);
            parentB.latentTraits.Add(new TraitGene("speed_burst", recessive: false, potency: 0.8f));
            parentB.latentTraits.Add(new TraitGene("iron_hide",   recessive: true,  potency: 0.9f)); // same recessive as A

            Debug.Log("── Parents ─────────────────────────");
            LogCat("Parent A", parentA);
            LogCat("Parent B", parentB);

            var currentA = parentA;
            var currentB = parentB;

            Debug.Log("\n── Offspring ───────────────────────");
            for (int gen = 1; gen <= generations; gen++)
            {
                var log   = showBreedingLog ? new BreedingLog() : null;
                var child = _breedingService.Breed(currentA, currentB, log);

                LogCat($"Generation {gen}", child);

                if (showBreedingLog && log != null)
                {
                    Debug.Log($"  <Breeding Log>\n{log.Dump()}\n");
                }

                // Each child becomes the next "parent A" — parent B stays constant for variation
                currentA = child;
            }

            Debug.Log("══════════════════════════════════════");
        }

        private static void LogCat(string label, CatGenome genome)
        {
            var p = CatStatResolver.Resolve(genome);
            Debug.Log($"[{label}]  Gen:{genome.generation}  {p}");

            Debug.Log($"  Vitality    {genome.vitality}   Agility    {genome.agility}");
            Debug.Log($"  Aggression  {genome.aggression}   BodySize   {genome.bodySize}   CoatColor  {genome.coatColor}");

            if (genome.mutations.Count > 0)
                Debug.Log($"  Mutations  : {string.Join(", ", genome.mutations)}");

            if (genome.latentTraits.Count > 0)
                Debug.Log($"  Latent Traits: {string.Join(", ", genome.latentTraits)}");

            Debug.Log(string.Empty);
        }
    }
}
