namespace MewgenicsGenetics.Domain
{
    public static class AlleleMath
    {
        /// <summary>
        /// Returns the clamped sum of both alleles in a pair.
        /// </summary>
        public static int SumPair(GenePair pair, int min, int max)
        {
            int sum = pair.alleleA + pair.alleleB;
            return System.Math.Clamp(sum, min, max);
        }

        /// <summary>
        /// Returns the average value of both alleles as a float.
        /// </summary>
        public static float AveragePair(GenePair pair)
        {
            return (pair.alleleA + pair.alleleB) / 2f;
        }

        /// <summary>
        /// An allele is considered dominant if its value is 4 or above (out of 0–7 range).
        /// </summary>
        public static bool IsDominant(byte allele) => allele >= 4;

        /// <summary>
        /// Both alleles are the same value — homozygous expression.
        /// </summary>
        public static bool IsHomozygous(GenePair pair) => pair.alleleA == pair.alleleB;
    }
}
