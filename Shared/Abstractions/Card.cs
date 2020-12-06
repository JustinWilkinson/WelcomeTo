using WelcomeTo.Shared.Enumerations;

namespace WelcomeTo.Shared.Abstractions
{
    public record Card
    {
        public int HouseNumber { get; init; }

        public CardType Type { get; init; }
    }
}