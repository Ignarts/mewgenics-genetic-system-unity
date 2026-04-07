using System.Collections.Generic;

namespace MewgenicsGenetics.Domain
{
    /// <summary>
    /// Records every meaningful event during genome construction.
    /// Essential for debugging emergent behavior — without this, you see
    /// the result but can't trace the cause.
    /// </summary>
    public class BreedingLog
    {
        public List<string> events = new();

        public void Record(string message) => events.Add(message);

        public void Clear() => events.Clear();

        public string Dump() => string.Join("\n", events);

        public int Count => events.Count;
    }
}
