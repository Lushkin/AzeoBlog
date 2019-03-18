namespace NS.SpaceShooter.Models.Helpers
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;

    public static class DrawStringHelper
    {
        private const int defaultMargin = 10;

        public static float GetXCenter(this string text, int screenWidth, int screenHeight, SpriteFont spriteFont)
        {
            return screenWidth / 2 - spriteFont.MeasureString(text).X / 2;
        }

        public static float GetYCenter(this string text, int screenWidth, int screenHeight, SpriteFont spriteFont)
        {
            return screenHeight / 2 - spriteFont.MeasureString(text).Y / 2;
        }

        public static float GetXCenter(this string text, Rectangle container, SpriteFont spriteFont)
        {
            return (container.X + container.Width) / 2 + container.X / 2 - spriteFont.MeasureString(text).X / 2;
        }

        public static float GetYCenter(this string text, Rectangle container, SpriteFont spriteFont)
        {
            return (container.Y + container.Height) / 2 + container.Y / 2 - spriteFont.MeasureString(text).Y / 2;
        }

        #region With margin
        public static Vector2 ToTopLeftWithMargin(this string text, int screenWidth, int screenHeight, SpriteFont spriteFont, int? margin = null)
        {
            var vector = text.ToTopLeft(screenWidth, screenHeight, spriteFont);
            return new Vector2(vector.X + ((margin ?? defaultMargin)), vector.Y + (margin ?? defaultMargin));
        }

        public static Vector2 ToTopCenterWithMargin(this string text, int screenWidth, int screenHeight, SpriteFont spriteFont, int? margin = null)
        {
            var vector = text.ToTopCenter(screenWidth, screenHeight, spriteFont);
            return new Vector2(vector.X, vector.Y + (margin ?? defaultMargin));
        }

        public static Vector2 ToTopRightWithMargin(this string text, int screenWidth, int screenHeight, SpriteFont spriteFont, int? margin = null)
        {
            var vector = text.ToTopRight(screenWidth, screenHeight, spriteFont);
            return new Vector2(vector.X - ((margin ?? defaultMargin)), vector.Y + ((margin ?? defaultMargin)));
        }

        public static Vector2 ToCenterLeftWithMargin(this string text, int screenWidth, int screenHeight, SpriteFont spriteFont, int? margin = null)
        {
            var vector = text.ToCenterLeft(screenWidth, screenHeight, spriteFont);
            return new Vector2(vector.X + (margin ?? defaultMargin), vector.Y);
        }

        public static Vector2 ToCenterRightWithMargin(this string text, int screenWidth, int screenHeight, SpriteFont spriteFont, int? margin = null)
        {
            var vector = text.ToCenterRight(screenWidth, screenHeight, spriteFont);
            return new Vector2(vector.X - (margin ?? defaultMargin), vector.Y);
        }

        public static Vector2 ToBottomLeftWithMargin(this string text, int screenWidth, int screenHeight, SpriteFont spriteFont, int? margin = null)
        {
            var vector = text.ToBottomLeft(screenWidth, screenHeight, spriteFont);
            return new Vector2(vector.X + (margin ?? defaultMargin), vector.Y - (margin ?? defaultMargin));
        }

        public static Vector2 ToBottomCenterWithMargin(this string text, int screenWidth, int screenHeight, SpriteFont spriteFont, int? margin = null)
        {
            var vector = text.ToBottomCenter(screenWidth, screenHeight, spriteFont);
            return new Vector2(vector.X, vector.Y - (margin ?? defaultMargin));
        }

        public static Vector2 ToBottomRightWithMargin(this string text, int screenWidth, int screenHeight, SpriteFont spriteFont, int? margin = null)
        {
            var vector = text.ToBottomRight(screenWidth, screenHeight, spriteFont);
            return new Vector2(vector.X - (margin ?? defaultMargin), vector.Y - (margin ?? defaultMargin));
        }
        #endregion

        #region With margin in container
        public static Vector2 ToTopLeftWithMargin(this string text, Rectangle container, SpriteFont spriteFont, int? margin = null)
        {
            var vector = text.ToTopLeft(container, spriteFont);
            return new Vector2(vector.X + ((margin ?? defaultMargin)), vector.Y + (margin ?? defaultMargin));
        }

        public static Vector2 ToTopCenterWithMargin(this string text, Rectangle container, SpriteFont spriteFont, int? margin = null)
        {
            var vector = text.ToTopCenter(container, spriteFont);
            return new Vector2(vector.X, vector.Y + (margin ?? defaultMargin));
        }

        public static Vector2 ToTopRightWithMargin(this string text, Rectangle container, SpriteFont spriteFont, int? margin = null)
        {
            var vector = text.ToTopRight(container, spriteFont);
            return new Vector2(vector.X - ((margin ?? defaultMargin)), vector.Y + ((margin ?? defaultMargin)));
        }

        public static Vector2 ToCenterLeftWithMargin(this string text, Rectangle container, SpriteFont spriteFont, int? margin = null)
        {
            var vector = text.ToCenterLeft(container, spriteFont);
            return new Vector2(vector.X + (margin ?? defaultMargin), vector.Y);
        }

        public static Vector2 ToCenterRightWithMargin(this string text, Rectangle container, SpriteFont spriteFont, int? margin = null)
        {
            var vector = text.ToCenterRight(container, spriteFont);
            return new Vector2(vector.X - (margin ?? defaultMargin), vector.Y);
        }

        public static Vector2 ToBottomLeftWithMargin(this string text, Rectangle container, SpriteFont spriteFont, int? margin = null)
        {
            var vector = text.ToBottomLeft(container, spriteFont);
            return new Vector2(vector.X + (margin ?? defaultMargin), vector.Y - (margin ?? defaultMargin));
        }

        public static Vector2 ToBottomCenterWithMargin(this string text, Rectangle container, SpriteFont spriteFont, int? margin = null)
        {
            var vector = text.ToBottomCenter(container, spriteFont);
            return new Vector2(vector.X, vector.Y - (margin ?? defaultMargin));
        }

        public static Vector2 ToBottomRightWithMargin(this string text, Rectangle container, SpriteFont spriteFont, int? margin = null)
        {
            var vector = text.ToBottomRight(container, spriteFont);
            return new Vector2(vector.X - (margin ?? defaultMargin), vector.Y - (margin ?? defaultMargin));
        }
        #endregion

        #region NoMargin
        public static Vector2 ToTopLeft(this string text, int screenWidth, int screenHeight, SpriteFont spriteFont)
        {
            return new Vector2(0, 0);
        }

        public static Vector2 ToTopCenter(this string text, int screenWidth, int screenHeight, SpriteFont spriteFont)
        {
            return new Vector2(screenWidth / 2 - spriteFont.MeasureString(text).X / 2, 0);
        }

        public static Vector2 ToTopRight(this string text, int screenWidth, int screenHeight, SpriteFont spriteFont)
        {
            return new Vector2(screenWidth - spriteFont.MeasureString(text).X, 0);
        }

        public static Vector2 ToCenterLeft(this string text, int screenWidth, int screenHeight, SpriteFont spriteFont)
        {
            return new Vector2(0, screenHeight / 2 - spriteFont.MeasureString(text).Y / 2);
        }

        public static Vector2 ToCenter(this string text, int screenWidth, int screenHeight, SpriteFont spriteFont)
        {
            return new Vector2(screenWidth / 2 - spriteFont.MeasureString(text).X / 2, screenHeight / 2 - spriteFont.MeasureString(text).Y / 2);
        }

        public static Vector2 ToCenterRight(this string text, int screenWidth, int screenHeight, SpriteFont spriteFont)
        {
            return new Vector2(screenWidth - spriteFont.MeasureString(text).X, screenHeight / 2 - spriteFont.MeasureString(text).Y / 2);
        }

        public static Vector2 ToBottomLeft(this string text, int screenWidth, int screenHeight, SpriteFont spriteFont)
        {
            return new Vector2(0, screenHeight - spriteFont.MeasureString(text).Y);
        }

        public static Vector2 ToBottomCenter(this string text, int screenWidth, int screenHeight, SpriteFont spriteFont)
        {
            return new Vector2(screenWidth / 2 - spriteFont.MeasureString(text).X / 2, screenHeight - spriteFont.MeasureString(text).Y);
        }

        public static Vector2 ToBottomRight(this string text, int screenWidth, int screenHeight, SpriteFont spriteFont)
        {
            return new Vector2(screenWidth - spriteFont.MeasureString(text).X, screenHeight - spriteFont.MeasureString(text).Y);
        }
        #endregion

        #region NoMargin in container
        public static Vector2 ToTopLeft(this string text, Rectangle container, SpriteFont spriteFont)
        {
            return new Vector2(container.X, container.Y);
        }

        public static Vector2 ToTopCenter(this string text, Rectangle container, SpriteFont spriteFont)
        {
            return new Vector2((container.X + container.Width) / 2 + container.X / 2 - spriteFont.MeasureString(text).X / 2, container.Y);
        }

        public static Vector2 ToTopRight(this string text, Rectangle container, SpriteFont spriteFont)
        {
            return new Vector2((container.X + container.Width) - spriteFont.MeasureString(text).X, container.Y);
        }

        public static Vector2 ToCenterLeft(this string text, Rectangle container, SpriteFont spriteFont)
        {
            return new Vector2(container.X, (container.Y + container.Height) / 2 + container.Y / 2 - spriteFont.MeasureString(text).Y / 2);
        }

        public static Vector2 ToCenter(this string text, Rectangle container, SpriteFont spriteFont)
        {
            return new Vector2((container.X + container.Width) / 2 + container.X / 2 - spriteFont.MeasureString(text).X / 2, (container.Y + container.Height) / 2 + container.Y / 2 - spriteFont.MeasureString(text).Y / 2);
        }

        public static Vector2 ToCenterRight(this string text, Rectangle container, SpriteFont spriteFont)
        {
            return new Vector2((container.X + container.Width) - spriteFont.MeasureString(text).X, (container.Y + container.Height) / 2  + container.Y / 2 - spriteFont.MeasureString(text).Y / 2);
        }

        public static Vector2 ToBottomLeft(this string text, Rectangle container, SpriteFont spriteFont)
        {
            return new Vector2(container.X, (container.Y + container.Height) - spriteFont.MeasureString(text).Y);
        }

        public static Vector2 ToBottomCenter(this string text, Rectangle container, SpriteFont spriteFont)
        {
            return new Vector2((container.X + container.Width) / 2 + container.X / 2 - spriteFont.MeasureString(text).X / 2, (container.Y + container.Height) - spriteFont.MeasureString(text).Y);
        }

        public static Vector2 ToBottomRight(this string text, Rectangle container, SpriteFont spriteFont)
        {
            return new Vector2((container.X + container.Width) - spriteFont.MeasureString(text).X, (container.Y + container.Height) - spriteFont.MeasureString(text).Y);
        }
        #endregion

        public static string TruncateLongString(this string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            return str.Substring(0, Math.Min(str.Length, maxLength));
        }

        public static string TruncateLongStringWithDots(this string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            return $"{str.Substring(0, Math.Min(str.Length, maxLength))}...";
        }
    }
}
