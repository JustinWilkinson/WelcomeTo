using System.Collections.Generic;
using System.Linq;
using WelcomeTo.Shared.Enumerations;

namespace WelcomeTo.Shared
{
    public class ScoreSheet
    {
        public Dictionary<RealEstateSize, List<PointsListItem>> RealEstateValuesTable { get; set; }

        public List<PointsListItem> PoolPoints { get; set; }

        public List<int> TempAgencyPoints { get; set; }

        public List<PointsListItem> BisPoints { get; set; }

        public List<PointsListItem> RefusalPoints { get; set; }

        public int Plan1 { get; set; }

        public int Plan2 { get; set; }

        public int Plan3 { get; set; }

        public int TopParks { get; set; }

        public int MiddleParks { get; set; }

        public int BottomParks { get; set; }

        public int Pools => PoolPoints.First(x => !x.IsCovered).Points;

        public int TempAgenciesUsed { get; set; }

        public int RealEstateValue { get; set; }

        public int Bis => BisPoints.First(x => !x.IsCovered).Points;

        public int Refusals => RefusalPoints.First(x => !x.IsCovered).Points;

        public int GetTempAgencyPoints(Game game, string playerName)
        {
            var index = game.Players
                .Where(p => p.ScoreSheet.TempAgenciesUsed > 0)
                .GroupBy(p => p.ScoreSheet.TempAgenciesUsed)
                .OrderByDescending(g => g.Key)
                .Select((group, index) => new { Names = group.Select(player => player.Name), Index = index })
                .SingleOrDefault(x => x.Names.Contains(playerName))?.Index;

            return index.HasValue && index.Value < TempAgencyPoints.Count ? TempAgencyPoints[index.Value] : 0;
        }

        public int GetTotal(Game game, string playerName) => Plan1 + Plan2 + Plan3 + TopParks + MiddleParks + BottomParks + GetTempAgencyPoints(game, playerName) + RealEstateValue - Bis - Refusals;
    }
}