namespace NS.SpaceShooter.Models.GamePlay
{
    public class DifficultyStep
    {
        public int ScoreLevel { get; set; }
        public bool IncrementEnnemiesCount { get; set; }
        public bool IcrementEnnemiesSpeed { get; set; }
        public bool IsReached { get; set; }
    }
}
