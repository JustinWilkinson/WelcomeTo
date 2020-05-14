using System.Collections.Generic;
using System.Linq;
using WelcomeTo.Shared.Extensions;

namespace WelcomeTo.Shared
{
    public class GameDeck
    {
        public Stack<Card> Deck1 { get; set; }

        public Stack<Card> Deck2 { get; set; }

        public Stack<Card> Deck3 { get; set; }

        public Stack<Card> Discard1 { get; set; }

        public Stack<Card> Discard2 { get; set; }

        public Stack<Card> Discard3 { get; set; }

        public void FlipCards()
        {
            var discard1 = Deck1.Pop();
            var discard2 = Deck2.Pop();
            var discard3 = Deck3.Pop();

            if (Deck1.Count == 0)
            {
                var reshuffledDeck = Discard1.Concat(Discard2).Concat(Discard3).Shuffle();
                reshuffledDeck.Distribute(Deck1, Deck2, Deck3);
            }

            Discard1.Push(discard1);
            Discard2.Push(discard2);
            Discard3.Push(discard3);
        }
    }
}