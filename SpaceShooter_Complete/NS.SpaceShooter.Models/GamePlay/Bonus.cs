namespace NS.SpaceShooter.Models.GamePlay
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using NS.SpaceShooter.Models.Animations;
    using NS.SpaceShooter.Models.Enums;

    public class Bonus
    {
        public bool HasDuration { get; set; }
        public float Duration { get; set; }
        public BonusType Type { get; set; }
        public int Value { get; set; }
        public bool IsActive { get; set; }
        public Texture2D Texture { get; set; }
        public Rectangle Rectangle { get; set; }
        public bool FollowShip { get; set; }
        public bool IsAnimated { get; set; }
        public DoubleAnimation Animation { get; set; }

        private float AnimationTime;

        public Bonus()
        {
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (IsAnimated)
            {
                AnimationTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                while (AnimationTime > Animation.FrameTime)
                {
                    // Play the next frame in the SpriteSheet
                    Animation.FrameXIndex++;

                    // reset elapsed time
                    AnimationTime = 0f;
                }

                if (Animation.FrameXIndex >= Animation.TotalXFrames)
                {
                    Animation.FrameXIndex = 0;
                    Animation.FrameYIndex++;
                }

                if (Animation.FrameYIndex >= Animation.TotalYFrames)
                {
                    Animation.FrameXIndex = 0;
                    Animation.FrameYIndex = 0;
                }

                // Calculate the source rectangle of the current frame.
                Rectangle source = new Rectangle(Animation.FrameXIndex * Animation.FrameWidth, Animation.FrameYIndex * Animation.FrameHeight, Animation.FrameWidth, Animation.FrameHeight);

                // Draw the current frame.
                spriteBatch.Draw(Animation.SpriteSheet, Rectangle, source, Color.White);
            }
            else
            {
                if(Texture != null)
                    spriteBatch.Draw(Texture, Rectangle, Color.White);
            }
        }
    }
}
