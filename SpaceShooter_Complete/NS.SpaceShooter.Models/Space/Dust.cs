namespace NS.SpaceShooter.Models.Space
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class Dust : BaseObject
    {
        public Dust(Texture2D texture, Rectangle rectangle) : base (texture, rectangle, 1)
        {
        }
    }
}
