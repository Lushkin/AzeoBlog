namespace NS.SpaceShooter.Models.Game
{
    using System;

    public class Score
    {
        public Guid PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int Points { get; set; }
        public DateTime Date { get; set; }
    }
}
