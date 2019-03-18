namespace NS.SpaceShooter.Models.Game
{
    using System;
    using System.Collections.Generic;

    public class Player
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<Score> Scores { get; set; }
        public GameSettings Settings { get; set; }

        public Player()
        {
            Scores = new List<Score>();
        }
    }
}
