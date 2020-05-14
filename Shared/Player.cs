﻿using System.Linq;
using WelcomeTo.Shared.Enumerations;

namespace WelcomeTo.Shared
{
    public class Player
    {
        public string Name { get; set; }

        public bool IsHost { get; set; }

        public Board Board { get; set; }

        public ScoreSheet ScoreSheet { get; set; }

        public void ProcessAction(NumberEffectPair selectedNumberEffectPair, SelectedHouse selectedHouse, SelectedHouse selectedBisHouse, SelectedHouse selectedFenceHouse, Park selectedPark, SelectedRealEstateValue selectedRealEstateValue)
        {
            var street = Board.GetStreet(selectedHouse.Street);
            var house = street.Houses[selectedHouse.Index];
            house.Number = selectedHouse.Number;

            if (selectedNumberEffectPair.Effect == CardType.Bis && selectedBisHouse != null)
            {
                var bisHouse = Board.GetStreet(selectedBisHouse.Street).Houses[selectedBisHouse.Index];
                bisHouse.Number = selectedBisHouse.Number;
                bisHouse.IsBis = true;
                ScoreSheet.BisPoints.First(x => !x.IsCovered).IsCovered = true;
            }
            else if (selectedNumberEffectPair.Effect == CardType.Park && selectedPark != null)
            {
                street.Parks.Single(x => x.Points == selectedPark.Points).IsCovered = true;
                switch (street.Position)
                {
                    case StreetPosition.Top:
                        ScoreSheet.TopParks = street.Parks.First(x => !x.IsCovered).Points;
                        break;
                    case StreetPosition.Middle:
                        ScoreSheet.MiddleParks = street.Parks.First(x => !x.IsCovered).Points;
                        break;
                    case StreetPosition.Bottom:
                        ScoreSheet.BottomParks = street.Parks.First(x => !x.IsCovered).Points;
                        break;
                }
            }
            else if (selectedNumberEffectPair.Effect == CardType.TempAgency)
            {
                ScoreSheet.TempAgenciesUsed++;
            }
            else if (selectedNumberEffectPair.Effect == CardType.Pool && house.Pool == PoolType.Unbuilt)
            {
                house.Pool = PoolType.Built;
                ScoreSheet.PoolPoints.First(x => !x.IsCovered).IsCovered = true;
            }
            else if (selectedNumberEffectPair.Effect == CardType.Fence && selectedFenceHouse != null)
            {
                Board.GetStreet(selectedFenceHouse.Street).Houses[selectedFenceHouse.Index].FenceBuilt = true;
            }
            else if (selectedNumberEffectPair.Effect == CardType.RealEstateValue && selectedRealEstateValue != null)
            {
                ScoreSheet.RealEstateValuesTable[selectedRealEstateValue.Size][selectedRealEstateValue.Index].IsCovered = true;
            }
        }
    }
}