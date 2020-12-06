using System.Collections.Generic;
using System.Linq;
using WelcomeTo.Shared.Extensions;

namespace WelcomeTo.Shared.Abstractions
{
    public class GameDeck
    {
        public List<Card> Deck1 { get; set; }

        public List<Card> Deck2 { get; set; }

        public List<Card> Deck3 { get; set; }

        public List<Card> Discard1 { get; set; }

        public List<Card> Discard2 { get; set; }

        public List<Card> Discard3 { get; set; }

        public void FlipCards()
        {
            var discard1 = Deck1.Pop();
            var discard2 = Deck2.Pop();
            var discard3 = Deck3.Pop();

            if (Deck1.Count == 0)
            {
                var reshuffledDeck = Discard1.Concat(Discard2).Concat(Discard3).Shuffle();
                reshuffledDeck.Distribute(Deck1, Deck2, Deck3);

                Discard1.Clear();
                Discard2.Clear();
                Discard3.Clear();
            }

            Discard1.Insert(0, discard1);
            Discard2.Insert(0, discard2);
            Discard3.Insert(0, discard3);
        }

        public void Reshuffle()
        {
            var reshuffledDeck = Discard1.Concat(Discard2).Concat(Discard3).Concat(Deck1).Concat(Deck2).Concat(Deck3).Shuffle();
            
            Deck1.Clear();
            Deck2.Clear();
            Deck3.Clear();
            Discard1.Clear();
            Discard2.Clear();
            Discard3.Clear();

            reshuffledDeck.Distribute(Deck1, Deck2, Deck3);
        }
    }
}