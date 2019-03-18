namespace NS.SpaceShooter.Screens
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework.Media;
    using NS.SpaceShooter.Contracts.Managers;
    using NS.SpaceShooter.Managers;
    using NS.SpaceShooter.Models.Animations;
    using NS.SpaceShooter.Models.Constants;
    using NS.SpaceShooter.Models.Enums;
    using NS.SpaceShooter.Models.Game;
    using NS.SpaceShooter.Models.Helpers;
    using NS.SpaceShooter.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class MenuScreen : IGameScreen
    {
        private readonly IGameScreenManager _gameScreenManager;
        private readonly IInputManager _inputManager;
        private readonly ContentManager _contentManager;
        private readonly bool _firstNavigation;

        private bool _canExitGame, _menuIsDisplayed;
        private Random _random;
        private int _backgroundBigSpeed, _backgroundMediumSpeed, _backgroundSmallSpeed, _planetSpeed, _nebulaSpeed, _dustSpeed, _farStarSpeed, _asteroidSpeed;
        private Rectangle _backgroudStarBig1, _backgroudStarBig2, _backgroudStarMedium1, _backgroudStarMedium2, _backgroudStarSmall1, _backgroudStarSmall2;
        private Texture2D _backgroudStarBigTexture, _backgroudStarMediumTexture, _backgroudStarSmallTexture;
        private Rectangle _planet, _nebula, _dust, _farStar, _asteroid;
        private Texture2D _planetTexture, _nebulaTexture, _dustTexture, _farStarTexture;
        private List<Texture2D> _planetTextures, _nebulaTextures, _dustTextures, _farStarTextures, _asteroidTextures;
        private SimpleAnimation _asteroidAnimation;
        private SpriteFont _gameTitleFontBig, _gameTitleFontMedium, _gameFontMedium, _gameFontSmall;
        private Vector2 _titlePosition, _subTitlePosition;
        private Song _backgroundSong;
        private SoundEffect _menuButtonSwitchSound;
        private float _alphaValue, _alphaIncrement, _animationTime;
        private double _fadeDelay;
        private List<(int index, string text, Vector2 position)> _menuList;
        private int _menuSelectedIndex;

        #region Timer
        private const float _delay = 30; //seconds
        private float _remainingDelay = _delay;
        #endregion

        public MenuScreen(IGameScreenManager gameScreenManager, ContentManager contentManager, bool firstNavigation = true)
        {
            _gameScreenManager = gameScreenManager;
            _contentManager = contentManager;
            _inputManager = new InputManager(_gameScreenManager);
            _firstNavigation = firstNavigation;
        }

        public bool IsPaused { get; private set; }

        public void Init(ContentManager contentManager)
        {
            _menuIsDisplayed = false;
            _planetTextures = new List<Texture2D>();
            _nebulaTextures = new List<Texture2D>();
            _dustTextures = new List<Texture2D>();
            _farStarTextures = new List<Texture2D>();
            _asteroidTextures = new List<Texture2D>();
            LoadContent();

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = _gameScreenManager.MusicVolume;
            MediaPlayer.Play(_backgroundSong);

            _backgroundBigSpeed = 6; _backgroundMediumSpeed = 4; _backgroundSmallSpeed = 2;
            _planetSpeed = 2; _nebulaSpeed = 1; _dustSpeed = 1; _farStarSpeed = 2; _asteroidSpeed = 4;

            _backgroudStarBig1 = new Rectangle(0, 0, _gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight);
            _backgroudStarBig2 = new Rectangle(0, -_gameScreenManager.GameScreenHeight, _gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight); // the Y position initialized above the first big Background
            _backgroudStarMedium1 = new Rectangle(0, 100, _gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight);
            _backgroudStarMedium2 = new Rectangle(0, -_gameScreenManager.GameScreenHeight + 100, _gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight); // the Y position initialized above the first medium Background
            _backgroudStarSmall1 = new Rectangle(0, 200, _gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight);
            _backgroudStarSmall2 = new Rectangle(0, -_gameScreenManager.GameScreenHeight + 200, _gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight); // the Y position initialized above the first small Background

            _planet = GetRandomRectangle(SpaceElement.Planet);
            _planetTexture = GetRandomTexture(SpaceElement.Planet);
            _nebula = GetRandomRectangle(SpaceElement.Nebula);
            _nebulaTexture = GetRandomTexture(SpaceElement.Nebula);
            _dust = GetRandomRectangle(SpaceElement.Dust);
            _dustTexture = GetRandomTexture(SpaceElement.Dust);
            _farStar = GetRandomRectangle(SpaceElement.FarStar);
            _farStarTexture = GetRandomTexture(SpaceElement.FarStar);
            _asteroid = GetRandomRectangle(SpaceElement.Asteroid);
            _asteroidAnimation = new SimpleAnimation(GetRandomTexture(SpaceElement.Asteroid), 0.1f, 18);

            _titlePosition = new Vector2((_gameScreenManager.GameScreenWidth / 2) - (_gameTitleFontBig.MeasureString(CommonScreenTextConstants.Menu.Title).X / 2),
                                         (_gameScreenManager.GameScreenHeight / 2) - (_gameTitleFontBig.MeasureString(CommonScreenTextConstants.Menu.Title).Y));
            _subTitlePosition = new Vector2((_gameScreenManager.GameScreenWidth / 2) - (_gameFontMedium.MeasureString(CommonScreenTextConstants.Menu.SubTitle).X / 2),
                                            (_titlePosition.Y) + _gameTitleFontBig.MeasureString(CommonScreenTextConstants.Menu.Title).Y + 50);
            InitMenu();

            _alphaValue = 1.0f;
            _alphaIncrement = -0.05f;
            _fadeDelay = 0.035;

            _menuIsDisplayed = !_firstNavigation;
        }

        public void HandleInput(GameTime gameTime)
        {
            var buttonSound = _menuButtonSwitchSound.CreateInstance();
            buttonSound.Volume = _gameScreenManager.SoundVolume;
            buttonSound.IsLooped = false;

            //if (_inputManager.IsTapped(Keys.Escape))
            //    _canExitGame = true;

            if (_inputManager.IsTapped(Keys.Space))
            {
                if (!_menuIsDisplayed)
                    _menuIsDisplayed = true;

                _remainingDelay = _delay;
            }

            if (_menuIsDisplayed)
            {
                if (_inputManager.IsTapped(Keys.Down) && _menuSelectedIndex < _menuList.Count() - 1)
                {
                    buttonSound.Play();
                    _menuSelectedIndex++;
                    _remainingDelay = _delay;
                }

                if (_inputManager.IsTapped(Keys.Up) && _menuSelectedIndex > 0)
                {
                    buttonSound.Play();
                    _menuSelectedIndex--;
                    _remainingDelay = _delay;
                }

                if (_inputManager.IsTapped(Keys.Enter) && _menuIsDisplayed)
                    ExecuteMenuAction();
            }
        }

        public void Update(GameTime gameTime)
        {
            if(_menuIsDisplayed)
            {
                if (string.IsNullOrWhiteSpace(_gameScreenManager.Player.Name))
                    _gameScreenManager.PushScreen(new PlayerScreen(_gameScreenManager, _contentManager));
            }

            MoveSpaceElements();
            ReplaceSpaceElements();
            FadeInOut(gameTime);
            ManageMenuDisplay();
            HideMenu(gameTime);
            MediaPlayer.Volume = _gameScreenManager.MusicVolume;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _gameScreenManager.GraphicsDevice.Clear(new Color(2, 3, 19));
            spriteBatch.Draw(_backgroudStarSmallTexture, _backgroudStarSmall1, Color.White);
            spriteBatch.Draw(_backgroudStarSmallTexture, _backgroudStarSmall2, Color.White);
            spriteBatch.Draw(_nebulaTexture, _nebula, Color.White);
            spriteBatch.Draw(_farStarTexture, _farStar, Color.White);
            spriteBatch.Draw(_backgroudStarMediumTexture, _backgroudStarMedium1, Color.White);
            spriteBatch.Draw(_backgroudStarMediumTexture, _backgroudStarMedium2, Color.White);
            spriteBatch.Draw(_dustTexture, _dust, Color.White);
            spriteBatch.Draw(_planetTexture, _planet, Color.White);
            //DrawAsteroid(gameTime, spriteBatch);
            spriteBatch.Draw(_backgroudStarBigTexture, _backgroudStarBig1, Color.White);
            spriteBatch.Draw(_backgroudStarBigTexture, _backgroudStarBig2, Color.White);

            spriteBatch.DrawString(_menuIsDisplayed ? _gameTitleFontMedium : _gameTitleFontBig, CommonScreenTextConstants.Menu.Title, _titlePosition, GameConstants.TextColor);
            if(!_menuIsDisplayed)
                spriteBatch.DrawString(_gameFontMedium, CommonScreenTextConstants.Menu.SubTitle, _subTitlePosition, GameConstants.TextColor * _alphaValue);
            spriteBatch.DrawString(_gameFontSmall,
                CommonScreenTextConstants.Menu.Version,
                CommonScreenTextConstants.Menu.Version.ToBottomCenterWithMargin(_gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight, _gameFontSmall), 
                Color.White);

            if(_menuIsDisplayed)
            {
                foreach(var button in _menuList)
                {
                    spriteBatch.DrawString(_gameFontMedium, button.text, button.position, _menuSelectedIndex == button.index ? GameConstants.MenuSelectedButtonColor : GameConstants.MenuButtonColor);
                }
            }
        }

        public void ChangeBetweenScreens()
        {
            if (_canExitGame)
                _gameScreenManager.Exit();
        }

        public void Pause()
        {
            IsPaused = true;
        }

        public void Resume()
        {
            IsPaused = false;
        }

        public void Dispose()
        {

        }

        private void LoadContent()
        {
            _backgroundSong = _contentManager.Load<Song>("Audio/Musics/Menu_Music");
            _menuButtonSwitchSound = _contentManager.Load<SoundEffect>("Audio/Sounds/Menu_Button");

            _backgroudStarBigTexture = _contentManager.Load<Texture2D>("Images/Space/Background_Stars_Landscape_Big");
            _backgroudStarMediumTexture = _contentManager.Load<Texture2D>("Images/Space/Background_Stars_Landscape_Medium");
            _backgroudStarSmallTexture = _contentManager.Load<Texture2D>("Images/Space/Background_Stars_Landscape_Small");

            _planetTextures.Add(_contentManager.Load<Texture2D>("Images/Planets/Planet_Exo_Blue"));
            _planetTextures.Add(_contentManager.Load<Texture2D>("Images/Planets/Planet_Exo_Green"));
            _planetTextures.Add(_contentManager.Load<Texture2D>("Images/Planets/Planet_Exo_Maroon"));
            _planetTextures.Add(_contentManager.Load<Texture2D>("Images/Planets/Planet_Gaz_Giant"));
            _planetTextures.Add(_contentManager.Load<Texture2D>("Images/Planets/Planet_Ice_Giant"));
            _planetTextures.Add(_contentManager.Load<Texture2D>("Images/Planets/Planet_Red_Giant"));
            _planetTextures.Add(_contentManager.Load<Texture2D>("Images/Planets/Planet_Sun"));

            _nebulaTextures.Add(_contentManager.Load<Texture2D>("Images/Space/Nebula_Cold"));
            _nebulaTextures.Add(_contentManager.Load<Texture2D>("Images/Space/Nebula_Hot"));

            _dustTextures.Add(_contentManager.Load<Texture2D>("Images/Space/Dust_Blue"));
            _dustTextures.Add(_contentManager.Load<Texture2D>("Images/Space/Dust_Violet"));
            _dustTextures.Add(_contentManager.Load<Texture2D>("Images/Space/Dust_Yellow"));

            _farStarTextures.Add(_contentManager.Load<Texture2D>("Images/Space/Far_Star_Big"));
            _farStarTextures.Add(_contentManager.Load<Texture2D>("Images/Space/Far_Star_Small"));

            _asteroidTextures.Add(_contentManager.Load<Texture2D>("Images/Space/Asteroids_Grey"));
            _asteroidTextures.Add(_contentManager.Load<Texture2D>("Images/Space/Asteroids_Red"));

            _gameTitleFontBig = _contentManager.Load<SpriteFont>("Fonts/GameTitleBig");
            _gameTitleFontMedium = _contentManager.Load<SpriteFont>("Fonts/GameTitleMedium");
            _gameFontMedium = _contentManager.Load<SpriteFont>("Fonts/GameFontMedium");
            _gameFontSmall = _contentManager.Load<SpriteFont>("Fonts/GameFontSmall");
        }

        private void MoveSpaceElements()
        {
            _backgroudStarBig1.Y += _backgroundBigSpeed;
            _backgroudStarBig2.Y += _backgroundBigSpeed;
            _backgroudStarMedium1.Y += _backgroundMediumSpeed;
            _backgroudStarMedium2.Y += _backgroundMediumSpeed;
            _backgroudStarSmall1.Y += _backgroundSmallSpeed;
            _backgroudStarSmall2.Y += _backgroundSmallSpeed;

            _planet.Y += _planetSpeed;
            _nebula.Y += _nebulaSpeed;
            _dust.Y += _dustSpeed;
            _farStar.Y += _farStarSpeed;
            _asteroid.Y += _asteroidSpeed;
        }

        private void ReplaceSpaceElements()
        {
            if (_backgroudStarBig1.Y >= _gameScreenManager.GameScreenHeight)
                _backgroudStarBig1.Y = -_gameScreenManager.GameScreenHeight;
            if (_backgroudStarBig2.Y >= _gameScreenManager.GameScreenHeight)
                _backgroudStarBig2.Y = -_gameScreenManager.GameScreenHeight;

            if (_backgroudStarMedium1.Y >= _gameScreenManager.GameScreenHeight)
                _backgroudStarMedium1.Y = -_gameScreenManager.GameScreenHeight;
            if (_backgroudStarMedium2.Y >= _gameScreenManager.GameScreenHeight)
                _backgroudStarMedium2.Y = -_gameScreenManager.GameScreenHeight;

            if (_backgroudStarSmall1.Y >= _gameScreenManager.GameScreenHeight)
                _backgroudStarSmall1.Y = -_gameScreenManager.GameScreenHeight;
            if (_backgroudStarSmall2.Y >= _gameScreenManager.GameScreenHeight)
                _backgroudStarSmall2.Y = -_gameScreenManager.GameScreenHeight;

            if (_planet.Y >= _gameScreenManager.GameScreenHeight)
            {
                _planet = GetRandomRectangle(SpaceElement.Planet);
                _planetTexture = GetRandomTexture(SpaceElement.Planet);
            }

            if (_nebula.Y >= _gameScreenManager.GameScreenHeight)
            {
                _nebula = GetRandomRectangle(SpaceElement.Nebula);
                _nebulaTexture = GetRandomTexture(SpaceElement.Nebula);
            }

            if (_dust.Y >= _gameScreenManager.GameScreenHeight)
            {
                _dust = GetRandomRectangle(SpaceElement.Dust);
                _dustTexture = GetRandomTexture(SpaceElement.Dust);
            }

            if (_farStar.Y >= _gameScreenManager.GameScreenHeight)
            {
                _farStar = GetRandomRectangle(SpaceElement.FarStar);
                _farStarTexture = GetRandomTexture(SpaceElement.FarStar);
            }

            if(_asteroid.Y >= _gameScreenManager.GameScreenHeight)
            {
                _asteroid = GetRandomRectangle(SpaceElement.Asteroid);
                _asteroidAnimation.SpriteSheet = GetRandomTexture(SpaceElement.Asteroid);
            }
        }

        private Rectangle GetRandomRectangle(SpaceElement elementType)
        {
            _random = new Random(DateTime.Now.Millisecond);
            int width = 0, height = 0, xPosition = 0, yPosition = 0;

            switch (elementType)
            {
                case SpaceElement.Planet:
                    width = height = _random.Next(_gameScreenManager.GameScreenHeight / 2, _gameScreenManager.GameScreenHeight * 2);
                    yPosition = _random.Next(-_gameScreenManager.GameScreenHeight * 8, -height * 3);
                    break;
                case SpaceElement.Nebula:
                    width = height = _random.Next(_gameScreenManager.GameScreenHeight, _gameScreenManager.GameScreenHeight * 2);
                    yPosition = _random.Next(-_gameScreenManager.GameScreenHeight * 8, -height * 3);
                    break;
                case SpaceElement.Dust:
                    width = height = _random.Next(_gameScreenManager.GameScreenHeight, _gameScreenManager.GameScreenHeight * 2);
                    yPosition = _random.Next(-_gameScreenManager.GameScreenHeight * 8, -height * 3);
                    break;
                case SpaceElement.FarStar:
                    width = height = _random.Next(5, 50);
                    yPosition = _random.Next(-_gameScreenManager.GameScreenHeight * 8, -height * 3);
                    break;
                case SpaceElement.Asteroid:
                    width = _random.Next(50, 300);
                    height = (int)(width / 1.4);
                    yPosition = _random.Next(-_gameScreenManager.GameScreenHeight * 16, -height * 3);
                    break;
            }

            xPosition = _random.Next(-(width / 2), _gameScreenManager.GameScreenWidth - (width / 2));

            return new Rectangle(xPosition, yPosition, width, height);
        }

        private Texture2D GetRandomTexture(SpaceElement elementType)
        {
            _random = new Random(DateTime.Now.Millisecond);
            int index;

            switch(elementType)
            {
                case SpaceElement.Planet:
                    index = _random.Next(0, _planetTextures.Count - 1);
                    return _planetTextures.ElementAt(index);
                case SpaceElement.Dust:
                    index = _random.Next(0, _dustTextures.Count - 1);
                    return _dustTextures.ElementAt(index);
                case SpaceElement.FarStar:
                    index = _random.Next(0, _farStarTextures.Count - 1);
                    return _farStarTextures.ElementAt(index);
                case SpaceElement.Nebula:
                    index = _random.Next(0, _nebulaTextures.Count - 1);
                    return _nebulaTextures.ElementAt(index);
                case SpaceElement.Asteroid:
                    index = _random.Next(0, _asteroidTextures.Count - 1);
                    return _asteroidTextures.ElementAt(index);
                default:
                    return null;
            }
        }

        private void FadeInOut(GameTime gameTime)
        {
            if (!_menuIsDisplayed)
            {
                _fadeDelay -= gameTime.ElapsedGameTime.TotalSeconds;
                if (_fadeDelay <= 0)
                {
                    //Reset the Fade delay
                    _fadeDelay = .035;
                    _alphaValue += _alphaIncrement;

                    if (_alphaValue >= 1.0f || _alphaValue <= 0.0f)
                        _alphaIncrement *= -1;
                }
            }
        }

        private void InitMenu()
        {
            _menuSelectedIndex = 0;
            var menuButtonMargin = 15;
            var menuInitialPosition = CommonScreenTextConstants.Menu.Play.ToCenter(_gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight, _gameFontMedium);
            _menuList = new List<(int index, string text, Vector2 position)>();
            _menuList.Add((
                0,
                CommonScreenTextConstants.Menu.Play,
                new Vector2(GetMenuButtonXPosition(CommonScreenTextConstants.Menu.Play), menuInitialPosition.Y)));
            _menuList.Add((
                1,
                CommonScreenTextConstants.Menu.Options,
                new Vector2(GetMenuButtonXPosition(CommonScreenTextConstants.Menu.Options),
                menuInitialPosition.Y + _gameFontMedium.MeasureString(CommonScreenTextConstants.Menu.Play).Y + menuButtonMargin)));
            _menuList.Add((
                2,
                CommonScreenTextConstants.Menu.Scores,
                new Vector2(GetMenuButtonXPosition(CommonScreenTextConstants.Menu.Scores),
                menuInitialPosition.Y + (_gameFontMedium.MeasureString(CommonScreenTextConstants.Menu.Play).Y + menuButtonMargin) * 2)));
            _menuList.Add((
                3,
                CommonScreenTextConstants.Menu.Credits,
                new Vector2(GetMenuButtonXPosition(CommonScreenTextConstants.Menu.Credits),
                menuInitialPosition.Y + (_gameFontMedium.MeasureString(CommonScreenTextConstants.Menu.Play).Y + menuButtonMargin) * 3)));
            _menuList.Add((
                4,
                CommonScreenTextConstants.Menu.Quit,
                new Vector2(GetMenuButtonXPosition(CommonScreenTextConstants.Menu.Quit),
                menuInitialPosition.Y + (_gameFontMedium.MeasureString(CommonScreenTextConstants.Menu.Play).Y + menuButtonMargin) * 4)));
        }

        private float GetMenuButtonXPosition(string text)
        {
            return text.GetXCenter(_gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight, _gameFontMedium);
        }

        private void DrawAsteroid(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _animationTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (_animationTime > _asteroidAnimation.FrameTime)
            {
                // Play the next frame in the SpriteSheet
                _asteroidAnimation.FrameIndex++;

                // reset elapsed time
                _animationTime = 0f;
            }

            if (_asteroidAnimation.FrameIndex >= _asteroidAnimation.TotalFrames)
                _asteroidAnimation.FrameIndex = 0;

            // Calculate the source rectangle of the current frame.
            Rectangle source = new Rectangle(_asteroidAnimation.FrameIndex * _asteroidAnimation.FrameWidth, 0, _asteroidAnimation.FrameWidth, _asteroidAnimation.FrameHeight);

            // Draw the current frame.
            spriteBatch.Draw(_asteroidAnimation.SpriteSheet, _asteroid, source, Color.White);
        }

        private void HideMenu(GameTime gameTime)
        {
            if(_menuIsDisplayed)
            {
                var timer = (float)gameTime.ElapsedGameTime.TotalSeconds;
                _remainingDelay -= timer;

                if(_remainingDelay <= 0)
                {
                    _menuIsDisplayed = false;
                    _remainingDelay = _delay;
                }
            }
        }

        private void ManageMenuDisplay()
        {
            if(!_menuIsDisplayed)
                _titlePosition = new Vector2((_gameScreenManager.GameScreenWidth / 2) - (_gameTitleFontBig.MeasureString(CommonScreenTextConstants.Menu.Title).X / 2),
                                         (_gameScreenManager.GameScreenHeight / 2) - (_gameTitleFontBig.MeasureString(CommonScreenTextConstants.Menu.Title).Y));
            else
                _titlePosition = CommonScreenTextConstants.Menu.Title.ToTopCenterWithMargin(_gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight, _gameTitleFontMedium, 150);
        }

        private void ExecuteMenuAction()
        {
            switch(_menuSelectedIndex)
            {
                case 0: //Play
                    MediaPlayer.Stop();
                    _gameScreenManager.ChangeScreen(new GameScreen(_gameScreenManager, _contentManager));
                    break;
                case 1: //Option
                    _gameScreenManager.PushScreen(new OptionScreen(_gameScreenManager, _contentManager));
                    break;
                case 2: //Score
                    _gameScreenManager.PushScreen(new ScoreScreen(_gameScreenManager, _contentManager));
                    break;
                case 3: //Credit
                    MediaPlayer.Stop();
                    _gameScreenManager.ChangeScreen(new CreditScreen(_gameScreenManager, _contentManager));
                    break;
                case 4: //Quit
                    _canExitGame = true;
                    break;
            }
        }
    }
}
