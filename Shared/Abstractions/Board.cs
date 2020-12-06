using System.Collections.Generic;
using System.Linq;
using WelcomeTo.Shared.Enumerations;

namespace WelcomeTo.Shared.Abstractions
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

        public List<Estate> GetEstates(bool includeFinal = true)
        {
            var allEstates = TopStreet.GetEstates();
            allEstates.AddRange(MiddleStreet.GetEstates());
            allEstates.AddRange(BottomStreet.GetEstates());
            return (includeFinal ? allEstates : allEstates.Where(e => !e.IsFinal)).ToList();
        }
    }
}