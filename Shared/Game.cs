using System;
using System.Collections.Generic;
using System.Linq;
using WelcomeTo.Shared.Enumerations;

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

        public List<int> TempAgencyPoints { get; set; }

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
                HouseNumberCard3 = GameDeck.Deck3.Peek(),
                PlayerNamesWithActionTaken = new List<string>()
            };
        }

        public int GetTempAgencyPoints(Player player)
        {
            var index = Players
                .Where(p => p.ScoreSheet.TempAgenciesUsed > 0)
                .GroupBy(p => p.ScoreSheet.TempAgenciesUsed)
                .OrderByDescending(g => g.Key)
                .Select((group, index) => new { Names = group.Select(player => player.Name), Index = index })
                .SingleOrDefault(x => x.Names.Contains(player.Name))?.Index;

            return index.HasValue && index.Value < TempAgencyPoints.Count ? TempAgencyPoints[index.Value] : 0;
        }

        public int GetPointsTotal(Player player)
        {
            var scoreSheet = player.ScoreSheet;
            return scoreSheet.Plan1 + scoreSheet.Plan2 + scoreSheet.Plan3 + scoreSheet.TopParks + scoreSheet.MiddleParks + scoreSheet.BottomParks + scoreSheet.Pools +
                GetTempAgencyPoints(player) + GetRealEstateValuePoints(player) - scoreSheet.Bis - scoreSheet.Refusals;
        }

        public int GetRealEstateValuePoints(Player player)
        {
            var estates = player.Board.GetEstates();
            var points = 0;
            for (var i = 1; i <= 6; i++)
            {
                points += player.ScoreSheet.RealEstateValuesTable[(RealEstateSize)i].First(x => !x.IsCovered).Points * estates.Where(e => e.HouseIndices.Count == i).Count();
            }
            return points;
        }
    }
}