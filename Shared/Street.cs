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
            var fenceLeft = true;
            for (int i = 0; i < Houses.Count; i++)
            {
                var house = Houses[i];
                if (house.Number.HasValue)
                {
                    if (fenceLeft)
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
                        fenceLeft = house.FenceBuilt;
                    }
                }
                else
                {
                    fenceLeft = house.FenceBuilt;
                    currentHousesInEstate.Clear();
                }
            }
            return estates.Where(e => e.HouseIndices.Count >= 1 && e.HouseIndices.Count <= 6).ToList();
        }

        public IEnumerable<int> GetPossibileNumbersForUnbuiltHouse(House house, NumberEffectPair selectedNumberEffectPair)
        {
            if (house.Number.HasValue || selectedNumberEffectPair == null)
            {
                yield break;
            }

            var builtHouses = Houses.Where(x => x.Number.HasValue && x.Index != house.Index);
            var possibilities = new List<int> { selectedNumberEffectPair.Number };

            if (selectedNumberEffectPair.Effect == CardType.TempAgency)
            {
                possibilities.Add(selectedNumberEffectPair.Number - 2);
                possibilities.Add(selectedNumberEffectPair.Number - 1);
                possibilities.Add(selectedNumberEffectPair.Number + 1);
                possibilities.Add(selectedNumberEffectPair.Number + 2);
            }

            if (!builtHouses.Any())
            {
                foreach (var possibility in possibilities)
                {
                    yield return possibility;
                }
            }
            else
            {
                var nearestHouseLeft = builtHouses.Where(x => x.Index < house.Index).OrderByDescending(x => x.Index).FirstOrDefault();
                var nearestHouseRight = builtHouses.Where(x => x.Index > house.Index).OrderBy(x => x.Index).FirstOrDefault();

                foreach (var possibility in possibilities)
                {
                    if ((nearestHouseLeft == null || possibility > nearestHouseLeft.Number.Value) && (nearestHouseRight == null || possibility < nearestHouseRight.Number.Value))
                    {
                        yield return possibility;
                    }
                }
            }
        }


        public IEnumerable<int> GetPossibleBisNumbersForUnbuiltHouse(House house, SelectedHouse selectedHouse)
        {
            var houseToTheLeft = Houses.SingleOrDefault(x => x.Index == house.Index - 1);
            if (houseToTheLeft != null && !houseToTheLeft.FenceBuilt)
            {
                if (houseToTheLeft.Number.HasValue)
                {
                    yield return houseToTheLeft.Number.Value;
                }
                else if (selectedHouse != null && selectedHouse.Street == Position && selectedHouse.Index == houseToTheLeft.Index)
                {
                    yield return selectedHouse.Number;
                }
            }

            if (!house.FenceBuilt)
            {
                var houseToTheRight = Houses.SingleOrDefault(x => x.Index == house.Index + 1);
                if (houseToTheRight != null)
                {
                    if (houseToTheRight.Number.HasValue)
                    {
                        yield return houseToTheRight.Number.Value;
                    }
                    else if (selectedHouse != null && selectedHouse.Street == Position && selectedHouse.Index == houseToTheRight.Index)
                    {
                        yield return selectedHouse.Number;
                    }
                }
            }

            yield break;
        }
    }
}