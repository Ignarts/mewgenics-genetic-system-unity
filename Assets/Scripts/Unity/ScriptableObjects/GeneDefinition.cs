using UnityEngine;

namespace MewgenicsGenetics.Unity
{
    /// <summary>
    /// Defines a gene in the editor catalog.
    /// Designers can add new genes, tune allele variants, and adjust mutation
    /// probabilities without touching code — they're just data changes.
    /// </summary>
    [CreateAssetMenu(menuName = "Genetics/Gene Definition", fileName = "GeneDefinition")]
    public class GeneDefinition : ScriptableObject
    {
        public string id;
        public string displayName;

        [TextArea]
        public string description;

        public AlleleDefinition[] alleles;

        public bool canMutate;

        [Range(0f, 1f)]
        public float mutationChance;
    }
}
