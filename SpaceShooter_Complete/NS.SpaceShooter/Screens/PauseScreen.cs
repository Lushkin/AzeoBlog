namespace NS.SpaceShooter.Screens
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using NS.SpaceShooter.Contracts.Managers;
    using NS.SpaceShooter.Managers;
    using NS.SpaceShooter.Models.Constants;
    using NS.SpaceShooter.Models.Helpers;
    using System.Collections.Generic;
    using System.Linq;

    public class PauseScreen : IGameScreen
    {
        private readonly IGameScreenManager _gameScreenManager;
        private readonly IInputManager _inputManager;
        private readonly ContentManager _contentManager;

        private Texture2D _whitePixelTexture, _backgroudStarTexture;
        private SpriteFont _gameFontBig, _gameFontMedium, _gameFontSmall;
        private SoundEffect _switchSound;
        private Rectangle _backgroudStar, _confirmationMessageRecrangle;
        private List<(int index, string text, Vector2 position)> _menuList;
        private List<(int index, string text, Vector2 position)> _confirmationList;
        private int _menuSelectedIndex, _confirmationSelectedIndex;
        private bool _isQuitTapped;

        public PauseScreen(IGameScreenManager gameScreenManager, ContentManager contentManager)
        {
            _gameScreenManager = gameScreenManager;
            _contentManager = contentManager;
            _inputManager = new InputManager(_gameScreenManager);
        }

        public bool IsPaused { get; private set; }

        public void Init(ContentManager contentManager)
        {
            LoadContent();
            _backgroudStar = new Rectangle(0, 0, _gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight);
            InitMenu();
            InitConfirmationMessage();

            _isQuitTapped = false;
        }

        public void HandleInput(GameTime gameTime)
        {
            var buttonSound = _switchSound.CreateInstance();
            buttonSound.Volume = _gameScreenManager.SoundVolume;
            buttonSound.IsLooped = false;

            if (_inputManager.IsTapped(Keys.Escape))
            {
                if (!_isQuitTapped)
                    _gameScreenManager.PopScreen();
                else
                    _isQuitTapped = !_isQuitTapped;
            }

            if (!_isQuitTapped)
            {
                if (_inputManager.IsTapped(Keys.Down) && _menuSelectedIndex < _menuList.Count() - 1)
                {
                    buttonSound.Play();
                    _menuSelectedIndex++;
                }

                if (_inputManager.IsTapped(Keys.Up) && _menuSelectedIndex > 0)
                {
                    buttonSound.Play();
                    _menuSelectedIndex--;
                }
            }
            else
            {
                if (_inputManager.IsTapped(Keys.Right) && _confirmationSelectedIndex < _confirmationList.Count() - 2)
                {
                    buttonSound.Play();
                    _confirmationSelectedIndex++;
                }

                if (_inputManager.IsTapped(Keys.Left) && _confirmationSelectedIndex > 0)
                {
                    buttonSound.Play();
                    _confirmationSelectedIndex--;
                }
            }

            if (_inputManager.IsTapped(Keys.Enter))
                ExecuteMenuAction();
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _gameScreenManager.GraphicsDevice.Clear(new Color(2, 3, 19));
            spriteBatch.Draw(_backgroudStarTexture, _backgroudStar, Color.White);

            foreach (var button in _menuList)
            {
                spriteBatch.DrawString(_gameFontMedium, button.text, button.position, _menuSelectedIndex == button.index ? GameConstants.MenuSelectedButtonColor : GameConstants.MenuButtonColor);
            }

            if(_isQuitTapped)
            {
                spriteBatch.Draw(_whitePixelTexture, _confirmationMessageRecrangle, Color.Black * 0.9f);
                foreach (var button in _confirmationList)
                {
                    if(button.index == -1)
                        spriteBatch.DrawString(_gameFontMedium, button.text, button.position, GameConstants.TextColor);
                    else
                        spriteBatch.DrawString(_gameFontMedium, button.text, button.position, _confirmationSelectedIndex == button.index ? GameConstants.MenuSelectedButtonColor : GameConstants.MenuButtonColor);
                }
            }

        }

        private void LoadContent()
        {
            _whitePixelTexture = _contentManager.Load<Texture2D>("Images/WhitePixel");
            _backgroudStarTexture = _contentManager.Load<Texture2D>("Images/Miscelaneous/Pause_Background");

            _gameFontBig = _contentManager.Load<SpriteFont>("Fonts/GameFontBig");
            _gameFontMedium = _contentManager.Load<SpriteFont>("Fonts/GameFontMedium");
            _gameFontSmall = _contentManager.Load<SpriteFont>("Fonts/GameFontSmall");

            _switchSound = _contentManager.Load<SoundEffect>("Audio/Sounds/Menu_Button");
        }

        public void Pause()
        {
            IsPaused = true;
        }

        public void Resume()
        {
            IsPaused = false;
        }

        public void ChangeBetweenScreens()
        {
        }

        public void Dispose()
        {
        }

        private void InitMenu()
        {
            _menuSelectedIndex = 0;
            var menuButtonMargin = 15;
            var menuInitialPosition = CommonScreenTextConstants.Pause.Title.ToCenter(_gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight, _gameFontBig);
            _menuList = new List<(int index, string text, Vector2 position)>();
            _menuList.Add((
                0,
                CommonScreenTextConstants.Pause.Resume,
                new Vector2(GetMenuButtonXPosition(CommonScreenTextConstants.Pause.Resume), menuInitialPosition.Y)));
            _menuList.Add((
                1,
                CommonScreenTextConstants.Pause.Options,
                new Vector2(GetMenuButtonXPosition(CommonScreenTextConstants.Pause.Options),
                menuInitialPosition.Y + _gameFontMedium.MeasureString(CommonScreenTextConstants.Pause.Options).Y + menuButtonMargin)));
            _menuList.Add((
                2,
                CommonScreenTextConstants.Pause.Quit,
                new Vector2(GetMenuButtonXPosition(CommonScreenTextConstants.Pause.Quit),
                menuInitialPosition.Y + (_gameFontMedium.MeasureString(CommonScreenTextConstants.Pause.Quit).Y + menuButtonMargin) * 2)));
        }

        private void InitConfirmationMessage()
        {
            _confirmationSelectedIndex = 1;
            var tempRectangle = new Rectangle(0, 0, 700, 200);
            _confirmationMessageRecrangle = tempRectangle.ToCenter(_gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight);

            _confirmationList = new List<(int index, string text, Vector2 position)>();
            _confirmationList.Add((
                -1,
                CommonScreenTextConstants.Pause.QuitConfirmMessage,
                CommonScreenTextConstants.Pause.QuitConfirmMessage.ToTopCenterWithMargin(_confirmationMessageRecrangle, _gameFontMedium)));
            _confirmationList.Add((
                0,
                CommonScreenTextConstants.Yes,
                CommonScreenTextConstants.Yes.ToBottomLeftWithMargin(_confirmationMessageRecrangle, _gameFontMedium)));
            _confirmationList.Add((
                1,
                CommonScreenTextConstants.No,
                CommonScreenTextConstants.No.ToBottomRightWithMargin(_confirmationMessageRecrangle, _gameFontMedium)));
        }

        private float GetMenuButtonXPosition(string text)
        {
            return text.GetXCenter(_gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight, _gameFontMedium);
        }

        private void ExecuteMenuAction()
        {
            if (_isQuitTapped)
            {
                switch (_confirmationSelectedIndex)
                {
                    case 0: //Yes
                        _gameScreenManager.ChangeScreen(new MenuScreen(_gameScreenManager, _contentManager, false));
                        break;
                    case 1: //No
                        _isQuitTapped = false;
                        break;
                }
            }
            else
            {
                switch (_menuSelectedIndex)
                {
                    case 0: //Resume
                        _gameScreenManager.PopScreen();
                        break;
                    case 1: //Option
                        _gameScreenManager.PushScreen(new OptionScreen(_gameScreenManager, _contentManager));
                        break;
                    case 2: //Quit
                        _isQuitTapped = true;
                        break;
                }
            }
        }
    }
}
