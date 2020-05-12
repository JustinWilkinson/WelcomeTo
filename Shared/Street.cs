using System.Collections.Generic;
using WelcomeTo.Shared.Enumerations;

namespace WelcomeTo.Shared
{
    public class Street
    {
        public StreetPosition Position { get; set; }

        public List<House> Houses { get; set; }

        public List<Park> Parks { get; set; }
    }
}