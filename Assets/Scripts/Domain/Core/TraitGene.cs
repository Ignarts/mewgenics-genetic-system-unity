using System;

namespace MewgenicsGenetics.Domain
{
    [Serializable]
    public class TraitGene
    {
        public string id;
        public bool recessive;
        public float potency;

        public TraitGene(string id, bool recessive = false, float potency = 1f)
        {
            this.id = id;
            this.recessive = recessive;
            this.potency = potency;
        }

        public override string ToString() => $"{id} (recessive:{recessive}, potency:{potency:F2})";
    }
}
