using WelcomeTo.Shared.Enumerations;

namespace WelcomeTo.Shared.Abstractions
{
    public record SelectedHouse
    {
        public StreetPosition Street { get; set; }

        public int Index { get; set; }

        public int Number { get; set; }
    }
}