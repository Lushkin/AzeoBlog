namespace NS.SpaceShooter.Models.Space
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class Bullet : BaseObject
    {
        public bool CollisionEntered { get; set; }
        public Bullet(Texture2D texture, bool player, Texture2D whitePixel = null) : base(texture, new Rectangle(0, 0, 20, player ? 50 : 20), player ? 10 : 5, whitePixel)
        {
            CollisionEntered = false;
        }

        public Bullet(Texture2D texture, bool player, int speed, Texture2D whitePixel = null) : base(texture, new Rectangle(0, 0, 20, player ? 50 : 20), speed, whitePixel)
        {
            CollisionEntered = false;
        }
    }
}
