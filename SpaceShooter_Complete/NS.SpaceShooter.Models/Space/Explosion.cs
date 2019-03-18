namespace NS.SpaceShooter.Models.Space
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using NS.SpaceShooter.Models.Animations;

    public class Explosion : BaseObject
    {

        public bool HasEnded { get; set; }

        private DoubleAnimation _animation;
        private float _animationTime;

        public Explosion(Texture2D texture, Rectangle rectangle) : base(texture, rectangle, 0)
        {
            _animation = new DoubleAnimation(texture, 0.01f, 8, 8);
            HasEnded = false;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _animationTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (_animationTime > _animation.FrameTime)
            {
                // Play the next frame in the SpriteSheet
                _animation.FrameXIndex++;

                // reset elapsed time
                _animationTime = 0f;
            }

            if (_animation.FrameXIndex >= _animation.TotalXFrames)
            {
                _animation.FrameXIndex = 0;
                _animation.FrameYIndex++;
            }

            if(_animation.FrameYIndex >= _animation.TotalYFrames)
            {
                _animation.FrameXIndex = 0;
                _animation.FrameYIndex = 0;
                HasEnded = true;
            }

            // Calculate the source rectangle of the current frame.
            Rectangle source = new Rectangle(_animation.FrameXIndex * _animation.FrameWidth, _animation.FrameYIndex * _animation.FrameHeight, _animation.FrameWidth, _animation.FrameHeight);

            // Draw the current frame.
            spriteBatch.Draw(_animation.SpriteSheet, Rectangle, source, Color.White);
        }
    }
}
