using System.Collections.Generic;
using WelcomeTo.Shared.Abstractions;
using WelcomeTo.Shared.Enumerations;

namespace WelcomeTo.Server.Configuration
{
    public class ApplicationOptions
    {
        public string ConnectionString { get; set; }

        public Dictionary<CardType, List<int>> CardDistribution { get; set; }

        public List<CityPlan> CityPlans { get; set; }

        public Dictionary<StreetPosition, List<bool>> PoolPositions { get; set; }

        public Dictionary<StreetPosition, List<int>> ParkPoints { get; set; }

        public Dictionary<RealEstateSize, List<int>> RealEstateSizes { get; set; }

        public IEnumerable<int> PoolPoints { get; set; }

        public IEnumerable<int> TempAgencyPoints { get; set; }

        public IEnumerable<int> BisPoints { get; set; }

        public IEnumerable<int> RefusalPoints { get; set; }
    }
}