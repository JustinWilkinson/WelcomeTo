using System.Collections.Generic;
using System.Linq;

namespace WelcomeTo.Shared
{
    public class Turn
    {
        public Card HouseNumberCard1 { get; set; }

        public Card HouseNumberCard2 { get; set; }

        public Card HouseNumberCard3 { get; set; }

        public Card EffectCard1 { get; set; }

        public Card EffectCard2 { get; set; }

        public Card EffectCard3 { get; set; }

        public Dictionary<Player, bool> PlayerActionTaken { get; set; }

        public IEnumerable<int> GetHouseNumbers() => new[] { HouseNumberCard1.HouseNumber, HouseNumberCard2.HouseNumber, HouseNumberCard3.HouseNumber };
    }
}