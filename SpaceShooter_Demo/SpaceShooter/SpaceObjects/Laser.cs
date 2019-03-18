namespace SpaceShooter.SpaceObjects
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class Laser
    {
        public Rectangle Rectangle { get; set; }
        public Texture2D Texture { get; set; }
        public int Speed { get; set; }
        public bool CollisionEntered { get; set; }

        public Laser(Rectangle rectangle, Texture2D texture)
        {
            Rectangle = rectangle;
            Texture = texture;
            Speed = 10;
            CollisionEntered = false;
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
