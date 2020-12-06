using System.Collections.Generic;
using WelcomeTo.Shared.Enumerations;

namespace WelcomeTo.Shared.Abstractions
{
    public record Estate
    {
        public StreetPosition Street { get; init; }

        public List<int> HouseIndices { get; init; }

        public bool IsFinal { get; set; }
    }
}