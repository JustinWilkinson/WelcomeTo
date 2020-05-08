using WelcomeTo.Shared.Enumerations;

namespace WelcomeTo.Shared
{
    public class House
    {
        public int? Number { get; set; }

        public PoolType Pool { get; set; }

        public bool FenceBuilt { get; set; }

        public bool InEstate { get; set; }
    }
}