using System;

namespace MewgenicsGenetics.Domain
{
    [Serializable]
    public class MutationGene
    {
        public string id;
        public float potency;
        public int generationIntroduced;

        public MutationGene(string id, float potency, int generationIntroduced)
        {
            this.id = id;
            this.potency = potency;
            this.generationIntroduced = generationIntroduced;
        }

        public override string ToString() => $"{id} (potency:{potency:F2}, gen:{generationIntroduced})";
    }
}
