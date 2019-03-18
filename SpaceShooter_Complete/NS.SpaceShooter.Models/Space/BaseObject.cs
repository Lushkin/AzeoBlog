namespace NS.SpaceShooter.Models.Space
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class BaseObject
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Rectangle Rectangle { get; private set; }
        public Rectangle CollisionBox { get; set; }
        private readonly Texture2D _texture, _whitePixelTexture;
        private readonly int _speed;

        private const int _xPadding = 5;
        private const int _yPadding = 5;

        public BaseObject(Texture2D texture, Rectangle rectangle, int speed, Texture2D whitePixel = null)
        {
            X = rectangle.X;
            Y = rectangle.Y;
            _texture = texture;
            Rectangle = rectangle;
            _speed = speed;
            _whitePixelTexture = whitePixel;
            CollisionBox = new Rectangle(Rectangle.X + _xPadding, Rectangle.Y + _yPadding, Rectangle.Width - _xPadding * 2, Rectangle.Height - _yPadding * 2);
        }

        public void Update(GameTime gameTime)
        {
            Rectangle = new Rectangle(X, Y, Rectangle.Width, Rectangle.Height);
            CollisionBox = new Rectangle(Rectangle.X + _xPadding, Rectangle.Y + _yPadding, Rectangle.Width - _xPadding * 2, Rectangle.Height - _yPadding * 2);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Rectangle, Color.White);
            if (_whitePixelTexture != null)
                spriteBatch.Draw(_whitePixelTexture, CollisionBox, Color.White * 0.3f);
        }

        public void MoveRight()
        {
            X += _speed;
        }

        public void MoveLeft()
        {
            X -= _speed;
        }

        public void MoveUp()
        {
            Y -= _speed;
        }

        public void MoveDown()
        {
            Y += _speed;
        }
    }
}
