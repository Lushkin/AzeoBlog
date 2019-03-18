namespace NS.SpaceShooter.Models.Helpers
{
    using Microsoft.Xna.Framework;

    public static class RectangleHelper
    {
        public static Rectangle AlignCenter(this Rectangle rectangle, Rectangle target)
        {
            return new Rectangle(target.X + target.Width / 2 - rectangle.Width / 2, target.Y + target.Height / 2 - rectangle.Height / 2, rectangle.Width, rectangle.Height);
        }
    }
}
