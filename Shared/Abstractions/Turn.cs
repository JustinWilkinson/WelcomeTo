﻿using System.Collections.Generic;
using WelcomeTo.Shared.Enumerations;

namespace WelcomeTo.Shared.Abstractions
{
    public record Turn
    {
        public Card HouseNumberCard1 { get; set; }

        public Card HouseNumberCard2 { get; set; }

        public Card HouseNumberCard3 { get; set; }

        public Card EffectCard1 { get; set; }

        public Card EffectCard2 { get; set; }

        public Card EffectCard3 { get; set; }

        public List<string> PlayerNamesWithActionTaken { get; set; } = new List<string>();

        public List<string> ReshuffleRequesters { get; set; } = new List<string>();

        public IEnumerable<NumberEffectPair> GetNumberEffectPairs() => new[]
        {
            new NumberEffectPair { DeckIndex = DeckIndex.Option1, Number = HouseNumberCard1.HouseNumber, Effect = EffectCard1.Type },
            new NumberEffectPair { DeckIndex = DeckIndex.Option2, Number = HouseNumberCard2.HouseNumber, Effect = EffectCard2.Type },
            new NumberEffectPair { DeckIndex = DeckIndex.Option3, Number = HouseNumberCard3.HouseNumber, Effect = EffectCard3.Type }
        };
    }
}