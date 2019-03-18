namespace NS.SpaceShooter.Models.Space
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using NS.SpaceShooter.Models.GamePlay;

    public class BonusBox : BaseObject
    {
        public Bonus Bonus { get; set; }
        public bool CollisionEntered { get; set; }

        public BonusBox(Texture2D texture, Rectangle rectangle) : base(texture, rectangle, 3)
        {
            CollisionEntered = false;
        }
    }
}
