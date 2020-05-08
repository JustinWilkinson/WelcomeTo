using System.Collections.Generic;

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
    }
}