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

    public class CreditScreen : IGameScreen
    {
        private readonly IGameScreenManager _gameScreenManager;
        private readonly IInputManager _inputManager;
        private readonly ContentManager _contentManager;

        private Texture2D _whitePixelTexture, _backgroundTexture, _creditsTexture;
        private Rectangle _darkOverlayRectangle, _backgroundRectangle, _creditsRectangle, _screenBandTopRectangle, _screenBandBottomRectangle;
        private SpriteFont _textFont;
        private Song _backgroundSong;
        private int _creditSpeed, _creditDefaultSpeed, _creditSpeedFast, _creditSpeedSlow;
        private bool _postCreditMessageIsVisible;

        public CreditScreen(IGameScreenManager gameScreenManager, ContentManager contentManager)
        {
            _gameScreenManager = gameScreenManager;
            _contentManager = contentManager;
            _inputManager = new InputManager(_gameScreenManager);
        }

        public bool IsPaused { get; private set; }

        public void Init(ContentManager contentManager)
        {
            int screenBarHeight = _gameScreenManager.GameScreenHeight / 8;
            _creditSpeedSlow = 1;
            _creditSpeed = _creditDefaultSpeed = 2;
            _creditSpeedFast = 10;
            _postCreditMessageIsVisible = false;

            LoadContent();

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = _gameScreenManager.MusicVolume;
            MediaPlayer.Play(_backgroundSong);

            _backgroundRectangle = _darkOverlayRectangle = new Rectangle(0, 0, _gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight);
            _screenBandTopRectangle = new Rectangle(0, 0, _gameScreenManager.GameScreenWidth, screenBarHeight);
            _screenBandBottomRectangle = new Rectangle(0, _gameScreenManager.GameScreenHeight - screenBarHeight, _gameScreenManager.GameScreenWidth, screenBarHeight);
            InitCreditContentRectangle();
        }

        public void HandleInput(GameTime gameTime)
        {
            if (_inputManager.IsTapped(Keys.Escape))
            {
                MediaPlayer.Stop();
                _gameScreenManager.ChangeScreen(new MenuScreen(_gameScreenManager, _contentManager, false));
            }

            if (_inputManager.IsPressed(Keys.Down))
                _creditSpeed = _creditSpeedFast;

            if (_inputManager.IsPressed(Keys.Up))
                _creditSpeed = _creditSpeedSlow;

            if (_inputManager.IsFree())
                _creditSpeed = _creditDefaultSpeed;
        }


        public void Update(GameTime gameTime)
        {
            if (_creditsRectangle.Y > -_creditsRectangle.Height)
                _creditsRectangle.Y -= _creditSpeed;
            else
                _postCreditMessageIsVisible = true;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_backgroundTexture, _backgroundRectangle, Color.White);
            spriteBatch.Draw(_whitePixelTexture, _darkOverlayRectangle, Color.Black * 0.7f);

            spriteBatch.Draw(_creditsTexture, _creditsRectangle, Color.White);

            spriteBatch.Draw(_whitePixelTexture, _screenBandTopRectangle, Color.Black);
            spriteBatch.Draw(_whitePixelTexture, _screenBandBottomRectangle, Color.Black);
            spriteBatch.DrawString(
                       _textFont,
                       CommonScreenTextConstants.BackNavigationText,
                       CommonScreenTextConstants.BackNavigationText.ToBottomRightWithMargin(_gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight, _textFont),
                       Color.White);

            if(_postCreditMessageIsVisible)
                spriteBatch.DrawString(
                       _textFont,
                       CommonScreenTextConstants.Credits.PostCreditMessage,
                       CommonScreenTextConstants.Credits.PostCreditMessage.ToCenter(_gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight, _textFont),
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
            _creditsTexture = _contentManager.Load<Texture2D>("Images/Credits/Credits");
            _textFont = _contentManager.Load<SpriteFont>("Fonts/GameFontSmall");
            _backgroundSong = _contentManager.Load<Song>("Audio/Musics/Game_Music");
        }

        private void InitCreditContentRectangle()
        {
            int width = (int)(_gameScreenManager.GameScreenWidth / 1.5);
            int height = width * 5;
            _creditsRectangle = new Rectangle(_gameScreenManager.GameScreenWidth / 2 - width / 2, _gameScreenManager.GameScreenHeight, width, height);
        }
    }
}
