using System.Collections.Generic;
using WelcomeTo.Shared.Enumerations;

namespace WelcomeTo.Shared
{
    public class Board
    {
        public string TownName { get; set; }

        public Street TopStreet { get; set; }

        public Street MiddleStreet { get; set; }

        public Street BottomStreet { get; set; }

        public Street GetStreet(StreetPosition street) => street switch
        {
            StreetPosition.Top => TopStreet,
            StreetPosition.Middle => MiddleStreet,
            StreetPosition.Bottom => BottomStreet,
            _ => null
        };

        public List<Estate> GetNonFinalEstates()
        {
            var allNonFinalEstates = TopStreet.GetEstates(false);
            allNonFinalEstates.AddRange(MiddleStreet.GetEstates(false));
            allNonFinalEstates.AddRange(BottomStreet.GetEstates(false));
            return allNonFinalEstates;
        }
    }
}