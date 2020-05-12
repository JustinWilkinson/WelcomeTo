using System.Collections.Generic;
using System.Linq;

namespace WelcomeTo.Shared
{
    public class ScoreSheet
    {
        public List<RealEstateValueTableCell> RealEstateValueTableCell { get; set; }

        public List<PointsListItem> PoolPoints { get; set; }

        public List<PointsListItem> BisPoints { get; set; }

        public List<PointsListItem> RefusalPoints { get; set; }

        public int Plan1 { get; set; }

        public int Plan2{ get; set; }

        public int Plan3 { get; set; }

        public int TopParks{ get; set; }

        public int MiddleParks { get; set; }

        public int BottomParks { get; set; }

        public int Pools => PoolPoints.First(x => !x.IsCovered).Points;

        public int TempAgenciesUsed { get; set; }

        public int RealEstateValue { get; set; }

        public int Bis => BisPoints.First(x => !x.IsCovered).Points;

        public int Refusals => RefusalPoints.First(x => !x.IsCovered).Points;

        public int GetTempAgencyPoints(Game game, string playerName)
        {
            var ranking = game.Players
                .GroupBy(p => p.ScoreSheet.TempAgenciesUsed)
                .OrderByDescending(g => g.Key)
                .Select((group, index) => new { Names = group.Select(player => player.Name), Ranking = index + 1 })
                .Single(x => x.Names.Contains(playerName)).Ranking;

            if (ranking == 1)
            {
                return 7;
            }
            else if (ranking == 2)
            {
                return 4;
            }
            else if (ranking == 3)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public int GetTotal(Game game, string playerName) => Plan1 + Plan2 + Plan3 + TopParks + MiddleParks + BottomParks + GetTempAgencyPoints(game, playerName) + RealEstateValue - Bis - Refusals;
    }
}