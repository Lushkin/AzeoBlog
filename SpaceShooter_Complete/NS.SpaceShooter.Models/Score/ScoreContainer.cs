namespace NS.SpaceShooter.Models.Score
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using NS.SpaceShooter.Models.Constants;
    using NS.SpaceShooter.Models.Game;
    using NS.SpaceShooter.Models.Helpers;

    public class ScoreContainer
    {
        private readonly string _title;
        private readonly List<Score> _scores;
        private readonly bool _leftSide;
        private readonly Rectangle _container;

        private SpriteFont _titleFont, _scoreFont;
        private Rectangle _rectangle;
        private Texture2D _whitePixelTexture;


        public ScoreContainer(List<Score> scores, string title, bool leftSide, Rectangle container)
        {
            _scores = scores;
            _title = title;
            _leftSide = leftSide;
            _container = container;
        }

        public void Init()
        {
            if(_leftSide)
                _rectangle = new Rectangle(0, 0, (int)(_container.Width / 2.5), _container.Height - 200).ToCenterLeftWithMargin(_container, 70);
            else
                _rectangle = new Rectangle(0, 0, (int)(_container.Width / 2.5), _container.Height - 200).ToCenterRightWithMargin(_container, 70);
        }

        public void Update(GameTime gameTime)
        {

        }

        public void LoadContent(ContentManager contentManager)
        {
            _titleFont = contentManager.Load<SpriteFont>("Fonts/GameFontMedium");
            _scoreFont = contentManager.Load<SpriteFont>("Fonts/GameFontSmall");
            _whitePixelTexture = contentManager.Load<Texture2D>("Images/WhitePixel");
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_whitePixelTexture, _rectangle, Color.Black * 0.8f);
            spriteBatch.DrawString(
                _titleFont,
                _title,
                _title.ToTopCenterWithMargin(_rectangle, _titleFont, 15),
                GameConstants.TextColor);

            for(int i = 0; i < _scores.Count(); i++)
            {
                string playerName = _scores.ElementAt(i).PlayerName.Length > 25 ? _scores.ElementAt(i).PlayerName.TruncateLongStringWithDots(20) : _scores.ElementAt(i).PlayerName;
                string score = $"{playerName} ({_scores.ElementAt(i).Points} pts)";
                string date = _scores.ElementAt(i).Date.ToString("dd/MM/yyyy");

                spriteBatch.DrawString(
                _scoreFont,
                score,
                new Vector2(
                    score.ToTopLeftWithMargin(_rectangle, _scoreFont, 30).X,
                    score.ToTopLeftWithMargin(_rectangle, _scoreFont, 30).Y + (20 * (i+1))),
                GameConstants.TextColor);

                spriteBatch.DrawString(
                _scoreFont,
                date,
                new Vector2(
                    date.ToTopRightWithMargin(_rectangle, _scoreFont, 30).X,
                    date.ToTopRightWithMargin(_rectangle, _scoreFont, 30).Y + (20 * (i + 1))),
                GameConstants.TextColor);
            }
        }
    }
}
