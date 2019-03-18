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

    public class ControlsScreen : IGameScreen
    {
        private readonly IGameScreenManager _gameScreenManager;
        private readonly IInputManager _inputManager;
        private readonly ContentManager _contentManager;

        private Texture2D _whitePixelTexture, _backgroundTexture, _controlsTexture;
        private Rectangle _darkOverlayRectangle, _backgroundRectangle, _contolsRectangle;
        private SpriteFont _textFont;

        public ControlsScreen(IGameScreenManager gameScreenManager, ContentManager contentManager)
        {
            _gameScreenManager = gameScreenManager;
            _contentManager = contentManager;
            _inputManager = new InputManager(_gameScreenManager);
        }

        public bool IsPaused { get; private set; }

        public void Init(ContentManager contentManager)
        {
            LoadContent();

            _backgroundRectangle = _darkOverlayRectangle = new Rectangle(0, 0, _gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight);
            _contolsRectangle = new Rectangle(0, 0, 1000, 500).ToCenter(_backgroundRectangle);
        }

        public void HandleInput(GameTime gameTime)
        {
            if (_inputManager.IsTapped(Keys.Escape))
            {
                _gameScreenManager.PopScreen();
            }            
        }


        public void Update(GameTime gameTime)
        {
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_backgroundTexture, _backgroundRectangle, Color.White);
            spriteBatch.Draw(_whitePixelTexture, _darkOverlayRectangle, Color.Black * 0.7f);

            spriteBatch.Draw(_controlsTexture, _contolsRectangle, Color.White);


            spriteBatch.DrawString(
                      _textFont,
                      CommonScreenTextConstants.BackNavigationText,
                      CommonScreenTextConstants.BackNavigationText.ToTopLeftWithMargin(_backgroundRectangle, _textFont),
                      Color.White);
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
            _backgroundTexture = _contentManager.Load<Texture2D>("Images/Backgrounds/Background_Static");
            _controlsTexture = _contentManager.Load<Texture2D>("Images/Miscelaneous/KeyBoard");
            _textFont = _contentManager.Load<SpriteFont>("Fonts/GameFontSmall");
        }
    }
}
