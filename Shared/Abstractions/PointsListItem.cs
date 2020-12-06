namespace WelcomeTo.Shared.Abstractions
{
    public record PointsListItem
    {
        public int Points { get; init; }

        public bool IsCovered { get; set; }
    }
}