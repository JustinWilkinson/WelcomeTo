using System;
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

        public int GetCityPlanPoints(PlanType planType) => planType switch
        {
            PlanType.No1 => Plan1,
            PlanType.No2 => Plan2,
            PlanType.No3 => Plan3,
            _ => throw new ArgumentException($"Unrecognized plan type '{planType}'.")
        };

        public void SetCityPlanPoints(PlanType planType, int points)
        {
            switch (planType)
            {
                case PlanType.No1:
                    Plan1 = points;
                    break;
                case PlanType.No2:
                    Plan2 = points;
                    break;
                case PlanType.No3:
                    Plan3 = points;
                    break;
                default:
                    throw new ArgumentException($"Unrecognized plan type '{planType}'.");
            };
        }

        public int GetTotal(Game game, string playerName) => Plan1 + Plan2 + Plan3 + TopParks + MiddleParks + BottomParks + Pools + GetTempAgencyPoints(game, playerName) + RealEstateValue - Bis - Refusals;
    }
}