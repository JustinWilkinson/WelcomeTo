using System;
using WelcomeTo.Shared.Enumerations;

namespace WelcomeTo.Shared
{
    public class NumberEffectPair : IEquatable<NumberEffectPair>
    {
        public DeckIndex DeckIndex { get; set; }

        public int Number { get; set; }

        public CardType Effect { get; set; }

        public bool Equals(NumberEffectPair other) => DeckIndex == other.DeckIndex && Number == other.Number && Effect == other.Effect;
    }
}