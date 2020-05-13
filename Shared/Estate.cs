using System.Collections.Generic;
using WelcomeTo.Shared.Enumerations;

namespace WelcomeTo.Shared
{
    public class Estate
    {
        public StreetPosition Street { get; set; }

        public List<int> HouseIndices{ get; set; }

        public bool IsFinal { get; set; }
    }
}