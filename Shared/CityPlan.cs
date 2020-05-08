using System.Collections.Generic;
using WelcomeTo.Shared.Enumerations;

namespace WelcomeTo.Shared
{
    public class CityPlan
    {
        public PlanType Type { get; set; }

        public List<int> EstateSizes { get; set; }

        public int FirstPoints { get; set; }

        public int BasicPoints { get; set; }
    }
}