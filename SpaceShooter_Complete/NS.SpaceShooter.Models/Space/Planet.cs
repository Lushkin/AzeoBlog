namespace NS.SpaceShooter.Models.Space
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class Planet : BaseObject
    {
        public Planet(Texture2D texture, Rectangle rectangle): base(texture, rectangle, 2)
        {
        }
    }
}
