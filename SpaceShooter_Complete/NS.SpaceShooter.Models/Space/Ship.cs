namespace NS.SpaceShooter.Models.Space
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using NS.SpaceShooter.Models.Animations;

    public class Ship
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Rectangle Rectangle { get; private set; }
        public  Rectangle CollisionBox { get; set; }
        public int Speed { get; set; }
        public int Life { get; set; }
        public float Alpha { get; set; }

        private Texture2D _texture, _whitePixelTexture;
        private SimpleAnimation _animation;
        private float _animationTime;
        private const int _xPadding = 8;
        private const int _yPadding = 16;

        public Ship(Texture2D texture, int width, int height, int x, int y, int life, Texture2D whitePixel = null)
        {
            _texture = texture;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Life = life;
            Rectangle = new Rectangle(x, y, width, height);
            Alpha = 1f;
            CollisionBox = new Rectangle(Rectangle.X + _xPadding, Rectangle.Y + _yPadding, Rectangle.Width - _xPadding * 2, Rectangle.Height - _yPadding * 2);
            _whitePixelTexture = whitePixel;
        }

        public Ship(Texture2D texture, Rectangle rectangle, int lives, Texture2D whitePixel = null)
        {
            _texture = texture;
            Width = rectangle.Width;
            Height = rectangle.Height;
            X = rectangle.X;
            Y = rectangle.Y;
            Life = lives;
            Rectangle = rectangle;
            Alpha = 1f;

            CollisionBox = new Rectangle(Rectangle.X + _xPadding, Rectangle.Y + _yPadding, Rectangle.Width - _xPadding * 2, Rectangle.Height - _yPadding * 2);
            _whitePixelTexture = whitePixel;
        }


        public void Init()
        {
            _animation = new SimpleAnimation(_texture, 0.1f, 6);
        }

        public virtual void Update(GameTime gameTime)
        {
            Rectangle = new Rectangle(X, Y, Width, Height);
            CollisionBox = new Rectangle(Rectangle.X + _xPadding, Rectangle.Y + _yPadding, Rectangle.Width - _xPadding * 2, Rectangle.Height - _yPadding * 2);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _animationTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (_animationTime > _animation.FrameTime)
            {
                // Play the next frame in the SpriteSheet
                _animation.FrameIndex++;

                // reset elapsed time
                _animationTime = 0f;
            }

            if (_animation.FrameIndex >= _animation.TotalFrames)
                _animation.FrameIndex = 0;

            // Calculate the source rectangle of the current frame.
            Rectangle source = new Rectangle(_animation.FrameIndex * _animation.FrameWidth, 0, _animation.FrameWidth, _animation.FrameHeight);

            // Draw the current frame.
            spriteBatch.Draw(_animation.SpriteSheet, Rectangle, source, Color.White * Alpha);

            if (_whitePixelTexture != null)
                spriteBatch.Draw(_whitePixelTexture, CollisionBox, Color.White * 0.3f);
        }

        public void MoveRight()
        {
            X += Speed;
        }

        public void MoveLeft()
        {
            X -= Speed;
        }

        public void MoveUp()
        {
            Y -= Speed;
        }

        public void MoveDown()
        {
            Y += Speed;
        }

        public void Destroy()
        {
            X = Y = Width = Height = 0;
        }
    }
}
