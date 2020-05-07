using System;
using System.Collections.Generic;

namespace WelcomeTo.Shared
{
    public class Game
    {
        private static readonly Random _random = new Random();

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string CompletedMessage { get; set; }

        public DateTime? StartedAtUtc { get; set; }

        public DateTime? CompletedAtUtc { get; set; }

        public List<Player> Players { get; set; }

        public Turn CurrentTurn { get; set; }

        public string Winner { get; set; }

        public static Game NewGame(string id, string name)
        {
            return new Game
            {
                Id = new Guid(id),
                Name = name ?? "Unnamed Game",
                Players = new List<Player> { new Player { Name = "Host", IsHost = true } },
                CurrentTurn = new Turn()
            };
        }

        public void StartGame()
        {
            StartedAtUtc = DateTime.UtcNow;
        }
    }
}