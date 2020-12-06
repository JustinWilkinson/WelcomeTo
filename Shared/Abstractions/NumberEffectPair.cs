using System;
using WelcomeTo.Shared.Enumerations;

namespace WelcomeTo.Shared.Abstractions
{
    public sealed record NumberEffectPair : IEquatable<NumberEffectPair>
    {
        public DeckIndex DeckIndex { get; init; }

        public int Number { get; init; }

        public CardType Effect { get; init; }

        bool IEquatable<NumberEffectPair>.Equals(NumberEffectPair other) => DeckIndex == other.DeckIndex && Number == other.Number && Effect == other.Effect;
    }
}