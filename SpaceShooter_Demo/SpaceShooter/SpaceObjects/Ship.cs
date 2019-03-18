namespace SpaceShooter.SpaceObjects
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class Ship
    {
        public Rectangle Rectangle { get; set; }
        public Texture2D Texture { get; set; }
        public int Speed { get; set; }
        public int Life { get; set; }
        public double ShotInterval { get; set; }
        public double CurrentShotInterval { get; set; }


        public Ship(Rectangle rectangle, int life, int speed, double shotIntervalle)
        {
            Rectangle = rectangle;
            Speed = speed;
            Life = life;
            ShotInterval = CurrentShotInterval = shotIntervalle;
        }

        public void MoveRight()
        {
            Rectangle = new Rectangle(Rectangle.X + Speed, Rectangle.Y, Rectangle.Width, Rectangle.Height);
        }

        public void MoveLeft()
        {
            Rectangle = new Rectangle(Rectangle.X - Speed, Rectangle.Y, Rectangle.Width, Rectangle.Height);
        }

        public void MoveUp()
        {
            Rectangle = new Rectangle(Rectangle.X, Rectangle.Y - Speed, Rectangle.Width, Rectangle.Height);
        }

        public void MoveDown()
        {
            Rectangle = new Rectangle(Rectangle.X, Rectangle.Y + Speed, Rectangle.Width, Rectangle.Height);
        }
    }
}
