using System.Collections.Generic;
using WelcomeTo.Shared.Abstractions;
using WelcomeTo.Shared.Enumerations;

namespace WelcomeTo.Server.Configuration
{
    public record ApplicationOptions
    {
        public string ConnectionString { get; init; }

        public Dictionary<CardType, List<int>> CardDistribution { get; init; }

        public List<CityPlan> CityPlans { get; init; }

        public Dictionary<StreetPosition, List<bool>> PoolPositions { get; init; }

        public Dictionary<StreetPosition, List<int>> ParkPoints { get; init; }

        public Dictionary<RealEstateSize, List<int>> RealEstateSizes { get; init; }

        public IEnumerable<int> PoolPoints { get; init; }

        public IEnumerable<int> TempAgencyPoints { get; init; }

        public IEnumerable<int> BisPoints { get; init; }

        public IEnumerable<int> RefusalPoints { get; init; }
    }
}