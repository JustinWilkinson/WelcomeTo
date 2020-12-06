using System.Collections.Generic;
using WelcomeTo.Shared.Enumerations;

namespace WelcomeTo.Shared.Abstractions
{
    public record CityPlan
    {
        public PlanType Type { get; init; }

        public List<int> EstateSizes { get; init; }

        public int FirstPoints { get; init; }

        public int BasicPoints { get; init; }

        public bool CompletedByAnyPlayer { get; set; }
    }
}