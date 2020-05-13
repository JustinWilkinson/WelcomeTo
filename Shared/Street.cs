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

        public List<Estate> GetEstates()
        {
            var estates = new List<Estate>();
            var currentHousesInEstate = new List<int>();
            for (int i = 0; i < Houses.Count; i++)
            {
                var house = Houses[i];
                if (house.Number.HasValue)
                {
                    currentHousesInEstate.Add(house.Index);

                    if (house.FenceBuilt || i == Houses.Count - 1)
                    {
                        estates.Add(new Estate { HouseIndices = new List<int>(currentHousesInEstate), Street = Position, IsFinal = house.InFinalEstate });
                        currentHousesInEstate.Clear();
                    }
                }
                else
                {
                    currentHousesInEstate.Clear();
                }
            }
            return estates.Where(e => e.HouseIndices.Count >=1 && e.HouseIndices.Count <= 6).ToList();
        }
    }
}