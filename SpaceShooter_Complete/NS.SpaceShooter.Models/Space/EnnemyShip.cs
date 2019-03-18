namespace NS.SpaceShooter.Models.Space
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class EnnemyShip : Ship
    {
        public int ScorePoints { get; private set; }
        public int MaxSpeed { get; private set; }
        public double ShotInterval { get; private set; }
        public double CurrentShotInterval { get; set; }

        public EnnemyShip(Texture2D texture, int width, int height, int x, int y, int speed = 1, Texture2D whitePixel = null) : base(texture, width, height, x, y, 3, whitePixel)
        {
            Initialize(speed);
        }

        public EnnemyShip(Texture2D texture, Rectangle rectangle, int speed = 1, Texture2D whitePixel = null) : base(texture, rectangle, 3, whitePixel)
        {
            Initialize(speed);
        }

        private void Initialize(int speed)
        {
            Speed = speed;
            ScorePoints = 3;
            MaxSpeed = 4;
            ShotInterval = CurrentShotInterval = 3f;
        }

    }
}
