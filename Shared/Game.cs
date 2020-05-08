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
    }
}