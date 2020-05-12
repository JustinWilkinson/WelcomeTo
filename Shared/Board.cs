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
    }
}