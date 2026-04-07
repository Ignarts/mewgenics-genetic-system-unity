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
    ///   Extras → expressed traits and mutations
    /// </summary>
    public static class CatAsciiRenderer
    {
        public static string Render(CatGenome genome, CatPhenotype phenotype = null)
        {
            phenotype ??= CatStatResolver.Resolve(genome);

            string earTip  = EarTip(genome.agility);
            string eye     = Eye(genome.aggression);
            string mouth   = Mouth(genome.vitality);
            string body    = Body(genome.bodySize);
            string coat    = CoatLabel(genome.coatColor);
            string extras  = Extras(phenotype);

            // Fixed-width layout so cats align when printed in a row
            var sb = new StringBuilder();
            sb.AppendLine($"  {earTip}  {earTip}  ");
            sb.AppendLine($" < {eye}   {eye} > ");
            sb.AppendLine($" (  {mouth}  )  {coat}");
            sb.AppendLine($"  `{body}`  ");
            if (extras.Length > 0)
                sb.AppendLine($"  [{extras}]");

            return sb.ToString();
        }

        // ── Ears ─────────────────────────────────────────────────────────────
        // Low agility → droopy ears. High agility → sharp, alert ears.
        private static string EarTip(GenePair agility) =>
            AlleleMath.SumPair(agility, 0, 8) switch
            {
                <= 2 => "/-\\",   // droopy
                <= 4 => "/^\\",   // normal
                <= 6 => "/*\\",   // sharp
                _    => "/!\\",   // hyper-alert
            };

        // ── Eyes ─────────────────────────────────────────────────────────────
        // Low aggression → sleepy/calm. High aggression → fierce.
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
        // Low vitality → frail. High vitality → strong/robust.
        private static string Mouth(GenePair vitality) =>
            AlleleMath.SumPair(vitality, 0, 8) switch
            {
                <= 1 => "._",   // frail
                <= 3 => "~~",   // normal
                <= 5 => "==",   // robust
                _    => "##",   // tank
            };

        // ── Body / tail ──────────────────────────────────────────────────────
        // bodySize average maps to tail length — bigger cat, longer tail.
        private static string Body(GenePair bodySize) =>
            AlleleMath.AveragePair(bodySize) switch
            {
                < 1f  => "-vv-",       // tiny
                < 2f  => "-vvvv-",     // small
                < 3f  => "-vvvvvv-",   // medium
                _     => "-vvvvvvvv-", // large
            };

        // ── Coat label ───────────────────────────────────────────────────────
        // Maps the coatColor pair to a named pattern so designers can read it.
        private static string CoatLabel(GenePair coat)
        {
            int sum = AlleleMath.SumPair(coat, 0, 14);
            return sum switch
            {
                <= 2  => "white",
                <= 4  => "cream",
                <= 6  => "tabby",
                <= 8  => "calico",
                <= 10 => "tuxedo",
                <= 12 => "tortie",
                _     => "void",
            };
        }

        // ── Extras ───────────────────────────────────────────────────────────
        // Expressed traits and mutations appear as compact tags.
        private static string Extras(CatPhenotype phenotype)
        {
            if (phenotype.expressedTraits.Count == 0) return string.Empty;

            var tags = phenotype.expressedTraits.ConvertAll(t => t switch
            {
                "iron_hide"     => "🛡",
                "speed_burst"   => "⚡",
                "berserker"     => "🔥",
                "swift_feet"    => "💨",
                "giant_paws"    => "🐾",
                "hollow_bones"  => "🦴",
                "extra_tail"    => "〜",
                "vibrant_coat"  => "✦",
                "feral_instinct"=> "☠",
                "night_vision"  => "🌙",
                _               => t,
            });

            return string.Join(" ", tags);
        }
    }
}
