# Mewgenics Genetic System — Unity Prototype

A domain-driven genetic system for Unity inspired by the breeding mechanics of [Mewgenics](https://store.steampowered.com/app/2173390/Mewgenics/).

This repository is the technical companion to the article:
**[Replicating Mewgenics' Genetic System in Unity](https://www.ignarts.dev/blog/replicating-mewgenics-genetic-system-unity)**

---

## Project structure

```
Assets/Scripts/
├── Domain/               # Pure C# — no Unity dependency
│   ├── Core/             # CatGenome, CatPhenotype, GenePair, MutationGene, TraitGene
│   ├── Services/         # BreedingService, CatStatResolver, GenomeRules
│   └── Utils/            # AlleleMath, BreedingLog, IRng
├── Unity/
│   └── ScriptableObjects/  # GeneDefinition, AlleleDefinition
└── Demo/
    └── GeneticsDemo.cs   # MonoBehaviour demo — attach and press Play

ConsoleTests/             # .NET 8 console app — runs without Unity
├── Program.cs            # Test runner + integration demo
└── Tests/
    ├── BreedingServiceTests.cs
    ├── CatStatResolverTests.cs
    └── GenomeRulesTests.cs
```

---

## Running the console tests

Requires [.NET 8 SDK](https://dotnet.microsoft.com/download).

```bash
cd ConsoleTests
dotnet run
```

Each run uses a different random seed, so results vary every time. The seed is printed at the top — pass it as an argument to reproduce an exact run:

```bash
dotnet run -- 183741   # reproduce a specific run
```

After the unit tests pass, three interactive demos run automatically:

| Demo | What it shows |
|---|---|
| **1 · 6-Generation Lineage** | The same parent pair bred 6 times. Stats drift and traits accumulate across generations. Full breeding log included. |
| **2 · Mutation Frequency** | 30 random cats bred in a loop. Shows which mutations triggered and how often. |
| **3 · Recessive Trait Expression** | Two parents carrying `iron_hide` (recessive). Breeds up to 20 offspring and highlights the first one where the trait expresses. |

---

## Running the Unity demo

1. Open the project in Unity (2022.3 LTS or newer recommended).
2. Create an empty scene.
3. Add an empty `GameObject` and attach the `GeneticsDemo` component.
4. Open the **Console** window (`Window → General → Console`).
5. Press **Play**.

The demo creates two random Gen-0 parents with preset latent traits and breeds them across a configurable number of generations. Each cat's resolved stats, active traits, and full breeding log are printed to the console.

Available inspector settings:

| Field | Description |
|---|---|
| `Generations` | Number of offspring to generate |
| `Show Breeding Log` | Print per-allele inheritance and mutation events |
| `Random Seed` | Set to any value ≥ 0 for a reproducible run |

---

## Using the ScriptableObject gene catalog

1. In the Unity Project window, right-click → **Create → Genetics → Gene Definition**.
2. Fill in the `id`, `displayName`, allele values, and mutation chance.
3. Reference the asset from your own systems to drive gene data from the editor without touching code.

---

## Key design decisions

- **Genotype / Phenotype separation** — `CatGenome` is the source of truth. Stats are always derived via `CatStatResolver`, never stored.
- **Engine-agnostic domain layer** — `Domain/` has zero Unity dependencies, which is why the console tests work without the engine.
- **Deterministic testing via `IRng`** — inject `FixedRng` in tests to get reproducible results regardless of `UnityEngine.Random`.
- **`BreedingLog` is opt-in** — pass `null` in production code to avoid allocations; pass a `BreedingLog` instance when debugging.
