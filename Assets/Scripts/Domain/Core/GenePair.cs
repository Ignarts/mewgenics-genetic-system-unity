using System;

namespace MewgenicsGenetics.Domain
{
    [Serializable]
    public struct GenePair
    {
        public byte alleleA;
        public byte alleleB;

        public GenePair(byte a, byte b)
        {
            alleleA = a;
            alleleB = b;
        }

        public override string ToString() => $"({alleleA}, {alleleB})";
    }
}
