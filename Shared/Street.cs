using System.Collections.Generic;
using System.Linq;
using WelcomeTo.Shared.Enumerations;

namespace WelcomeTo.Shared
{
    public class Street
    {
        public StreetPosition Position { get; set; }

        public List<House> Houses { get; set; }

        public List<Park> Parks { get; set; }

        public List<Estate> GetEstates(bool includeFinal = true)
        {
            var estates = new List<Estate>();
            var currentHousesInEstate = new List<int>();
            foreach (var house in Houses.Where(h => (!h.InFinalEstate || h.InFinalEstate && includeFinal)))
            {
                currentHousesInEstate.Add(house.Index);
                if (house.FenceBuilt)
                {
                    estates.Add(new Estate { HouseIndices = currentHousesInEstate, Street = Position, IsFinal = house.InFinalEstate });
                    currentHousesInEstate.Clear();
                }
            }
            return estates;
        }
    }
}