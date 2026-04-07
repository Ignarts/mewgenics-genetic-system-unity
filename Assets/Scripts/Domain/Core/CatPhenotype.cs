using System.Collections.Generic;

namespace MewgenicsGenetics.Domain
{
    /// <summary>
    /// The observable result — what actually appears in the game.
    /// Always derived from CatGenome, never stored independently.
    /// Changing balance formulas in CatStatResolver automatically updates all cats.
    /// </summary>
    public class CatPhenotype
    {
        public float sizeScale;
        public int   maxHp;
        public int   attack;
        public int   speed;

        public List<string> expressedTraits = new();

        public override string ToString()
        {
            string traits = expressedTraits.Count > 0
                ? $" | Traits: [{string.Join(", ", expressedTraits)}]"
                : string.Empty;
            return $"HP:{maxHp}  ATK:{attack}  SPD:{speed}  Scale:{sizeScale:F2}{traits}";
        }
    }
}
