using System;
using WelcomeTo.Shared.Enumerations;

namespace WelcomeTo.Shared
{
    public class NumberEffectPair : IEquatable<NumberEffectPair>
    {
        public int Number { get; set; }

        public CardType Effect { get; set; }

        public bool Equals(NumberEffectPair other) => Number == other.Number && Effect == other.Effect;
    }
}