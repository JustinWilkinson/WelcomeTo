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

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? StartedAtUtc { get; set; }

        public DateTime? CompletedAtUtc { get; set; }

        public List<Player> Players { get; set; }

        public Turn CurrentTurn { get; set; }

        public string WinnerText { get; set; }

        public GameDeck GameDeck { get; set; }

        public CityPlans Plans { get; set; }

        public List<int> TempAgencyPoints { get; set; }

        #region Turns
        public void StartNextTurn()
        {
            MarkCompletedCityPlans();
            CheckForGameOver();

            if (!CompletedAtUtc.HasValue)
            {
                GameDeck.FlipCards();

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
        }

        public void MarkCompletedCityPlans()
        {
            foreach (PlanType planType in Enum.GetValues(typeof(PlanType)))
            {
                if (!Plans[planType].CompletedByAnyPlayer && AnyPlayerCompletedPlan(planType))
                {
                    Plans[planType].CompletedByAnyPlayer = true;
                }
            }
        }

        public void CheckForGameOver()
        {
            var completedMessage = string.Join(", ", Players.Select(p => p.CompletedGameMessage()).Where(message => message != null));
            if (!string.IsNullOrWhiteSpace(completedMessage))
            {
                CompletedMessage = $"{completedMessage}.";
                CompletedAtUtc = DateTime.UtcNow;
                ComputeWinner();
            }
        }

        public void ComputeWinner()
        {
            var firstPlaceGroup = Players.GroupBy(p => GetPointsTotal(p)).OrderByDescending(g => g.Key).First().ToList();

            if (firstPlaceGroup.Count > 1)
            {
                IEnumerable<(Player Player, List<Estate> Estates)> playerEstates = firstPlaceGroup.Select(p => (p, p.Board.GetEstates(true)));
                var tieBreakGroup = playerEstates.GroupBy(p => p.Estates.Count).OrderByDescending(g => g.Key).First().ToList();

                if (tieBreakGroup.Count > 1)
                {
                    WinnerText = HandleTieBreaker(tieBreakGroup, 1);
                }
                else
                {
                    WinnerText = $"{tieBreakGroup[0].Player.Name} wins on a tie break with {GetPointsTotal(tieBreakGroup[0].Player)} and the most estates!";
                }
            }
            else
            {
                WinnerText = $"{firstPlaceGroup[0].Name} wins with {GetPointsTotal(firstPlaceGroup[0])} points!";
            }
        }
        #endregion

        #region Scoring
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
        #endregion

        #region Private
        private bool AnyPlayerCompletedPlan(PlanType planType) => Players.Any(p => p.ScoreSheet.GetCityPlanPoints(planType) > 0);

        private string HandleTieBreaker(IEnumerable<(Player Player, List<Estate> Estates)> tieBreakGroup, int estateSize)
        {
            if (!Enum.IsDefined(typeof(RealEstateSize), estateSize))
            {
                return $"A winner could not be determined, {string.Join(", ", tieBreakGroup.Select(x => x.Player.Name))} all have {GetPointsTotal(tieBreakGroup.First().Player)} points and matching estate sizes!";
            }
            else
            {
                var tieBreakWinners = tieBreakGroup.GroupBy(c => c.Estates.Where(c => c.HouseIndices.Count == estateSize).Count()).OrderByDescending(g => g.Key).First().ToList();

                if (tieBreakWinners.Count > 1)
                {
                    return HandleTieBreaker(tieBreakWinners, ++estateSize);
                }
                else
                {
                    return $"{tieBreakWinners[0].Player.Name} wins on a tie break with {GetPointsTotal(tieBreakWinners[0].Player)} points and the most size {estateSize} estates!";
                }
            }
        }
        #endregion
    }
}