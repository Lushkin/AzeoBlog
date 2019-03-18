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

    public class OptionScreen : IGameScreen
    {
        private readonly IGameScreenManager _gameScreenManager;
        private readonly IInputManager _inputManager;
        private readonly ContentManager _contentManager;
        private readonly IStorageService _storageService;

        private Texture2D _whitePixelTexture, _backgroudStarTexture;
        private SpriteFont _gameFontBig, _gameFontMedium, _gameFontSmall;
        private SoundEffect _switchSound;
        private Rectangle _backgroudStar, _containerRecrangle;
        private List<(int index, string text, Vector2 position)> _menuList, _musicChoiceList, _soundChoiceList;
        private int _menuSelectedIndex, _musicChoiceSelectedIndex, _soundChoiceSelectedIndex;

        public OptionScreen(IGameScreenManager gameScreenManager, ContentManager contentManager)
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
            _backgroudStar = new Rectangle(0, 0, _gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight);
            InitMenu();
        }

        public void HandleInput(GameTime gameTime)
        {
            var buttonSound = _switchSound.CreateInstance();
            buttonSound.Volume = _gameScreenManager.SoundVolume;
            buttonSound.IsLooped = false;

            if (_inputManager.IsTapped(Keys.Escape))
                ExitScreen(false);

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

            if(_menuSelectedIndex == 0) // Sound
            {
                if (_inputManager.IsTapped(Keys.Left) && _soundChoiceSelectedIndex > 0)
                {
                    buttonSound.Play();
                    _soundChoiceSelectedIndex--;
                }

                if (_inputManager.IsTapped(Keys.Right) && _soundChoiceSelectedIndex < _soundChoiceList.Count() - 1)
                {
                    buttonSound.Play();
                    _soundChoiceSelectedIndex++;
                }
            }
            else if(_menuSelectedIndex == 1) // Music
            {
                if (_inputManager.IsTapped(Keys.Left) && _musicChoiceSelectedIndex > 0)
                {
                    buttonSound.Play();
                    _musicChoiceSelectedIndex--;
                }

                if (_inputManager.IsTapped(Keys.Right) && _musicChoiceSelectedIndex < _musicChoiceList.Count() - 1)
                {
                    buttonSound.Play();
                    _musicChoiceSelectedIndex++;
                }
            }

            if (_inputManager.IsTapped(Keys.Enter))
                ExecuteMenuAction();
        }

        public void Update(GameTime gameTime)
        {
            MediaPlayer.Volume = _gameScreenManager.MusicVolume;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _gameScreenManager.GraphicsDevice.Clear(new Color(2, 3, 19));
            spriteBatch.Draw(_backgroudStarTexture, _backgroudStar, Color.White);

            spriteBatch.Draw(_whitePixelTexture, _containerRecrangle, Color.Transparent);

            spriteBatch.DrawString(_gameFontMedium, CommonScreenTextConstants.Options.Title, CommonScreenTextConstants.Options.Title.ToTopCenterWithMargin(_containerRecrangle, _gameFontMedium), GameConstants.TextColor);
            foreach (var button in _menuList)
            {
                spriteBatch.DrawString(_gameFontMedium, button.text, button.position, _menuSelectedIndex == button.index ? GameConstants.MenuSelectedButtonColor : GameConstants.MenuButtonColor);
            }

            foreach(var musicChoice in _musicChoiceList)
            {
                spriteBatch.DrawString(_gameFontMedium, musicChoice.text, musicChoice.position, _musicChoiceSelectedIndex == musicChoice.index ? GameConstants.MenuSelectedButtonColor : GameConstants.MenuButtonColor);
            }

            foreach (var soundChoice in _soundChoiceList)
            {
                spriteBatch.DrawString(_gameFontMedium, soundChoice.text, soundChoice.position, _soundChoiceSelectedIndex == soundChoice.index ? GameConstants.MenuSelectedButtonColor : GameConstants.MenuButtonColor);
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
            _menuList = new List<(int index, string text, Vector2 position)>();
            _menuSelectedIndex = 0;

            _containerRecrangle = new Rectangle(0, 0, 250, 300).ToCenter(_gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight);
            var menuButtonMargin = 15;
            _menuList.Add((
                0,
                CommonScreenTextConstants.Options.Sound,
                new Vector2(CommonScreenTextConstants.Options.Sound.ToCenterLeft(_containerRecrangle, _gameFontMedium).X,
                CommonScreenTextConstants.Options.Sound.ToCenterLeft(_containerRecrangle, _gameFontMedium).Y)));
            _menuList.Add((
                1,
                CommonScreenTextConstants.Options.Music,
                new Vector2(CommonScreenTextConstants.Options.Music.ToCenterLeft(_containerRecrangle, _gameFontMedium).X,
                CommonScreenTextConstants.Options.Music.ToCenterLeft(_containerRecrangle, _gameFontMedium).Y + menuButtonMargin * 3)));
            _menuList.Add((
                2,
                CommonScreenTextConstants.Options.Controls,
                new Vector2(CommonScreenTextConstants.Options.Controls.ToCenter(_containerRecrangle, _gameFontMedium).X,
                CommonScreenTextConstants.Options.Controls.ToCenter(_containerRecrangle, _gameFontMedium).Y + menuButtonMargin * 6)));
            _menuList.Add((
                3,
                CommonScreenTextConstants.Options.Save,
                new Vector2(CommonScreenTextConstants.Options.Save.ToCenter(_containerRecrangle, _gameFontMedium).X,
                CommonScreenTextConstants.Options.Save.ToCenter(_containerRecrangle, _gameFontMedium).Y + menuButtonMargin * 9)));

            InitMusicChoices(menuButtonMargin);
            InitSoundChoices(menuButtonMargin);
        }

        private void InitSoundChoices(int menuButtonMargin)
        {
            _soundChoiceList = new List<(int index, string text, Vector2 position)>();
            _soundChoiceSelectedIndex = GetSettingIndex(true);
            _soundChoiceList.Add((
                0,
                CommonScreenTextConstants.On,
                new Vector2(CommonScreenTextConstants.On.ToCenterRight(_containerRecrangle, _gameFontMedium).X - (_gameFontMedium.MeasureString(CommonScreenTextConstants.Off).X + menuButtonMargin),
                CommonScreenTextConstants.On.ToCenterRight(_containerRecrangle, _gameFontMedium).Y)));
            _soundChoiceList.Add((
                1,
                CommonScreenTextConstants.Off,
                new Vector2(CommonScreenTextConstants.Off.ToCenterRight(_containerRecrangle, _gameFontMedium).X,
                CommonScreenTextConstants.Off.ToCenterRight(_containerRecrangle, _gameFontMedium).Y)));
        }

        private void InitMusicChoices(int menuButtonMargin)
        {
            _musicChoiceList = new List<(int index, string text, Vector2 position)>();
            _musicChoiceSelectedIndex = GetSettingIndex(false);
            _musicChoiceList.Add((
                0,
                CommonScreenTextConstants.On,
                new Vector2(CommonScreenTextConstants.On.ToCenterRight(_containerRecrangle, _gameFontMedium).X - (_gameFontMedium.MeasureString(CommonScreenTextConstants.Off).X + menuButtonMargin),
                CommonScreenTextConstants.On.ToCenterRight(_containerRecrangle, _gameFontMedium).Y + menuButtonMargin * 3)));
            _musicChoiceList.Add((
                1,
                CommonScreenTextConstants.Off,
                new Vector2(CommonScreenTextConstants.Off.ToCenterRight(_containerRecrangle, _gameFontMedium).X,
                CommonScreenTextConstants.Off.ToCenterRight(_containerRecrangle, _gameFontMedium).Y + menuButtonMargin * 3)));
        }

        private float GetMenuButtonXPosition(string text)
        {
            return text.GetXCenter(_gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight, _gameFontMedium);
        }

        private float GetMenuButtonXPosition(string text, Rectangle container)
        {
            return text.GetXCenter(container, _gameFontMedium);
        }

        private void ExecuteMenuAction()
        {
            switch (_menuSelectedIndex)
            {
                case 0: //Sound
                    break;
                case 1: //Music
                    break;
                case 2: //Controls
                    _gameScreenManager.PushScreen(new ControlsScreen(_gameScreenManager, _contentManager));
                    break;
                case 3: //Save
                    ExitScreen(true);
                    break;
            }
        }

        private int GetSettingIndex(bool isSound)
        {
            int index = -1;
            if (isSound)
            {
                if (_gameScreenManager.Player != null && _gameScreenManager.Player.Settings != null)
                    index = _gameScreenManager.Player.Settings.SoundActive ? 0 : 1;
                else
                    index = GameConstants.GameDefaultSoundEnabled ? 0 : 1;
            }
            else
            {
                if (_gameScreenManager.Player != null && _gameScreenManager.Player.Settings != null)
                    index = _gameScreenManager.Player.Settings.MusicActive ? 0 : 1;
                else
                    index = GameConstants.GameDefaultMusicEnabled ? 0 : 1;
            }
            return index;
        }

        private void ExitScreen(bool withSave)
        {
            var isMusicActive = false;
            var isSoundActive = false;
            if (_musicChoiceSelectedIndex == 0)
                isMusicActive = true;
            if (_soundChoiceSelectedIndex == 0)
                isSoundActive = true;

            if (withSave)
            {
                _gameScreenManager.Player.Settings.MusicActive = isMusicActive;
                _gameScreenManager.Player.Settings.SoundActive = isSoundActive;
                _storageService.Save(_gameScreenManager.Player);
            }
            _gameScreenManager.PopScreen();
        }
    }
}
