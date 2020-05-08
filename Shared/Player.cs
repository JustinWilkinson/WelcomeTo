namespace WelcomeTo.Shared
{
    public class Player
    {
        public string Name { get; set; }

        public bool IsHost { get; set; }

        public Board Board { get; set; }

        public ScoreSheet ScoreSheet { get; set; }
    }
}