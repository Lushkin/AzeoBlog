namespace NS.SpaceShooter.Screens
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework.Media;
    using NS.SpaceShooter.Contracts.Managers;
    using NS.SpaceShooter.Managers;
    using NS.SpaceShooter.Models.Constants;
    using NS.SpaceShooter.Models.Helpers;

    public class GameOverScreen : IGameScreen
    {
        private readonly IGameScreenManager _gameScreenManager;
        private readonly IInputManager _inputManager;
        private readonly ContentManager _contentManager;
        private readonly int _score;

        private Texture2D _whitePixelTexture;
        private Rectangle _backgroundRectangle;
        private SpriteFont _gameTitleFontBig, _gameFontMedium, _gameFontSmall;

        public GameOverScreen(IGameScreenManager gameScreenManager, ContentManager contentManager, int score)
        {
            _gameScreenManager = gameScreenManager;
            _contentManager = contentManager;
            _inputManager = new InputManager(_gameScreenManager);
            _score = score;
        }

        public bool IsPaused { get; private set; }

        public void Init(ContentManager contentManager)
        {
            LoadContent();

            _backgroundRectangle = new Rectangle(0, 0, _gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight);
        }

        public void HandleInput(GameTime gameTime)
        {
            if (_inputManager.IsTapped(Keys.Escape))
            {
                MediaPlayer.Stop();
                _gameScreenManager.ChangeScreen(new MenuScreen(_gameScreenManager, _contentManager, false));
            }
        }


        public void Update(GameTime gameTime)
        {
            
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_whitePixelTexture, _backgroundRectangle, Color.Black);
            spriteBatch.DrawString(_gameTitleFontBig, GameScreenTextConstants.GameOver, GameScreenTextConstants.GameOver.ToTopCenterWithMargin(_backgroundRectangle, _gameTitleFontBig, 50), GameConstants.MenuButtonColor);

            string scoreString = $"Score {_score} pts";
            spriteBatch.DrawString(_gameFontMedium, scoreString, scoreString.ToCenter(_backgroundRectangle, _gameFontMedium), GameConstants.TextColor);

            spriteBatch.DrawString(_gameFontSmall, CommonScreenTextConstants.BackNavigationText, CommonScreenTextConstants.BackNavigationText.ToBottomCenterWithMargin(_backgroundRectangle, _gameFontSmall, 50), GameConstants.MenuButtonColor);
        }

        public void ChangeBetweenScreens()
        {
        }

        public void Dispose()
        {
        }

        public void Pause()
        {
        }

        public void Resume()
        {
        }

        private void LoadContent()
        {
            _whitePixelTexture = _contentManager.Load<Texture2D>("Images/WhitePixel");

            _gameTitleFontBig = _contentManager.Load<SpriteFont>("Fonts/GameTitleBig");
            _gameFontMedium = _contentManager.Load<SpriteFont>("Fonts/GameFontMedium");
            _gameFontSmall = _contentManager.Load<SpriteFont>("Fonts/GameFontSmall");
        }
    }
}
