namespace NS.SpaceShooter.Screens
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework.Media;
    using NS.SpaceShooter.Contracts.Managers;
    using NS.SpaceShooter.Contracts.Services;
    using NS.SpaceShooter.Managers;
    using NS.SpaceShooter.Models.Constants;
    using NS.SpaceShooter.Models.Helpers;
    using NS.SpaceShooter.Services;
    using System.Collections.Generic;
    using System.Linq;

    public class PlayerScreen : IGameScreen
    {
        private readonly IGameScreenManager _gameScreenManager;
        private readonly IInputManager _inputManager;
        private readonly IStorageService _storageService;
        private readonly ContentManager _contentManager;

        private Texture2D _whitePixelTexture, _backgroundTexture;
        private Rectangle _darkOverlayRectangle, _backgroundRectangle, _containerRectangle, _inputRectangle, _cursorRectangle, _confirmationMessageRecrangle;
        private SpriteFont _textFontSmall, _textFontMedium;
        private Vector2 _namePosition;
        private SoundEffect _switchSound;
        private List<(int index, string text, Vector2 position)> _confirmationList;
        private int _confirmationSelectedIndex;
        private string _playerName;
        private bool _nameInError, _displayConfirmation;

        public PlayerScreen(IGameScreenManager gameScreenManager, ContentManager contentManager)
        {
            _gameScreenManager = gameScreenManager;
            _contentManager = contentManager;
            _inputManager = new InputManager(_gameScreenManager);
            _storageService = new StorageService();
        }

        public bool IsPaused { get; private set; }

        public void Init(ContentManager contentManager)
        {
            LoadContent();

            _playerName = _gameScreenManager.Player.Name ?? string.Empty;
            _nameInError =  _displayConfirmation = false;
            _backgroundRectangle = _darkOverlayRectangle = new Rectangle(0, 0, _gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight);
            _containerRectangle = new Rectangle(0, 0, _gameScreenManager.GameScreenWidth / 2, _gameScreenManager.GameScreenHeight / 3).ToCenter(_backgroundRectangle);
            _inputRectangle = new Rectangle(0, 0, _containerRectangle.Width - 80, _containerRectangle.Height / 4).ToTopCenterWithMargin(_containerRectangle, 40);
            _namePosition = _playerName.ToCenterLeftWithMargin(_inputRectangle, _textFontMedium, 20);
            _cursorRectangle = new Rectangle(0, 0, 2, _inputRectangle.Height - 40).ToCenterLeftWithMargin(_inputRectangle, 20);
            InitConfirmationMessage();
        }

        public void HandleInput(GameTime gameTime)
        {
            var buttonSound = _switchSound.CreateInstance();
            buttonSound.Volume = _gameScreenManager.SoundVolume;
            buttonSound.IsLooped = false;

            if (_displayConfirmation)
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

                if (_inputManager.IsTapped(Keys.Enter))
                    ExecuteMenuAction();
            }
            else
            {
                if (_inputManager.IsTapped(Keys.Enter))
                    _displayConfirmation = true;

                if (_inputManager.IsTapped(Keys.Back) && _playerName.Length > 0)
                    _playerName = _playerName.Remove(_playerName.Length - 1, 1);

                if (_playerName.Length < 30)
                {
                    var letter = GetLetter();
                    if (!string.IsNullOrWhiteSpace(letter))
                        _playerName += letter;
                }

                if (_inputManager.IsTapped(Keys.Escape))
                {
                    if (_displayConfirmation)
                        _displayConfirmation = !_displayConfirmation;
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            _namePosition = _playerName.ToCenterLeftWithMargin(_inputRectangle, _textFontMedium, 20);
            if (!string.IsNullOrWhiteSpace(_playerName))
                _cursorRectangle = new Rectangle((int)_namePosition.X + (int)_textFontMedium.MeasureString(_playerName).X, (int)_namePosition.Y, 2, (int)_textFontMedium.MeasureString(_playerName).Y);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_backgroundTexture, _backgroundRectangle, Color.White);
            spriteBatch.Draw(_whitePixelTexture, _darkOverlayRectangle, Color.Black * 0.7f);
            spriteBatch.Draw(_whitePixelTexture, _containerRectangle, Color.Black * 0.9f);
            spriteBatch.Draw(_whitePixelTexture, _inputRectangle, Color.White);
            spriteBatch.DrawString(_textFontSmall, CommonScreenTextConstants.PlayerSettings.PlayerName,
                CommonScreenTextConstants.PlayerSettings.PlayerName.ToBottomCenterWithMargin(_containerRectangle, _textFontSmall, 40), GameConstants.TextColor);
            if (_nameInError)
                spriteBatch.DrawString(_textFontSmall, CommonScreenTextConstants.PlayerSettings.EmptyNameErrorMessage,
                    CommonScreenTextConstants.PlayerSettings.EmptyNameErrorMessage.ToCenter(_containerRectangle, _textFontSmall), GameConstants.TextErrorColor);

            spriteBatch.DrawString(_textFontMedium, _playerName,
                _playerName.ToCenterLeftWithMargin(_inputRectangle, _textFontMedium, 20), GameConstants.TextNegativeColor);

            spriteBatch.Draw(_whitePixelTexture, _cursorRectangle, GameConstants.TextNegativeColor);

            if (_displayConfirmation)
            {
                spriteBatch.Draw(_whitePixelTexture, _confirmationMessageRecrangle, Color.Black);
                foreach (var button in _confirmationList)
                {
                    if (button.index == -1)
                        spriteBatch.DrawString(_textFontMedium, button.text, button.position, GameConstants.TextColor);
                    else
                        spriteBatch.DrawString(_textFontMedium, button.text, button.position, _confirmationSelectedIndex == button.index ? GameConstants.MenuSelectedButtonColor : GameConstants.MenuButtonColor);
                }
            }
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
            _textFontSmall = _contentManager.Load<SpriteFont>("Fonts/GameFontSmall");
            _textFontMedium = _contentManager.Load<SpriteFont>("Fonts/GameFontMedium");
            _switchSound = _contentManager.Load<SoundEffect>("Audio/Sounds/Menu_Button");
        }

        private void SaveName()
        {
            if (string.IsNullOrWhiteSpace(_playerName))
                _nameInError = true;
            else
            {
                _nameInError = false;
                _gameScreenManager.Player.Name = _playerName;
                _storageService.Save(_gameScreenManager.Player);
                _gameScreenManager.PopScreen();
            }
        }

        private void InitConfirmationMessage()
        {
            _confirmationSelectedIndex = 1;
            var tempRectangle = new Rectangle(0, 0, 750, 200);
            _confirmationMessageRecrangle = tempRectangle.ToCenter(_gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight);

            _confirmationList = new List<(int index, string text, Vector2 position)>();
            _confirmationList.Add((
                -1,
                CommonScreenTextConstants.PlayerSettings.ConfirmationMessage,
                CommonScreenTextConstants.PlayerSettings.ConfirmationMessage.ToTopCenterWithMargin(_confirmationMessageRecrangle, _textFontMedium)));
            _confirmationList.Add((
                0,
                CommonScreenTextConstants.Yes,
                CommonScreenTextConstants.Yes.ToBottomLeftWithMargin(_confirmationMessageRecrangle, _textFontMedium)));
            _confirmationList.Add((
                1,
                CommonScreenTextConstants.No,
                CommonScreenTextConstants.No.ToBottomRightWithMargin(_confirmationMessageRecrangle, _textFontMedium)));
        }

        private void ExecuteMenuAction()
        {
            switch (_confirmationSelectedIndex)
            {
                case 0: //Yes
                    SaveName();
                    break;
                case 1: //No
                    _displayConfirmation = !_displayConfirmation;
                    break;
            }

        }

        private string GetLetter()
        {
            if (_inputManager.IsTapped(Keys.A))
                return "A";
            if (_inputManager.IsTapped(Keys.B))
                return "B";
            if (_inputManager.IsTapped(Keys.C))
                return "C";
            if (_inputManager.IsTapped(Keys.D))
                return "D";
            if (_inputManager.IsTapped(Keys.E))
                return "E";
            if (_inputManager.IsTapped(Keys.F))
                return "F";
            if (_inputManager.IsTapped(Keys.G))
                return "G";
            if (_inputManager.IsTapped(Keys.H))
                return "H";
            if (_inputManager.IsTapped(Keys.I))
                return "I";
            if (_inputManager.IsTapped(Keys.J))
                return "J";
            if (_inputManager.IsTapped(Keys.K))
                return "K";
            if (_inputManager.IsTapped(Keys.L))
                return "L";
            if (_inputManager.IsTapped(Keys.M))
                return "M";
            if (_inputManager.IsTapped(Keys.N))
                return "N";
            if (_inputManager.IsTapped(Keys.O))
                return "O";
            if (_inputManager.IsTapped(Keys.P))
                return "P";
            if (_inputManager.IsTapped(Keys.Q))
                return "Q";
            if (_inputManager.IsTapped(Keys.R))
                return "R";
            if (_inputManager.IsTapped(Keys.S))
                return "S";
            if (_inputManager.IsTapped(Keys.T))
                return "T";
            if (_inputManager.IsTapped(Keys.U))
                return "U";
            if (_inputManager.IsTapped(Keys.V))
                return "V";
            if (_inputManager.IsTapped(Keys.W))
                return "W";
            if (_inputManager.IsTapped(Keys.X))
                return "X";
            if (_inputManager.IsTapped(Keys.Y))
                return "Y";
            if (_inputManager.IsTapped(Keys.Z))
                return "Z";

            return string.Empty;
        }
    }
}
