namespace NS.SpaceShooter.Screens
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using NS.SpaceShooter.Contracts.Managers;
    using NS.SpaceShooter.Managers;
    using NS.SpaceShooter.Models.Constants;
    using NS.SpaceShooter.Models.Game;
    using NS.SpaceShooter.Models.Score;
    using NS.SpaceShooter.Models.Helpers;
    using System.Collections.Generic;
    using System.Linq;
    using NS.SpaceShooter.Contracts.Services;
    using NS.SpaceShooter.Services;
    using NS.SpaceShooter.Models.Entities;
    using Microsoft.WindowsAzure.Storage.Table;
    using System;
    using NS.SpaceShooter.Models.Animations;
    using System.Threading;

    public class ScoreScreen : IGameScreen
    {
        private readonly IGameScreenManager _gameScreenManager;
        private readonly IInputManager _inputManager;
        private readonly ContentManager _contentManager;
        private readonly IAzureStorageService _azureStorageService;

        private Texture2D _whitePixelTexture, _backgroundTexture, _loaderTexture;
        private Rectangle _backgroundRectangle, _loaderContainer, _loaderRectangle;
        private SpriteFont _gameTitleFontBig, _gameFontMedium, _gameFontSmall;
        private ScoreContainer _playerScoreContainer, _globalScoreContainer;
        private bool _globalScoreIsLoaded;
        private SimpleAnimation _animation;
        private float _animationTime;
        private List<Score> _globalScores;
        private Thread _backgroundThread;

        public ScoreScreen(IGameScreenManager gameScreenManager, ContentManager contentManager)
        {
            _gameScreenManager = gameScreenManager;
            _contentManager = contentManager;
            _inputManager = new InputManager(_gameScreenManager);
            _azureStorageService = new AzureStorageService();
            _globalScoreIsLoaded = false;

            _playerScoreContainer = new ScoreContainer(GetPlayerScores(), 
             CommonScreenTextConstants.Score.PlayerScores, true, new Rectangle(0, 0, _gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight));
        }

        public bool IsPaused { get; private set; }

        public void Init(ContentManager contentManager)
        {
            LoadContent();
            _globalScores = new List<Score>();

            _backgroundThread = new Thread(
            new ThreadStart(GetGlobalScores));
            _backgroundThread.Start();

            _animation = new SimpleAnimation(_loaderTexture, 0.1f, 8);
            _loaderContainer = new Rectangle(0, 0, 200, 200).ToCenterRightWithMargin(new Rectangle(0, 0, _gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight), 200);
            _loaderRectangle = new Rectangle(0, 0, 50, 50).ToCenter(_loaderContainer);
            _backgroundRectangle = new Rectangle(0, 0, _gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight);
            _playerScoreContainer.Init();
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
            _playerScoreContainer.Update(gameTime);

            if(_globalScoreIsLoaded && _globalScoreContainer != null)
                _globalScoreContainer.Update(gameTime);

            if(_globalScoreIsLoaded && _globalScoreContainer == null)
            {
                _globalScoreContainer = new ScoreContainer(_globalScores,
                CommonScreenTextConstants.Score.GlobalScores, false, new Rectangle(0, 0, _gameScreenManager.GameScreenWidth, _gameScreenManager.GameScreenHeight));
                _globalScoreContainer.LoadContent(_contentManager);
                _globalScoreContainer.Init();
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_backgroundTexture, _backgroundRectangle, Color.White);
            _playerScoreContainer.Draw(gameTime, spriteBatch);
            if (_globalScoreIsLoaded)
                _globalScoreContainer.Draw(gameTime, spriteBatch);
            else
                DrawLoader(gameTime, spriteBatch);

            spriteBatch.DrawString(
                      _gameFontSmall,
                      CommonScreenTextConstants.BackNavigationText,
                      CommonScreenTextConstants.BackNavigationText.ToTopLeftWithMargin(_backgroundRectangle, _gameFontSmall),
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
            _backgroundTexture = _contentManager.Load<Texture2D>("Images/Miscelaneous/Pause_Background");
            _loaderTexture = _contentManager.Load<Texture2D>("Images/Miscelaneous/Loader");

            _gameTitleFontBig = _contentManager.Load<SpriteFont>("Fonts/GameTitleBig");
            _gameFontMedium = _contentManager.Load<SpriteFont>("Fonts/GameFontMedium");
            _gameFontSmall = _contentManager.Load<SpriteFont>("Fonts/GameFontSmall");
            _playerScoreContainer.LoadContent(_contentManager);
        }

        private List<Score> GetPlayerScores()
        {
            var scores = new List<Score>();
            foreach(var score in _gameScreenManager.Player.Scores)
            {
                scores.Add(new Score { Date = score.Date, Points = score.Points, PlayerName = _gameScreenManager.Player.Name });
            }

            return scores.OrderByDescending(m => m.Points).Take(20).ToList();
        }

        private async void GetGlobalScores()
        {
            try
            {
                var scores = new List<Score>();

                TableQuery<ScoreEntity> allScoresQuery = new TableQuery<ScoreEntity>();
                IEnumerable<ScoreEntity> allScores = await _azureStorageService.GetAsync(GameConstants.AzureStorageScoreTable, allScoresQuery);

                scores = allScores.Select(m => new Score { PlayerId = new Guid(m.PartitionKey), PlayerName = m.Name, Date = DateTime.Parse(m.RowKey), Points = m.Score }).ToList();

                _globalScores = scores.OrderByDescending(m => m.Points).Take(20).ToList();
            }
            catch (Exception e)
            {
                _globalScores = new List<Score>();
            }
            finally
            {
                _globalScoreIsLoaded = true;
            }
        }

        private void DrawLoader(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_whitePixelTexture, _loaderContainer, Color.Black * 0.9f);
            _animationTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (_animationTime > _animation.FrameTime)
            {
                // Play the next frame in the SpriteSheet
                _animation.FrameIndex++;

                // reset elapsed time
                _animationTime = 0f;
            }

            if (_animation.FrameIndex >= _animation.TotalFrames)
                _animation.FrameIndex = 0;

            // Calculate the source rectangle of the current frame.
            Rectangle source = new Rectangle(_animation.FrameIndex * _animation.FrameWidth, 0, _animation.FrameWidth, _animation.FrameHeight);

            // Draw the current frame.
            spriteBatch.Draw(_animation.SpriteSheet, _loaderRectangle, source, Color.White);
        }
    }
}
