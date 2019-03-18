namespace NS.SpaceShooter.Models.Helpers
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;

    public static class DrawHelper
    {
        private const int defaultMargin = 10;

        public static void DrawLine(this Texture2D texture, SpriteBatch priteBatch, Vector2 start, Vector2 end, Color color, int width = 1)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle = (float)Math.Atan2(edge.Y, edge.X);


            priteBatch.Draw(texture,
                new Rectangle(// rectangle defines shape of line and position of start of line
                    (int)start.X,
                    (int)start.Y,
                    (int)edge.Length(), //sb will strech the texture to fill this rectangle
                    width), //width of line, change this to make thicker line
                null,
                color, //colour of line
                angle,     //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                0);
        }

        #region WithMargin
        public static Rectangle ToTopLeftWithMargin(this Rectangle rectangle, int screenWidth, int screenHeight, int? margin = null)
        {
            var rect = ToTopLeft(rectangle, screenWidth, screenHeight);
            return new Rectangle(rect.X + (margin ?? defaultMargin), rect.Y + (margin ?? defaultMargin), rect.Width, rect.Height);
        }

        public static Rectangle ToTopCenterWithMargin(this Rectangle rectangle, int screenWidth, int screenHeight, int? margin = null)
        {
            var rect = ToTopCenter(rectangle, screenWidth, screenHeight);
            return new Rectangle(rect.X, rect.Y + (margin ?? defaultMargin), rect.Width, rect.Height);
        }

        public static Rectangle ToTopRightWithMargin(this Rectangle rectangle, int screenWidth, int screenHeight, int? margin = null)
        {
            var rect = ToTopRight(rectangle, screenWidth, screenHeight);
            return new Rectangle(rect.X - (margin ?? defaultMargin), rect.Y + (margin ?? defaultMargin), rect.Width, rect.Height);
        }

        public static Rectangle ToCenterLeftWithMargin(this Rectangle rectangle, int screenWidth, int screenHeight, int? margin = null)
        {
            var rect = ToCenterLeft(rectangle, screenWidth, screenHeight);
            return new Rectangle(rect.X + (margin ?? defaultMargin), rect.Y, rect.Width, rect.Height);
        }

        public static Rectangle ToCenterRightWithMargin(this Rectangle rectangle, int screenWidth, int screenHeight, int? margin = null)
        {
            var rect = ToCenterRight(rectangle, screenWidth, screenHeight);
            return new Rectangle(rect.X - (margin ?? defaultMargin), rect.Y, rect.Width, rect.Height);
        }

        public static Rectangle ToBottomLeftWithMargin(this Rectangle rectangle, int screenWidth, int screenHeight, int? margin = null)
        {
            var rect = ToBottomLeft(rectangle, screenWidth, screenHeight);
            return new Rectangle(rect.X + (margin ?? defaultMargin), rect.Y - (margin ?? defaultMargin), rect.Width, rect.Height);
        }

        public static Rectangle ToBottomCenterWithMargin(this Rectangle rectangle, int screenWidth, int screenHeight, int? margin = null)
        {
            var rect = ToBottomCenter(rectangle, screenWidth, screenHeight);
            return new Rectangle(rect.X, rect.Y - (margin ?? defaultMargin), rect.Width, rect.Height);
        }

        public static Rectangle ToBottomRightWithMargin(this Rectangle rectangle, int screenWidth, int screenHeight, int? margin = null)
        {
            var rect = ToBottomRight(rectangle, screenWidth, screenHeight);
            return new Rectangle(rect.X - (margin ?? defaultMargin), rect.Y - (margin ?? defaultMargin), rect.Width, rect.Height);
        }
        #endregion

        #region WithMargin in container
        public static Rectangle ToTopLeftWithMargin(this Rectangle rectangle, Rectangle container, int? margin = null)
        {
            var rect = ToTopLeft(rectangle, container);
            return new Rectangle(rect.X + (margin ?? defaultMargin), rect.Y + (margin ?? defaultMargin), rect.Width, rect.Height);
        }

        public static Rectangle ToTopCenterWithMargin(this Rectangle rectangle, Rectangle container, int? margin = null)
        {
            var rect = ToTopCenter(rectangle, container);
            return new Rectangle(rect.X, rect.Y + (margin ?? defaultMargin), rect.Width, rect.Height);
        }

        public static Rectangle ToTopRightWithMargin(this Rectangle rectangle, Rectangle container, int? margin = null)
        {
            var rect = ToTopRight(rectangle, container);
            return new Rectangle(rect.X - (margin ?? defaultMargin), rect.Y + (margin ?? defaultMargin), rect.Width, rect.Height);
        }

        public static Rectangle ToCenterLeftWithMargin(this Rectangle rectangle, Rectangle container, int? margin = null)
        {
            var rect = ToCenterLeft(rectangle, container);
            return new Rectangle(rect.X + (margin ?? defaultMargin), rect.Y, rect.Width, rect.Height);
        }

        public static Rectangle ToCenterRightWithMargin(this Rectangle rectangle, Rectangle container, int? margin = null)
        {
            var rect = ToCenterRight(rectangle, container);
            return new Rectangle(rect.X - (margin ?? defaultMargin), rect.Y, rect.Width, rect.Height);
        }

        public static Rectangle ToBottomLeftWithMargin(this Rectangle rectangle, Rectangle container, int? margin = null)
        {
            var rect = ToBottomLeft(rectangle, container);
            return new Rectangle(rect.X + (margin ?? defaultMargin), rect.Y - (margin ?? defaultMargin), rect.Width, rect.Height);
        }

        public static Rectangle ToBottomCenterWithMargin(this Rectangle rectangle, Rectangle container, int? margin = null)
        {
            var rect = ToBottomCenter(rectangle, container);
            return new Rectangle(rect.X, rect.Y - (margin ?? defaultMargin), rect.Width, rect.Height);
        }

        public static Rectangle ToBottomRightWithMargin(this Rectangle rectangle, Rectangle container, int? margin = null)
        {
            var rect = ToBottomRight(rectangle, container);
            return new Rectangle(rect.X - (margin ?? defaultMargin), rect.Y - (margin ?? defaultMargin), rect.Width, rect.Height);
        }
        #endregion

        #region NoMargin
        public static Rectangle ToTopLeft(this Rectangle rectangle, int screenWidth, int screenHeight)
        {
            return new Rectangle(0, 0, rectangle.Width, rectangle.Height);
        }

        public static Rectangle ToTopCenter(this Rectangle rectangle, int screenWidth, int screenHeight)
        {
            return new Rectangle(screenWidth / 2 - rectangle.Width / 2, 0, rectangle.Width, rectangle.Height);
        }

        public static Rectangle ToTopRight(this Rectangle rectangle, int screenWidth, int screenHeight)
        {
            return new Rectangle(screenWidth - rectangle.Width, 0, rectangle.Width, rectangle.Height);
        }

        public static Rectangle ToCenterLeft(this Rectangle rectangle, int screenWidth, int screenHeight)
        {
            return new Rectangle(0, screenHeight / 2 - rectangle.Height / 2, rectangle.Width, rectangle.Height);
        }

        public static Rectangle ToCenter(this Rectangle rectangle, int screenWidth, int screenHeight)
        {
            return new Rectangle(screenWidth / 2 - rectangle.Width / 2, screenHeight / 2 - rectangle.Height / 2, rectangle.Width, rectangle.Height);
        }

        public static Rectangle ToCenterRight(this Rectangle rectangle, int screenWidth, int screenHeight)
        {
            return new Rectangle(screenWidth - rectangle.Width, screenHeight / 2 - rectangle.Height / 2, rectangle.Width, rectangle.Height);
        }

        public static Rectangle ToBottomLeft(this Rectangle rectangle, int screenWidth, int screenHeight)
        {
            return new Rectangle(0, screenHeight - rectangle.Height, rectangle.Width, rectangle.Height);
        }

        public static Rectangle ToBottomCenter(this Rectangle rectangle, int screenWidth, int screenHeight)
        {
            return new Rectangle(screenWidth / 2 - rectangle.Width / 2, screenHeight - rectangle.Height, rectangle.Width, rectangle.Height);
        }

        public static Rectangle ToBottomRight(this Rectangle rectangle, int screenWidth, int screenHeight)
        {
            return new Rectangle(screenWidth - rectangle.Width, screenHeight - rectangle.Height, rectangle.Width, rectangle.Height);
        }
        #endregion

        #region NoMargin in conainer
        public static Rectangle ToTopLeft(this Rectangle rectangle, Rectangle container)
        {
            return new Rectangle(container.X, container.Y, rectangle.Width, rectangle.Height);
        }

        public static Rectangle ToTopCenter(this Rectangle rectangle, Rectangle container)
        {
            return new Rectangle((container.X + container.Width) / 2 + container.X / 2 - rectangle.Width / 2, container.Y, rectangle.Width, rectangle.Height);
        }

        public static Rectangle ToTopRight(this Rectangle rectangle, Rectangle container)
        {
            return new Rectangle(container.X + container.Width - rectangle.Width, container.Y, rectangle.Width, rectangle.Height);
        }

        public static Rectangle ToCenterLeft(this Rectangle rectangle, Rectangle container)
        {
            return new Rectangle(container.X, (container.Y + container.Height) / 2 + container.Y / 2 - rectangle.Height / 2, rectangle.Width, rectangle.Height);
        }

        public static Rectangle ToCenter(this Rectangle rectangle, Rectangle container)
        {
            return new Rectangle((container.X + container.Width) / 2 + container.X / 2 - rectangle.Width / 2, (container.Y + container.Height) / 2 + container.Y / 2 - rectangle.Height / 2, rectangle.Width, rectangle.Height);
        }

        public static Rectangle ToCenterRight(this Rectangle rectangle, Rectangle container)
        {
            return new Rectangle((container.X + container.Width) - rectangle.Width, (container.Y + container.Height) / 2 + container.Y / 2 - rectangle.Height / 2, rectangle.Width, rectangle.Height);
        }

        public static Rectangle ToBottomLeft(this Rectangle rectangle, Rectangle containert)
        {
            return new Rectangle(containert.X, (containert.Y + containert.Height) - rectangle.Height, rectangle.Width, rectangle.Height);
        }

        public static Rectangle ToBottomCenter(this Rectangle rectangle, Rectangle container)
        {
            return new Rectangle((container.X + container.Width) / 2 + container.X / 2 - rectangle.Width / 2, (container.Y + container.Height) - rectangle.Height, rectangle.Width, rectangle.Height);
        }

        public static Rectangle ToBottomRight(this Rectangle rectangle, Rectangle container)
        {
            return new Rectangle((container.X + container.Width) - rectangle.Width, (container.Y + container.Height) - rectangle.Height, rectangle.Width, rectangle.Height);
        }
        #endregion
    }
}
