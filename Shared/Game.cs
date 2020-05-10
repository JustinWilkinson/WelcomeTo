using System;
using System.Collections.Generic;

namespace WelcomeTo.Shared
{
    public class Game
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string CompletedMessage { get; set; }

        public DateTime? StartedAtUtc { get; set; }

        public DateTime? CompletedAtUtc { get; set; }

        public List<Player> Players { get; set; }

        public Turn CurrentTurn { get; set; }

        public Player Winner { get; set; }

        public GameDeck GameDeck { get; set; }

        public CityPlans Plans { get; set; }

        public void StartNextTurn()
        {
            GameDeck.Discard1.Push(GameDeck.Deck1.Pop());
            GameDeck.Discard2.Push(GameDeck.Deck2.Pop());
            GameDeck.Discard3.Push(GameDeck.Deck3.Pop());
            
            CurrentTurn = new Turn
            {
                EffectCard1 = GameDeck.Discard1.Peek(),
                EffectCard2 = GameDeck.Discard2.Peek(),
                EffectCard3 = GameDeck.Discard3.Peek(),
                HouseNumberCard1 = GameDeck.Deck1.Peek(),
                HouseNumberCard2 = GameDeck.Deck2.Peek(),
                HouseNumberCard3 = GameDeck.Deck3.Peek()
            };
        }
    }
}