namespace NS.SpaceShooter.Screens
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using NS.SpaceShooter.Contracts.Managers;
    using NS.SpaceShooter.Managers;
    using NS.SpaceShooter.Models.Space;
    using NS.SpaceShooter.Models.Constants;
    using NS.SpaceShooter.Models.Helpers;
    using System;
    using System.Collections.Generic;
    using NS.SpaceShooter.Models.Enums;
    using System.Linq;
    using Microsoft.Xna.Framework.Media;
    using Microsoft.Xna.Framework.Audio;
    using NS.SpaceShooter.Models.GamePlay;
    using NS.SpaceShooter.Models.Animations;
    using NS.SpaceShooter.Models.Game;
    using NS.SpaceShooter.Contracts.Services;
    using NS.SpaceShooter.Services;
    using NS.SpaceShooter.Models.Entities;
    using System.Threading;

    public class GameScreen : IGameScreen
    {
        private readonly ContentManager _contentManager;
        private readonly IGameScreenManager _gameScreenManager;
        private readonly IInputManager _inputManager;
        private readonly IStorageService _storageService;
        private readonly IAzureStorageService _azureStorageService;

        private int _backgroundBigSpeed, _backgroundMediumSpeed, _backgroundSmallSpeed;
        private Rectangle _backgroudStarBig1, _backgroudStarBig2, _backgroudStarMedium1, _backgroudStarMedium2, _backgroudStarSmall1, _backgroudStarSmall2,
                          _leftArea, _rightArea, _boostContainer, _boostGauge, _lifeContainer, _scoreContainer, _remainingFailContainer;
        private Texture2D _backgroudStarBigTexture, _backgroudStarMediumTexture, 
                          _backgroudStarSmallTexture, _whitePixelTexture, _bulletTexture, 
                          _doubleBulletTexture, _ennemyBulletTexture, _playerShipTexture,
                          _shipShieldTexture, _backShieldTexture, _boostContainerTexture,
                          _bonusContainerTexture, _scoreContainerTexture;
        private List<Texture2D> _planetTextures, _nebulaTextures, _dustTextures, _farStarTextures, _asteroidTextures, _shipTextures, _explosionTextures;
        private Random _random;
        private Song _backgroundSong;
        private SoundEffect _warpEngineSound, _bulletShotSound, _ennemyShotSound, _ennemyImpactSound, _playerImpactSound;
        private SoundEffectInstance _warpEngineSoundInstance;
        private SpriteFont _debugFont, _gameFontSmall;
        private List<BaseObject> _planets, _nebulas, _dusts, _farStars;
        private List<Bullet> _playerBullets, _ennemyBullets;
        private List<Rectangle> _remainingFailsGauge;
        private PlayerShip _playerShip;
        private List<Ship> _ennemieShips;
        private List<Explosion> _explosions;
        private List<BonusBox> _bonusBoxex;
        private List<(BonusType type, Texture2D texture)> _bonusBoxTextures;
        private List<SoundEffect> _explosionSounds;
        private double _shotInterval, _currentShotInterval, _fadeDelay;
        private int _remainingFails, _maxEnnemiesCount, _ennemyCountIncrementation, _ennemyDefaultSpeed, _ennemySpeedIncrementation, _playerScore;
        private List<DifficultyStep> _difficultySteps;
        private int _boostGaugeMaxWidth;
        private List<(int index, Texture2D texture)> _lifeContainerTextures;
        private List<(int index, Rectangle rectangle)> _bonusContainers;
        private bool _isLocalSaving, _isCloudSaving, _isGameOver;
        private float _alphaValue, _alphaIncrement, _animationTime;


        public GameScreen(IGameScreenManager gameScreenManager, ContentManager contentManager)
        {
            _gameScreenManager = gameScreenManager;
            _contentManager = contentManager;
            _inputManager = new InputManager(_gameScreenManager);
            _storageService = new StorageService();
            _azureStorageService = new AzureStorageService();
            _maxEnnemiesCount = 5;
            _remainingFails = 10;
            _ennemyCountIncrementation = 2;
            _ennemySpeedIncrementation = 1;
            _playerScore = 0;
            _ennemyDefaultSpeed = 1;
            _boostGaugeMaxWidth = 144;
            _isLocalSaving = _isCloudSaving = _isGameOver= false;
        }

        public bool IsPaused { get; private set; }

        public void Init(ContentManager contentManager)
        {
            _currentShotInterval = _shotInterval = 0.15;
            _alphaValue = 1.0f;
            _alphaIncrement = -0.05f;
            _fadeDelay = 0.015;

            _planetTextures = new List<Texture2D>();
            _nebulaTextures = new List<Texture2D>();
            _dustTextures = new List<Texture2D>();
            _farStarTextures = new List<Texture2D>();
            _asteroidTextures = new List<Texture2D>();
            _shipTextures = new List<Texture2D>();
            _explosionTextures = new List<Texture2D>();
            _explosionSounds = new List<SoundEffect>();
            _bonusBoxTextures = new List<(BonusType type, Texture2D texture)>();
            _lifeContainerTextures = new List<(int index, Texture2D texture)>();
            _bonusContainers = new List<(int index, Rectangle rectangle)>();
            _remainingFailsGauge = new List<Rectangle>();
            LoadContent();

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = _gameScreenManager.MusicVolume > 0 ? GameConstants.GameMusicVolume : 0;
            MediaPlayer.Play(_backgroundSong);
            _warpEngineSoundInstance = _warpEngineSound.CreateInstance();
            _warpEngineSoundInstance.Volume = _gameScreenManager.SoundVolume > 0 ? GameConstants.GameWrapEngineVolume : 0;
            _warpEngineSoundInstance.IsLooped = true;
            _warpEngineSoundInstance.Play();

            _planets = new List<BaseObject>();
            _nebulas = new List<BaseObject>();
            _dusts = new List<BaseObject>();
            _farStars = new List<BaseObject>();
            _playerBullets = new List<Bullet>();
            _ennemyBullets= new List<Bullet>();

            InitSides();
            InitBackground();
            InitBonusContainers();

            var tempRect = new Rectangle(100, 100, 59, 100);
            _playerShip = new PlayerShip(_playerShipTexture, tempRect.ToBottomCenterWithMargin(_gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight)/*, _whitePixelTexture*/);
            _playerShip.Init();

            _ennemieShips = new List<Ship>();
            _explosions = new List<Explosion>();
            _bonusBoxex = new List<BonusBox>();

            _lifeContainer = new Rectangle(0, 0, 200, 50).ToTopCenterWithMargin(_leftArea, 20);
            _scoreContainer = new Rectangle(0, 0, 200, 50).ToTopCenterWithMargin(_rightArea, 20);

            _boostContainer = new Rectangle(0, 0, 200, 50).ToTopCenterWithMargin(_leftArea, 60);
            _boostGauge = new Rectangle(_boostContainer.X + 10, _boostContainer.Y + 15, GetBoostGaugeWidth(), 20);
            _remainingFailContainer = new Rectangle(0, 0, 200, 50).ToBottomCenterWithMargin(_rightArea, 20);
            InitRemainingFails();
            IniteDifficulty();
        }

        public void HandleInput(GameTime gameTime)
        {

            if (_inputManager.IsTapped(Keys.Escape))
            {
                _gameScreenManager.PushScreen(new PauseScreen(_gameScreenManager, _contentManager));
            }

            if (_playerShip.Life > 0)
            {
                if (_inputManager.IsPressed(Keys.Right) || _inputManager.IsPressed(Keys.D))
                    _playerShip.MoveRight();
                if (_inputManager.IsPressed(Keys.Left) || _inputManager.IsPressed(Keys.Q))
                    _playerShip.MoveLeft();
                if (_inputManager.IsPressed(Keys.Up) || _inputManager.IsPressed(Keys.Z))
                    _playerShip.MoveUp();
                if (_inputManager.IsPressed(Keys.Down) || _inputManager.IsPressed(Keys.S))
                    _playerShip.MoveDown();

                if (_inputManager.IsPressed(Keys.LeftShift))
                    _playerShip.UseBoost();
                if (_inputManager.IsReleased(Keys.LeftShift))
                    _playerShip.StopBoost();

                if (_inputManager.IsTapped(Keys.Space))
                    ShootBullet();
                else if (_inputManager.IsPressed(Keys.Space))
                    ShootBullets(gameTime);

                UseBonus();
            }
        }

        public void Update(GameTime gameTime)
        {
            FadeInOut(gameTime);
            MoveSpaceElements();
            ReplaceSpaceElements();

            CheckSpaceElements();
            CheckPlayerShipPosition();
            CheckCollisions();
            CheckDifficulty();
            CheckRemainingFails();

            UpdateSpaceObjects(_planets, gameTime);
            UpdateSpaceObjects(_nebulas, gameTime);
            UpdateSpaceObjects(_dusts, gameTime);
            UpdateSpaceObjects(_farStars, gameTime);
            UpdateBonusBoxes(gameTime);

            UpdateBullets(_playerBullets, gameTime);
            UpdateBullets(_ennemyBullets, gameTime);

            AddEnnemies();
            UpdateEnnemies(gameTime);
            ShootEnnemyBullet(gameTime);

            CheckAndRemoveShipBonus();
            _playerShip.Update(gameTime);
            _boostGauge = new Rectangle(_boostContainer.X + 10, _boostContainer.Y + 15, GetBoostGaugeWidth(), 20);

            MediaPlayer.Volume = _gameScreenManager.MusicVolume > 0 ? GameConstants.GameMusicVolume : 0;
            _warpEngineSoundInstance.Volume = _gameScreenManager.SoundVolume > 0 ? GameConstants.GameWrapEngineVolume : 0;
            CheckGameOver();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _gameScreenManager.GraphicsDevice.Clear(new Color(2, 3, 19));
            spriteBatch.Draw(_backgroudStarSmallTexture, _backgroudStarSmall1, Color.White);
            spriteBatch.Draw(_backgroudStarSmallTexture, _backgroudStarSmall2, Color.White);
            DrawSpaceObjects(_nebulas, gameTime, spriteBatch);
            DrawSpaceObjects(_farStars, gameTime, spriteBatch);
            spriteBatch.Draw(_backgroudStarMediumTexture, _backgroudStarMedium1, Color.White);
            spriteBatch.Draw(_backgroudStarMediumTexture, _backgroudStarMedium2, Color.White);
            DrawSpaceObjects(_dusts, gameTime, spriteBatch);
            DrawSpaceObjects(_planets, gameTime, spriteBatch);
            spriteBatch.Draw(_backgroudStarBigTexture, _backgroudStarBig1, Color.White);
            spriteBatch.Draw(_backgroudStarBigTexture, _backgroudStarBig2, Color.White);

            DrawBonusBoxes(gameTime, spriteBatch);

            DrawBullets(_playerBullets, gameTime, spriteBatch);
            DrawBullets(_ennemyBullets, gameTime, spriteBatch);
            DrawEnnemies(gameTime, spriteBatch);
            DrawExplosions(gameTime, spriteBatch);
            _playerShip.Draw(gameTime, spriteBatch);

            spriteBatch.Draw(_whitePixelTexture, _leftArea, Color.Black * 0.8f);
            spriteBatch.Draw(_whitePixelTexture, _rightArea, Color.Black * 0.8f);

            if(_playerShip.Life > 0)
                spriteBatch.Draw(_lifeContainerTextures.FirstOrDefault(m => m.index == _playerShip.Life).texture, _lifeContainer, Color.White);
            else
                spriteBatch.Draw(_lifeContainerTextures.FirstOrDefault(m => m.index == 0).texture, _lifeContainer, Color.White);
            spriteBatch.Draw(_boostContainerTexture, _boostContainer, Color.White);
            spriteBatch.Draw(_whitePixelTexture, _boostGauge, new Color(63,194,18));

            spriteBatch.Draw(_scoreContainerTexture, _scoreContainer, Color.White);
            spriteBatch.DrawString(_gameFontSmall, _playerScore.ToString(), _playerScore.ToString().ToCenterLeftWithMargin(_scoreContainer, _gameFontSmall, 20), Color.White);

            DrawBonusContainers(gameTime, spriteBatch);
            DrawRemainingFails(gameTime, spriteBatch);

            // DrawDebug(spriteBatch);
        }

        private void LoadContent()
        {
            _whitePixelTexture = _contentManager.Load<Texture2D>("Images/WhitePixel");

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

            _shipTextures.Add(_contentManager.Load<Texture2D>($"Images/Ships/{GameScreenTextConstants.Ships.SmallShip1Bottom}"));
            _shipTextures.Add(_contentManager.Load<Texture2D>($"Images/Ships/{GameScreenTextConstants.Ships.SmallShip2Bottom}"));
            _shipTextures.Add(_contentManager.Load<Texture2D>($"Images/Ships/{GameScreenTextConstants.Ships.MediumShip1Bottom}"));
            _shipTextures.Add(_contentManager.Load<Texture2D>($"Images/Ships/{GameScreenTextConstants.Ships.MediumShip2Bottom}"));

            _playerShipTexture = _contentManager.Load<Texture2D>($"Images/Ships/{GameScreenTextConstants.Ships.BigShip2Top}");

            _bulletTexture = _contentManager.Load<Texture2D>("Images/Bullets/Projectile_Red");
            _doubleBulletTexture = _contentManager.Load<Texture2D>("Images/Bullets/Projectile_Blue");
            _ennemyBulletTexture = _contentManager.Load<Texture2D>("Images/Bullets/Bullet_Green");

            var exp1 = _contentManager.Load<Texture2D>("Images/Explosions/explosion_3");
            var exp2 = _contentManager.Load<Texture2D>("Images/Explosions/explosion_4");

            for (int i = 0; i < 4; i++)
            {
                _explosionTextures.Add(exp1);
                _explosionTextures.Add(exp2);
            }

            _bonusContainerTexture = _contentManager.Load<Texture2D>("Images/Game/Bonus_Container");
            _bonusBoxTextures.Add((BonusType.AmmoSpeed, _contentManager.Load<Texture2D>("Images/Bonus/Bonus_Ammo_Speed")));
            _bonusBoxTextures.Add((BonusType.AmmoX2, _contentManager.Load<Texture2D>("Images/Bonus/Bonus_Ammo_x2")));
            _bonusBoxTextures.Add((BonusType.BackShield, _contentManager.Load<Texture2D>("Images/Bonus/Bonus_Back_Shield")));
            _bonusBoxTextures.Add((BonusType.Boost, _contentManager.Load<Texture2D>("Images/Bonus/Bonus_Boost")));
            _bonusBoxTextures.Add((BonusType.Health, _contentManager.Load<Texture2D>("Images/Bonus/Bonus_Health")));
            _bonusBoxTextures.Add((BonusType.ShipShield, _contentManager.Load<Texture2D>("Images/Bonus/Bonus_Ship_Shield")));

            _shipShieldTexture = _contentManager.Load<Texture2D>("Images/Bonus/Shield");
            _backShieldTexture = _contentManager.Load<Texture2D>("Images/Bonus/BackShield");

            _boostContainerTexture = _contentManager.Load<Texture2D>("Images/Game/Boost_Container");
            _lifeContainerTextures.Add((0, _contentManager.Load<Texture2D>("Images/Game/Health_0")));
            _lifeContainerTextures.Add((1, _contentManager.Load<Texture2D>("Images/Game/Health_1")));
            _lifeContainerTextures.Add((2, _contentManager.Load<Texture2D>("Images/Game/Health_2")));
            _lifeContainerTextures.Add((3, _contentManager.Load<Texture2D>("Images/Game/Health_3")));
            _lifeContainerTextures.Add((4, _contentManager.Load<Texture2D>("Images/Game/Health_4")));
            _lifeContainerTextures.Add((5, _contentManager.Load<Texture2D>("Images/Game/Health_5")));
            _scoreContainerTexture = _contentManager.Load<Texture2D>("Images/Game/Score_Container");

            _backgroundSong = _contentManager.Load<Song>("Audio/Musics/Game_Music");
            _warpEngineSound = _contentManager.Load<SoundEffect>("Audio/Sounds/Warp_Engine");
            _bulletShotSound = _contentManager.Load<SoundEffect>("Audio/Sounds/Standard_Shot");
            _ennemyShotSound = _contentManager.Load<SoundEffect>("Audio/Sounds/Menu_Button");
            _explosionSounds.Add(_contentManager.Load<SoundEffect>("Audio/Sounds/Explosions/Explosion_1"));
            _explosionSounds.Add(_contentManager.Load<SoundEffect>("Audio/Sounds/Explosions/Explosion_2"));
            _explosionSounds.Add(_contentManager.Load<SoundEffect>("Audio/Sounds/Explosions/Explosion_6"));
            _explosionSounds.Add(_contentManager.Load<SoundEffect>("Audio/Sounds/Explosions/Explosion_8"));
            _explosionSounds.Add(_contentManager.Load<SoundEffect>("Audio/Sounds/Explosions/Explosion_9"));
            _explosionSounds.Add(_contentManager.Load<SoundEffect>("Audio/Sounds/Explosions/Explosion_10"));
            _ennemyImpactSound = _contentManager.Load<SoundEffect>("Audio/Sounds/Explosions/Explosion_3");
            _playerImpactSound = _contentManager.Load<SoundEffect>("Audio/Sounds/Explosions/Explosion_4");

            _gameFontSmall = _contentManager.Load<SpriteFont>("Fonts/GameFontSmall");
            _debugFont = _contentManager.Load<SpriteFont>("Fonts/GameFontSmall");
        }

        public void Pause()
        {
            IsPaused = true;
            MediaPlayer.Pause();
            _warpEngineSoundInstance.Pause();
        }

        public void Resume()
        {
            IsPaused = false;
            MediaPlayer.Resume();
            _warpEngineSoundInstance.Resume();
        }

        public void ChangeBetweenScreens()
        {
        }

        public void Dispose()
        {
        }

        private void UpdateSpaceObjects(List<BaseObject> objects, GameTime gameTime)
        {
            foreach (var obj in objects)
                obj.Update(gameTime);
        }

        private void UpdateBullets(List<Bullet> bullets, GameTime gameTime)
        {
            foreach (var bullet in bullets)
                bullet.Update(gameTime);
        }

        private void UpdateEnnemies(GameTime gameTime)
        {
            foreach (var ennemy in _ennemieShips)
                ennemy.Update(gameTime);
        }

        private void UpdateBonusBoxes(GameTime gameTime)
        {
            foreach (var bonusBox in _bonusBoxex)
                bonusBox.Update(gameTime);
        }

        private void DrawSpaceObjects(List<BaseObject> objects, GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var obj in objects)
                obj.Draw(gameTime, spriteBatch);
        }

        private void DrawBullets(List<Bullet> playerBullets, GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var bullet in playerBullets)
                bullet.Draw(gameTime, spriteBatch);
        }

        private void DrawEnnemies(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var ennemy in _ennemieShips)
                ennemy.Draw(gameTime, spriteBatch);
        }

        private void DrawExplosions(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var explosion in _explosions)
                explosion.Draw(gameTime, spriteBatch);
        }

        private void DrawBonusBoxes(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var bonusBox in _bonusBoxex)
                bonusBox.Draw(gameTime, spriteBatch);
        }

        private void DrawBonusContainers(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach(var container in _bonusContainers)
            {
                spriteBatch.Draw(_bonusContainerTexture, container.rectangle, Color.White);
                spriteBatch.DrawString(_gameFontSmall, container.index.ToString(),
                            container.index.ToString().ToBottomCenterWithMargin(container.rectangle, _gameFontSmall, 6), GameConstants.TextColor);
            }

            foreach(var bonus in _playerShip.Bonuses)
            {
                var rectangle = new Rectangle(0, 0, 50, 50).ToTopCenterWithMargin(_bonusContainers.FirstOrDefault(m => m.index == bonus.index).rectangle, 5);
                spriteBatch.Draw(GetBonusTexture(bonus.bonus.Type), rectangle, Color.White * (bonus.bonus.IsActive ? _alphaValue : 1));
            }
        }

        private void DrawRemainingFails(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_scoreContainerTexture, _remainingFailContainer, Color.White);

            foreach(var rect in _remainingFailsGauge)
            {
                spriteBatch.Draw(_whitePixelTexture, rect, new Color(20, 160, 20));
            }
        }

        private void InitBackground()
        {
            _backgroundBigSpeed = 6; _backgroundMediumSpeed = 4; _backgroundSmallSpeed = 2;

            _backgroudStarBig1 = new Rectangle(0, 0, _gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight);
            _backgroudStarBig2 = new Rectangle(0, -_gameScreenManager.GameScreenHeight, _gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight); // the Y position initialized above the first big Background
            _backgroudStarMedium1 = new Rectangle(0, 100, _gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight);
            _backgroudStarMedium2 = new Rectangle(0, -_gameScreenManager.GameScreenHeight + 100, _gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight); // the Y position initialized above the first medium Background
            _backgroudStarSmall1 = new Rectangle(0, 200, _gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight);
            _backgroudStarSmall2 = new Rectangle(0, -_gameScreenManager.GameScreenHeight + 200, _gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight); // the Y position initialized above the first small Background
        }

        private void InitSides()
        {
            int areaWidth = (int)Math.Round(_gameScreenManager.GameScreenWidth / 4.0);

            _leftArea = new Rectangle(0, 0, areaWidth, _gameScreenManager.GameScreenHeight)
                .ToTopLeft(_gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight);
            _rightArea = new Rectangle(0, 0, areaWidth, _gameScreenManager.GameScreenHeight)
                .ToTopRight(_gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight);
        }

        private void IniteDifficulty()
        {
            _difficultySteps = new List<DifficultyStep>
            {
                new DifficultyStep{ ScoreLevel = 15, IcrementEnnemiesSpeed = false, IncrementEnnemiesCount = true, IsReached = false }, // Ennemies count = 7 Speed = 1
                new DifficultyStep{ ScoreLevel = 30, IcrementEnnemiesSpeed = false, IncrementEnnemiesCount = true, IsReached = false }, // Ennemies count = 9 Speed = 1
                new DifficultyStep{ ScoreLevel = 60, IcrementEnnemiesSpeed = false, IncrementEnnemiesCount = true, IsReached = false }, // Ennemies count = 11 Speed = 1
                new DifficultyStep{ ScoreLevel = 90, IcrementEnnemiesSpeed = true, IncrementEnnemiesCount = true, IsReached = false }, // Ennemies count = 13 Speed = 2
                new DifficultyStep{ ScoreLevel = 300, IcrementEnnemiesSpeed = false, IncrementEnnemiesCount = true, IsReached = false }, // Ennemies count = 15 Speed = 2
                new DifficultyStep{ ScoreLevel = 500, IcrementEnnemiesSpeed = false, IncrementEnnemiesCount = true, IsReached = false }, // Ennemies count = 17 Speed = 2
                new DifficultyStep{ ScoreLevel = 1000, IcrementEnnemiesSpeed = true, IncrementEnnemiesCount = true, IsReached = false }, // Ennemies count = 19 Speed = 3
                new DifficultyStep{ ScoreLevel = 3000, IcrementEnnemiesSpeed = true, IncrementEnnemiesCount = true, IsReached = false }, // Ennemies count = 21 Speed = 4
                new DifficultyStep{ ScoreLevel = 8000, IcrementEnnemiesSpeed = false, IncrementEnnemiesCount = true, IsReached = false }, // Ennemies count = 23 Speed = 4
                new DifficultyStep{ ScoreLevel = 20000, IcrementEnnemiesSpeed = false, IncrementEnnemiesCount = true, IsReached = false }, // Ennemies count = 25 Speed = 4
            };
        }

        private void InitBonusContainers()
        {
            for(int i = 0; i < 5; i++)
            {
                var tempeRect = new Rectangle(0, 0, 60, 92);
                int xPosition = tempeRect.ToBottomLeftWithMargin(_leftArea).X;
                int yPosition = tempeRect.ToBottomLeftWithMargin(_leftArea).Y;

                _bonusContainers.Add((i + 1, new Rectangle(xPosition + tempeRect.Width * i, yPosition, tempeRect.Width, tempeRect.Height)));
            }
        }

        private void InitRemainingFails()
        {
            int width = 16;
            int height = 22;
            int spacing = 2;
            var tempRect = new Rectangle(0, 0, width, height);

            for (int i = 0; i < _remainingFails; i++)
            {
                _remainingFailsGauge.Add(new Rectangle(tempRect.ToCenterLeftWithMargin(_remainingFailContainer, 11).X + ((width + spacing) * i),
                                                       tempRect.ToCenterLeftWithMargin(_remainingFailContainer, 11).Y,
                                                       width, height));
            }
        }

        private void MoveSpaceElements()
        {
            _backgroudStarBig1.Y += _backgroundBigSpeed;
            _backgroudStarBig2.Y += _backgroundBigSpeed;
            _backgroudStarMedium1.Y += _backgroundMediumSpeed;
            _backgroudStarMedium2.Y += _backgroundMediumSpeed;
            _backgroudStarSmall1.Y += _backgroundSmallSpeed;
            _backgroudStarSmall2.Y += _backgroundSmallSpeed;

            foreach (var planet in _planets)
                planet.MoveDown();
            foreach (var nebula in _nebulas)
                nebula.MoveDown();
            foreach (var dust in _dusts)
                dust.MoveDown();
            foreach (var farStar in _farStars)
                farStar.MoveDown();

            foreach (var bullet in _playerBullets)
                bullet.MoveUp();
            foreach (var bullet in _ennemyBullets)
                bullet.MoveDown();

            foreach (var ennemy in _ennemieShips)
                ennemy.MoveDown();

            foreach (var bonusBox in _bonusBoxex)
                bonusBox.MoveDown();
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
        }

        private void CheckSpaceElements()
        {
            if (_planets.Any())
                CheckAndRemove(_planets, Positions.Bottom);
            else
                _planets.Add(new Planet(GetRandomTexture(SpaceElement.Planet), GetRandomRectangle(SpaceElement.Planet)));

            if (_nebulas.Any())
                CheckAndRemove(_nebulas, Positions.Bottom);
            else
                _nebulas.Add(new Nebula(GetRandomTexture(SpaceElement.Nebula), GetRandomRectangle(SpaceElement.Nebula)));

            if (_dusts.Any())
                CheckAndRemove(_dusts, Positions.Bottom);
            else
                _dusts.Add(new Dust(GetRandomTexture(SpaceElement.Dust), GetRandomRectangle(SpaceElement.Dust)));

            if (_farStars.Any())
                CheckAndRemove(_farStars, Positions.Bottom);
            else
                _farStars.Add(new FarStar(GetRandomTexture(SpaceElement.FarStar), GetRandomRectangle(SpaceElement.FarStar)));

            if (_playerBullets.Any())
                CheckAndRemove(_playerBullets, Positions.Top);

            if (_ennemyBullets.Any())
                CheckAndRemove(_ennemyBullets, Positions.Bottom);

            if (_ennemieShips.Any())
                CheckAndRemove(_ennemieShips);

            if (_explosions.Any())
                CheckAndRemove(_explosions);

            if (_bonusBoxex.Any())
                CheckAndRemove(_bonusBoxex);
        }

        private void AddEnnemies()
        {
            if (_ennemieShips.Count() < _maxEnnemiesCount)
            {
                var ennemy = new EnnemyShip(GetRandomTexture(SpaceElement.Ship), GetRandomRectangle(SpaceElement.Ship), _ennemyDefaultSpeed/*, _whitePixelTexture*/);
                ennemy.Init();
                _ennemieShips.Add(ennemy);
            }
        }

        private void AddExplosion(Ship ship)
        {
            int size = ship.Height * 2;
            var texture = GetRandomTexture(SpaceElement.Explosion);
            var rectangle = new Rectangle(ship.X + ship.Width / 2 - size / 2, ship.Y + ship.Height / 2 - size / 2, size, size);

            _explosions.Add(new Explosion(texture, rectangle));
            var sound = GetRandomSound(SpaceElement.Explosion);
            sound.Play();
        }

        private void AddBonus(Ship ship)
        {
            _random = new Random(Guid.NewGuid().GetHashCode());
            var randomValue = _random.Next(0, 1000);
            if (randomValue <= 100)
            {
                var bonus = GetRandomBonus();
                if (bonus != null)
                {

                    int size = 25;
                    var texture = GetBonusTexture(bonus.Type);
                    var rectangle = new Rectangle(ship.X + ship.Width / 2 - size / 2, ship.Y + ship.Height / 2 - size / 2, size, size);

                    var bonusBox = new BonusBox(texture, rectangle);
                    bonusBox.Bonus = bonus;
                    _bonusBoxex.Add(bonusBox);
                }
            }
        }

        private void CheckAndRemove(List<Ship> ships)
        {
            for (int i = 0; i < ships.Count(); i++)
            {
                if (ships.ElementAt(i).Y > _gameScreenManager.GameScreenHeight)
                {
                    ships.Remove(ships.ElementAt(i));
                    _remainingFails--;
                    continue;
                }

                if (ships.ElementAt(i).Life <= 0)
                {
                    if(!_isGameOver)
                        _playerScore += ((EnnemyShip)ships.ElementAt(i)).ScorePoints;
                    AddExplosion(ships.ElementAt(i));
                    AddBonus(ships.ElementAt(i));
                    ships.Remove(ships.ElementAt(i));
                    continue;
                }
            }
        }

        private void CheckAndRemove(List<Explosion> explosions)
        {
            for (int i = 0; i < explosions.Count(); i++)
            {
                if (explosions.ElementAt(i).HasEnded)
                    explosions.Remove(explosions.ElementAt(i));
            }
        }

        private void CheckAndRemove(List<BaseObject> elements, Positions limit)
        {
            switch (limit)
            {
                case Positions.Top:
                    for (int i = 0; i < elements.Count(); i++)
                    {
                        if (elements.ElementAt(i).Y + elements.ElementAt(i).Rectangle.Height < 0)
                            elements.Remove(elements.ElementAt(i));
                    }
                    break;
                case Positions.Right:
                    for (int i = 0; i < elements.Count(); i++)
                    {
                        if (elements.ElementAt(i).X > _gameScreenManager.GameScreenWidth)
                            elements.Remove(elements.ElementAt(i));
                    }
                    break;
                case Positions.Bottom:
                    for (int i = 0; i < elements.Count(); i++)
                    {
                        if (elements.ElementAt(i).Y > _gameScreenManager.GameScreenHeight)
                            elements.Remove(elements.ElementAt(i));
                    }
                    break;
                case Positions.Left:
                    for (int i = 0; i < elements.Count(); i++)
                    {
                        if (elements.ElementAt(i).X + elements.ElementAt(i).Rectangle.Width < 0)
                            elements.Remove(elements.ElementAt(i));
                    }
                    break;
            }
        }

        private void CheckAndRemove(List<Bullet> elements, Positions limit)
        {
            for (int i = 0; i < elements.Count(); i++)
            {
                if (elements.ElementAt(i).CollisionEntered)
                    elements.Remove(elements.ElementAt(i));
            }

            switch (limit)
            {
                case Positions.Top:
                    for (int i = 0; i < elements.Count(); i++)
                    {
                        if (elements.ElementAt(i).Y + elements.ElementAt(i).Rectangle.Height < 0)
                            elements.Remove(elements.ElementAt(i));
                    }
                    break;
                case Positions.Right:
                    for (int i = 0; i < elements.Count(); i++)
                    {
                        if (elements.ElementAt(i).X > _gameScreenManager.GameScreenWidth)
                            elements.Remove(elements.ElementAt(i));
                    }
                    break;
                case Positions.Bottom:
                    for (int i = 0; i < elements.Count(); i++)
                    {
                        if (elements.ElementAt(i).Y > _gameScreenManager.GameScreenHeight)
                            elements.Remove(elements.ElementAt(i));
                    }
                    break;
                case Positions.Left:
                    for (int i = 0; i < elements.Count(); i++)
                    {
                        if (elements.ElementAt(i).X + elements.ElementAt(i).Rectangle.Width < 0)
                            elements.Remove(elements.ElementAt(i));
                    }
                    break;
            }
        }

        private void CheckAndRemove(List<BonusBox> bonusBoxes)
        {
            for (int i = 0; i < bonusBoxes.Count(); i++)
            {
                if (bonusBoxes.ElementAt(i).CollisionEntered)
                {
                    bonusBoxes.Remove(bonusBoxes.ElementAt(i));
                    continue;
                }

                if (bonusBoxes.ElementAt(i).Y > _gameScreenManager.GameScreenHeight)
                {
                    bonusBoxes.Remove(bonusBoxes.ElementAt(i));
                    continue;
                }
            }
        }

        private void CheckAndRemoveShipBonus()
        {
            var bonusToRemove = _playerShip.Bonuses.FirstOrDefault(m => m.bonus.IsActive && m.bonus.Duration <= 0);
            if(!bonusToRemove.Equals(default(ValueTuple<int, Bonus>)))
            {
                _playerShip.Bonuses.Remove(bonusToRemove);
            }
        }

        private void CheckPlayerShipPosition()
        {
            if (_playerShip.X <= _leftArea.X + _leftArea.Width)
                _playerShip.X = _leftArea.X + _leftArea.Width;
            if (_playerShip.X + _playerShip.Width >= _rightArea.X)
                _playerShip.X = _rightArea.X - _playerShip.Width;
            if (_playerShip.Y <= 0)
                _playerShip.Y = 0;
            if (_playerShip.Y + _playerShip.Height >= _gameScreenManager.GameScreenHeight)
                _playerShip.Y = _gameScreenManager.GameScreenHeight - _playerShip.Height;
        }

        private void ShootEnnemyBullet(GameTime gameTime)
        {
            foreach (var ennemy in _ennemieShips)
            {
                if (ennemy.Y > -ennemy.Height)
                {
                    ((EnnemyShip)ennemy).CurrentShotInterval -= gameTime.ElapsedGameTime.TotalSeconds;
                    if (((EnnemyShip)ennemy).CurrentShotInterval <= 0)
                    {
                        ((EnnemyShip)ennemy).CurrentShotInterval = ((EnnemyShip)ennemy).ShotInterval;
                        ShootBullet(ennemy);
                    }
                }
            }
        }

        private void ShootBullets(GameTime gameTime)
        {
            var hasSpeedBonus = _playerShip.Bonuses.Any(m => m.bonus.Type == BonusType.AmmoSpeed && m.bonus.IsActive && m.bonus.Duration > 0);

            _currentShotInterval -= gameTime.ElapsedGameTime.TotalSeconds;
            if (_currentShotInterval <= 0)
            {
                _currentShotInterval = hasSpeedBonus ? _shotInterval / 2 : _shotInterval;
                ShootBullet();
            }
        }

        private void ShootBullet(Ship ship = null)
        {
            if (ship == null)
            {
                if (_playerShip.Bonuses.Any(m => m.bonus.Type == BonusType.AmmoX2 && m.bonus.IsActive && m.bonus.Duration > 0))
                {
                    var leftBullet = new Bullet(_doubleBulletTexture, true/*, _whitePixelTexture*/);
                    leftBullet.X = _playerShip.X - leftBullet.Rectangle.Width / 2;
                    leftBullet.Y = _playerShip.Y;
                    var rightBullet = new Bullet(_doubleBulletTexture, true/*, _whitePixelTexture*/);
                    rightBullet.X = _playerShip.X + _playerShip.Width - leftBullet.Rectangle.Width / 2;
                    rightBullet.Y = _playerShip.Y;
                    _playerBullets.Add(leftBullet);
                    _playerBullets.Add(rightBullet);
                    var bulletShotSoundInstance = _bulletShotSound.CreateInstance();
                    bulletShotSoundInstance.Volume = _gameScreenManager.SoundVolume > 0 ? GameConstants.GameBulletShotVolume : 0;
                    bulletShotSoundInstance.Play();
                }
                else
                {
                    var bullet = new Bullet(_bulletTexture, true/*, _whitePixelTexture*/);
                    bullet.X = _playerShip.X + _playerShip.Width / 2 - bullet.Rectangle.Width / 2;
                    bullet.Y = _playerShip.Y;
                    _playerBullets.Add(bullet);
                    var bulletShotSoundInstance = _bulletShotSound.CreateInstance();
                    bulletShotSoundInstance.Volume = _gameScreenManager.SoundVolume > 0 ? GameConstants.GameBulletShotVolume : 0;
                    bulletShotSoundInstance.Play();
                }
            }
            else
            {
                var bullet = new Bullet(_ennemyBulletTexture, false/*, _whitePixelTexture*/);
                bullet.X = ship.X + ship.Width / 2 - bullet.Rectangle.Width / 2;
                bullet.Y = ship.Y + ship.Height / 2;
                _ennemyBullets.Add(bullet);
                var bulletShotSoundInstance = _ennemyShotSound.CreateInstance();
                bulletShotSoundInstance.Volume = _gameScreenManager.SoundVolume > 0 ? GameConstants.GameBulletShotVolume : 0;
                bulletShotSoundInstance.Play();
            }
        }

        private void CheckCollisions()
        {
            if (!_isGameOver)
            {
                foreach (var ennemy in _ennemieShips)
                {
                    foreach (var bullet in _playerBullets)
                    {
                        if (bullet.CollisionBox.Intersects(ennemy.CollisionBox))
                        {
                            bullet.CollisionEntered = true;
                            ennemy.Life--;
                            var impactSound = _ennemyImpactSound.CreateInstance();
                            impactSound.Volume = _gameScreenManager.SoundVolume > 0 ? GameConstants.GameImpactVolume : 0;
                            impactSound.IsLooped = false;
                            impactSound.Play();
                        }
                    }


                    var activeShipShield = _playerShip.Bonuses.FirstOrDefault(m => m.bonus.Type == BonusType.ShipShield && m.bonus.IsActive).bonus;
                    if(activeShipShield != null)
                    {
                        if(ennemy.CollisionBox.Intersects(activeShipShield.Rectangle))
                        {
                            ennemy.Life--;
                        }
                    }
                    else
                    {
                        if (ennemy.CollisionBox.Intersects(_playerShip.CollisionBox))
                        {
                            ennemy.Life--;
                            _playerShip.Life--;
                        }
                    }

                    

                    var activeBackShield = _playerShip.Bonuses.FirstOrDefault(m => m.bonus.Type == BonusType.BackShield && m.bonus.IsActive).bonus;
                    if (activeBackShield != null && ennemy.CollisionBox.Intersects(activeBackShield.Rectangle)
                        && ennemy.Rectangle.Y > activeBackShield.Rectangle.Y - ennemy.Height + 20)
                    {
                        ennemy.Life = 0;
                    }

                }

                foreach (var bullet in _ennemyBullets)
                {
                    if (bullet.CollisionBox.Intersects(_playerShip.CollisionBox))
                    {
                        if (!_playerShip.Bonuses.Any(m => m.bonus.Type == BonusType.ShipShield && m.bonus.IsActive))
                            _playerShip.Life--;

                        bullet.CollisionEntered = true;
                        var impactSound = _ennemyImpactSound.CreateInstance();
                        impactSound.Volume = _gameScreenManager.SoundVolume > 0 ? GameConstants.GameImpactVolume : 0;
                        impactSound.IsLooped = false;
                        impactSound.Play();
                    }

                    foreach (var playerBullet in _playerBullets)
                    {
                        if (bullet.CollisionBox.Intersects(playerBullet.CollisionBox))
                        {
                            bullet.CollisionEntered = true;
                            playerBullet.CollisionEntered = true;
                        }
                    }
                }

                foreach (var bonusBox in _bonusBoxex)
                {
                    if (bonusBox.Rectangle.Intersects(_playerShip.CollisionBox))
                    {
                        if (_playerShip.Bonuses.Count < _playerShip.MaxBonusCapacity)
                        {
                            int index = 0;
                            for (int i = 1; i <= _playerShip.MaxBonusCapacity; i++)
                            {
                                if (!_playerShip.Bonuses.Any(m => m.index == i))
                                {
                                    index = i;
                                    i = _playerShip.MaxBonusCapacity;
                                    continue;
                                }
                            }

                            _playerShip.Bonuses.Add((index, bonusBox.Bonus));
                        }

                        bonusBox.CollisionEntered = true;
                    }
                }
            }
        }

        private void CheckDifficulty()
        {
            var currentDifficulty = _difficultySteps.LastOrDefault(m => m.ScoreLevel <= _playerScore);
            if(currentDifficulty != null && !currentDifficulty.IsReached)
            {
                currentDifficulty.IsReached = true;
                _maxEnnemiesCount += currentDifficulty.IncrementEnnemiesCount ? _ennemyCountIncrementation : 0;
                
                if(currentDifficulty.IcrementEnnemiesSpeed)
                {
                    _ennemyDefaultSpeed += _ennemySpeedIncrementation;
                    foreach(var ennemy in _ennemieShips)
                    {
                        ennemy.Speed += _ennemySpeedIncrementation;
                    }
                }
            }
        }

        private void CheckRemainingFails()
        {
            if(_remainingFails > 0 && _remainingFailsGauge.Count() > _remainingFails)
            {
                _remainingFailsGauge.RemoveAt(_remainingFailsGauge.Count() - 1);
            }
        }
        
        private void CheckGameOver()
        {
            if(_playerShip.Life <= 0 || _remainingFails  <= 0)
            {
                AddExplosion(_playerShip);
                _playerShip.Destroy();
                GameOver();
            }
        }

        private Rectangle GetRandomRectangle(SpaceElement elementType)
        {
            _random = new Random(Guid.NewGuid().GetHashCode());
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
                case SpaceElement.Ship:
                    return GetShipRandomRectangle();
            }

            xPosition = _random.Next(_leftArea.X + _leftArea.Width - (width / 2), _rightArea.X - (width / 2));

            return new Rectangle(xPosition, yPosition, width, height);
        }

        private Rectangle GetShipRandomRectangle()
        {
            _random = new Random(Guid.NewGuid().GetHashCode());
            int width = 59;
            int height = 100;
            int yPosition = yPosition = _random.Next(-_gameScreenManager.GameScreenHeight * 2, -height * 3);
            int xPoisition = _random.Next(_leftArea.X + _leftArea.Width, _rightArea.X - width);

            var rectangle = new Rectangle(xPoisition, yPosition, width, height);

            foreach (var ennemy in _ennemieShips)
            {
                if (ennemy.Rectangle.Intersects(rectangle))
                    return GetShipRandomRectangle();
            }

            return rectangle;
        }

        private Texture2D GetRandomTexture(SpaceElement elementType)
        {
            _random = new Random(Guid.NewGuid().GetHashCode());
            int index;

            switch (elementType)
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
                case SpaceElement.Ship:
                    index = _random.Next(0, _shipTextures.Count - 1);
                    return _shipTextures.ElementAt(index);
                case SpaceElement.Explosion:
                    index = _random.Next(0, _explosionTextures.Count - 1);
                    return _explosionTextures.ElementAt(index);
                default:
                    return null;
            }
        }

        private SoundEffectInstance GetRandomSound(SpaceElement elementType)
        {
            _random = new Random(Guid.NewGuid().GetHashCode());
            int index;

            switch (elementType)
            {
                case SpaceElement.Explosion:
                    index = _random.Next(0, _explosionSounds.Count - 1);
                    var sound = _explosionSounds.ElementAt(index);
                    var instance = sound.CreateInstance();
                    instance.Volume = _gameScreenManager.SoundVolume > 0 ? GameConstants.GameExplosionVolume : 0;
                    instance.IsLooped = false;
                    return instance;
                default:
                    return null;
            }
        }

        private Bonus GetRandomBonus()
        {
            _random = new Random(Guid.NewGuid().GetHashCode());
            var randomValue = _random.Next(0, 1000);

            if (randomValue >= 0 && randomValue < 100)
                return new Bonus { HasDuration = false, Duration = 0, IsActive = false, Type = BonusType.Health, Value = 1, FollowShip = false };
            if (randomValue >= 100 && randomValue < 200)
                return new Bonus { HasDuration = true, Duration = 10, IsActive = false, Type = BonusType.AmmoX2, Value = 0, FollowShip = false };
            if (randomValue >= 200 && randomValue < 300)
                return new Bonus { HasDuration = true, Duration = 10, IsActive = false, Type = BonusType.AmmoSpeed, Value = 2, FollowShip = false };
            if (randomValue >= 300 && randomValue < 400)
                return new Bonus { HasDuration = true, Duration = 10, IsActive = false, Type = BonusType.ShipShield, Value = 0, Texture = _shipShieldTexture, FollowShip = true };
            if (randomValue >= 400 && randomValue < 500)
                return new Bonus { HasDuration = true, Duration = 10, IsActive = false, Type = BonusType.BackShield, Value = 0, Texture = _backShieldTexture, FollowShip = false };
            if (randomValue >= 500 && randomValue < 600)
                return new Bonus { HasDuration = false, Duration = 0, IsActive = false, Type = BonusType.Boost, Value = 25, FollowShip = false };

            return null;
        }

        private Texture2D GetBonusTexture(BonusType type)
        {
            return _bonusBoxTextures.FirstOrDefault(m => m.type == type).texture;
        }

        private void UseBonus()
        {
            if ((_inputManager.IsTapped(Keys.NumPad1) || _inputManager.IsTapped(Keys.D1)) && _playerShip.Bonuses.Any(m => m.index == 1))
            {
                ApplyBonus(1);
            }
            if ((_inputManager.IsTapped(Keys.NumPad2) || _inputManager.IsTapped(Keys.D2)) && _playerShip.Bonuses.Any(m => m.index == 2))
            {
                ApplyBonus(2);
            }
            if ((_inputManager.IsTapped(Keys.NumPad3) || _inputManager.IsTapped(Keys.D3)) && _playerShip.Bonuses.Any(m => m.index == 3))
            {
                ApplyBonus(3);
            }
            if ((_inputManager.IsTapped(Keys.NumPad4) || _inputManager.IsTapped(Keys.D4)) && _playerShip.Bonuses.Any(m => m.index == 4))
            {
                ApplyBonus(4);
            }
            if ((_inputManager.IsTapped(Keys.NumPad5) || _inputManager.IsTapped(Keys.D5)) && _playerShip.Bonuses.Any(m => m.index == 5))
            {
                ApplyBonus(5);
            }
        }

        private void ApplyBonus(int index)
        {
            var bonus = _playerShip.Bonuses.FirstOrDefault(m => m.index == index).bonus;
            var sameActiveBonu = _playerShip.Bonuses.FirstOrDefault(m => m.bonus.Type == bonus.Type && m.bonus.IsActive).bonus;
            if(bonus != null && sameActiveBonu == null)
            {
                switch(bonus.Type)
                {
                    case BonusType.Health:
                        _playerShip.RestoreHealth();
                        break;
                    case BonusType.Boost:
                        _playerShip.AddBoost(bonus.Value);
                        break;
                    case BonusType.ShipShield:
                        UseShipShiel(bonus);
                        break;
                    case BonusType.BackShield:
                        UseBackShield(bonus);
                        break;
                }
                bonus.IsActive = true;
            }
        }

        private void UseShipShiel(Bonus bonus)
        {
            int size = _playerShip.Height + 20;
            var tempRectangle = new Rectangle(0, 0, size, size);

            bonus.Rectangle = tempRectangle.AlignCenter(_playerShip.Rectangle);
            bonus.IsAnimated = true;
            bonus.Animation = new DoubleAnimation(bonus.Texture, 0.1f, 4, 1);
        }

        private void UseBackShield(Bonus bonus)
        {
            int height = 50;
            bonus.Rectangle = new Rectangle(0, _gameScreenManager.GameScreenHeight - height, _gameScreenManager.GameScreenWidth, height);
            bonus.IsAnimated = true;
            bonus.Animation = new DoubleAnimation(bonus.Texture, 0.1f, 1, 4);
        }

        private void GameOver()
        {
            _isGameOver = true;
            var now = DateTime.Now;
            var scoreToSave = new Score { Date = now, Points = _playerScore, PlayerName = _gameScreenManager.Player.Name, PlayerId = _gameScreenManager.Player.Id };
            if(!_isLocalSaving)
                SaveLocal(scoreToSave, now);
            if (!_isCloudSaving)
            {
                var backgroundThread = new Thread(
                new ThreadStart(() => SaveToCloud(scoreToSave)));
                backgroundThread.Start();
                //await SaveToCloud(scoreToSave);
            }

            _gameScreenManager.ChangeScreen(new GameOverScreen(_gameScreenManager, _contentManager, _playerScore));
        }

        private void SaveLocal(Score score, DateTime now)
        {
            _isLocalSaving = true;
            var todayScore = _gameScreenManager.Player.Scores.FirstOrDefault(m => m.Date.Date == now.Date);
            if (todayScore == null)
                _gameScreenManager.Player.Scores.Add(score);
            else
            {
                if (todayScore.Points < _playerScore)
                    todayScore.Points = _playerScore;
            }

            _storageService.Save(_gameScreenManager.Player);
        }

        private async void SaveToCloud(Score score)
        {
            try
            {
                _isCloudSaving = true;
                ScoreEntity scoreToSave = new ScoreEntity(score.PlayerId, score.PlayerName, score.Points, score.Date.ToString("dd-MM-yyyy"));

                var todayScore = await _azureStorageService.GetUniqueAsync<ScoreEntity>(GameConstants.AzureStorageScoreTable, score.PlayerId.ToString(), score.Date.ToString("dd-MM-yyyy"));

                if (todayScore != null)
                {
                    if (todayScore.Score < score.Points)
                        await _azureStorageService.InsertOrUpdateUniqueAsync(GameConstants.AzureStorageScoreTable, scoreToSave);
                }
                else
                    await _azureStorageService.InsertUniqueAsync(GameConstants.AzureStorageScoreTable, scoreToSave);
            }
            catch(Exception e)
            {

            }
        }

        private int GetBoostGaugeWidth()
        {
            return _boostGaugeMaxWidth * _playerShip.Boost / 100;
        }

        private void FadeInOut(GameTime gameTime)
        {
            _fadeDelay -= gameTime.ElapsedGameTime.TotalSeconds;
            if (_fadeDelay <= 0)
            {
                //Reset the Fade delay
                _fadeDelay = .015;
                _alphaValue += _alphaIncrement;

                if (_alphaValue >= 1.0f || _alphaValue <= 0.0f)
                    _alphaIncrement *= -1;
            }
        }

        private void DrawDebug(SpriteBatch spriteBatch)
        {
            string playerBoost = $"Player boost : {_playerShip.Boost}";
            string ennemiesDefaultSpeed = $"Ennemies speed : {_ennemyDefaultSpeed}";
            string totalEnnemies = $"Total ennemies : {_ennemieShips.Count()}";
            string remainingFails = $"Remaining ships : {_remainingFails}";
            string playerBullets = $"Displayed player bullets {_playerBullets.Count()}";
            string playerLife = $"Player life : {_playerShip.Life}";
            string score = $"Score : {_playerScore}";

            string playerBonusses = "Player bonusses : \n";
            foreach(var bonus in _playerShip.Bonuses)
            {
                playerBonusses = string.Concat(playerBonusses, $"{bonus.index} - {bonus.bonus.Type.ToString()} (isActive : {bonus.bonus.IsActive})\n");
            }

            spriteBatch.DrawString(_debugFont, playerBoost, new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(_debugFont, remainingFails, new Vector2(10, 30), Color.White);
            spriteBatch.DrawString(_debugFont, ennemiesDefaultSpeed, new Vector2(10, 50), Color.White);
            spriteBatch.DrawString(_debugFont, totalEnnemies, new Vector2(10, 70), Color.White);
            spriteBatch.DrawString(_debugFont, playerBullets, new Vector2(10, 90), Color.White);
            spriteBatch.DrawString(_debugFont, playerLife, new Vector2(10, 110), Color.White);
            spriteBatch.DrawString(_debugFont, score, new Vector2(10, 130), Color.White);
            spriteBatch.DrawString(_debugFont, playerBonusses, new Vector2(10, 150), Color.White);
        }
    }
}
