using System;
using System.Linq;
using WelcomeTo.Shared.Enumerations;

namespace WelcomeTo.Shared.Abstractions
{
    public record Player
    {
        public string Name { get; set; }

        public bool IsHost { get; set; }

        public Board Board { get; set; }

        public bool HideBoard { get; set; }

        public ScoreSheet ScoreSheet { get; set; }

        public bool CanPerformAction(Turn currentTurn)
        {
            foreach (var pair in currentTurn.GetNumberEffectPairs())
            {
                foreach (StreetPosition streetPoisiton in Enum.GetValues(typeof(StreetPosition)))
                {
                    var street = Board.GetStreet(streetPoisiton);
                    foreach (var house in street.Houses)
                    {
                        if (street.GetPossibileNumbersForUnbuiltHouse(house, pair).Any())
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public void ProcessAction(NumberEffectPair selectedNumberEffectPair, SelectedHouse selectedHouse, SelectedHouse selectedBisHouse, SelectedHouse selectedFenceHouse, Park selectedPark, SelectedRealEstateValue selectedRealEstateValue)
        {
            var street = Board.GetStreet(selectedHouse.Street);
            var house = street.Houses[selectedHouse.Index];
            house.Number = selectedHouse.Number;

            if (selectedNumberEffectPair.Effect == CardType.Bis && selectedBisHouse is not null)
            {
                var bisHouse = Board.GetStreet(selectedBisHouse.Street).Houses[selectedBisHouse.Index];
                bisHouse.Number = selectedBisHouse.Number;
                bisHouse.IsBis = true;
                ScoreSheet.BisPoints.First(x => !x.IsCovered).IsCovered = true;
            }
            else if (selectedNumberEffectPair.Effect == CardType.Park && selectedPark is not null)
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
            else if (selectedNumberEffectPair.Effect == CardType.Fence && selectedFenceHouse is not null)
            {
                Board.GetStreet(selectedFenceHouse.Street).Houses[selectedFenceHouse.Index].FenceBuilt = true;
            }
            else if (selectedNumberEffectPair.Effect == CardType.RealEstateValue && selectedRealEstateValue is not null)
            {
                ScoreSheet.RealEstateValuesTable[selectedRealEstateValue.Size][selectedRealEstateValue.Index].IsCovered = true;
            }
        }

        public void PassTurn()
        {
            var uncoveredRefusals = ScoreSheet.RefusalPoints.Where(r => !r.IsCovered).ToList();
            if (uncoveredRefusals.Count > 1)
            {
                uncoveredRefusals.First(r => !r.IsCovered).IsCovered = true;
            }
        }

        public string CompletedGameMessage()
        {
            var lastRefusalCovered = ScoreSheet.RefusalPoints.Where(r => !r.IsCovered).Count() == 1;
            if (lastRefusalCovered)
            {
                return $"{Name} has had {ScoreSheet.RefusalPoints.Count - 1} permits refused";
            }

            var allCityPlansComplete = ScoreSheet.Plan1 > 0 && ScoreSheet.Plan2 > 0 && ScoreSheet.Plan3 > 0;
            if (allCityPlansComplete)
            {
                return $"{Name} has completed all three city plans";
            }

            foreach (StreetPosition streetPoisiton in Enum.GetValues(typeof(StreetPosition)))
            {
                if (!Board.GetStreet(streetPoisiton).Houses.All(h => h.Number.HasValue))
                {
                    return null;
                }
            }

            return $"{Name} has built every house on their streets";
        }
    }
}