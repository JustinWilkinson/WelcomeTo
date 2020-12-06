using WelcomeTo.Shared.Enumerations;

namespace WelcomeTo.Shared.Abstractions
{
    public record SelectedRealEstateValue
    {
        public RealEstateSize Size { get; set; }

        public int Index { get; set; }
    }
}