using System.Text;

namespace MewgenicsGenetics.Domain
{
    /// <summary>
    /// Renders a cat as ASCII art derived from its genome.
    /// Each visual element maps to a specific gene — the art is a direct
    /// read of the genetics, not a random decoration.
    ///
    ///   Ears   → agility     (droopy / normal / sharp)
    ///   Eyes   → aggression  (sleepy / calm / alert / fierce)
    ///   Mouth  → vitality    (frail / normal / robust)
    ///   Body   → bodySize    (small / medium / large)
    ///   Color  → coatColor   (use CoatColor() to get the ConsoleColor)
    ///   Extras → expressed traits and mutations
    /// </summary>
    public static class CatAsciiRenderer
    {
        public static string Render(CatGenome genome, CatPhenotype phenotype = null)
        {
            phenotype ??= CatStatResolver.Resolve(genome);

            string earTip = EarTip(genome.agility);
            string eye    = Eye(genome.aggression);
            string mouth  = Mouth(genome.vitality);
            string body   = Body(genome.bodySize);
            string extras = Extras(phenotype);

            var sb = new StringBuilder();
            sb.AppendLine($"  {earTip}  {earTip}  ");
            sb.AppendLine($" < {eye}   {eye} > ");
            sb.AppendLine($" (  {mouth}  )");
            sb.AppendLine($"  `{body}`  ");
            if (extras.Length > 0)
                sb.AppendLine($"  [{extras}]");

            return sb.ToString();
        }

        /// <summary>
        /// Returns the ConsoleColor that corresponds to this cat's coat gene.
        /// Call this before printing the art to set Console.ForegroundColor.
        /// </summary>
        public static ConsoleColor CoatColor(CatGenome genome) =>
            AlleleMath.SumPair(genome.coatColor, 0, 14) switch
            {
                <= 2  => ConsoleColor.White,       // white
                <= 4  => ConsoleColor.Yellow,      // cream
                <= 6  => ConsoleColor.DarkYellow,  // tabby
                <= 8  => ConsoleColor.Cyan,        // calico
                <= 10 => ConsoleColor.Gray,        // tuxedo
                <= 12 => ConsoleColor.Red,         // tortie
                _     => ConsoleColor.Magenta,     // void
            };

        // ── Ears ─────────────────────────────────────────────────────────────
        private static string EarTip(GenePair agility) =>
            AlleleMath.SumPair(agility, 0, 8) switch
            {
                <= 2 => "/-\\",   // droopy
                <= 4 => "/^\\",   // normal
                <= 6 => "/*\\",   // sharp
                _    => "/!\\",   // hyper-alert
            };

        // ── Eyes ─────────────────────────────────────────────────────────────
        private static string Eye(GenePair aggression) =>
            AlleleMath.SumPair(aggression, 0, 8) switch
            {
                <= 1 => "-",   // sleepy
                <= 3 => "o",   // calm
                <= 5 => "@",   // alert
                <= 6 => "*",   // fierce
                _    => ">",   // feral
            };

        // ── Mouth ────────────────────────────────────────────────────────────
        private static string Mouth(GenePair vitality) =>
            AlleleMath.SumPair(vitality, 0, 8) switch
            {
                <= 1 => "._",   // frail
                <= 3 => "~~",   // normal
                <= 5 => "==",   // robust
                _    => "##",   // tank
            };

        // ── Body / tail ──────────────────────────────────────────────────────
        private static string Body(GenePair bodySize) =>
            AlleleMath.AveragePair(bodySize) switch
            {
                < 1f => "-vv-",
                < 2f => "-vvvv-",
                < 3f => "-vvvvvv-",
                _    => "-vvvvvvvv-",
            };

        // ── Expressed traits and mutations ───────────────────────────────────
        private static string Extras(CatPhenotype phenotype)
        {
            if (phenotype.expressedTraits.Count == 0) return string.Empty;

            var tags = phenotype.expressedTraits.ConvertAll(t => t switch
            {
                "iron_hide"      => "🛡",
                "speed_burst"    => "⚡",
                "berserker"      => "🔥",
                "swift_feet"     => "💨",
                "giant_paws"     => "🐾",
                "hollow_bones"   => "🦴",
                "extra_tail"     => "〜",
                "vibrant_coat"   => "✦",
                "feral_instinct" => "☠",
                "night_vision"   => "🌙",
                _                => t,
            });

            return string.Join(" ", tags);
        }
    }
}
