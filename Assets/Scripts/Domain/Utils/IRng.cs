namespace MewgenicsGenetics.Domain
{
    /// <summary>
    /// Abstraction over random number generation.
    /// Allows injecting deterministic RNG in tests instead of relying on Unity.Random.
    /// </summary>
    public interface IRng
    {
        float Value();
        int Range(int minInclusive, int maxExclusive);
    }

    /// <summary>
    /// Default implementation backed by System.Random.
    /// Works in both Unity and pure console contexts.
    /// </summary>
    public class SystemRng : IRng
    {
        private readonly System.Random _random;

        public SystemRng(int seed = -1)
        {
            _random = seed >= 0 ? new System.Random(seed) : new System.Random();
        }

        public float Value() => (float)_random.NextDouble();
        public int Range(int minInclusive, int maxExclusive) => _random.Next(minInclusive, maxExclusive);
    }

    /// <summary>
    /// Deterministic RNG for tests — always returns the same sequence.
    /// </summary>
    public class FixedRng : IRng
    {
        private readonly float _fixedValue;
        private readonly int _fixedInt;

        public FixedRng(float fixedValue = 0.9f, int fixedInt = 0)
        {
            _fixedValue = fixedValue;
            _fixedInt = fixedInt;
        }

        public float Value() => _fixedValue;
        public int Range(int minInclusive, int maxExclusive) => System.Math.Clamp(_fixedInt, minInclusive, maxExclusive - 1);
    }
}
