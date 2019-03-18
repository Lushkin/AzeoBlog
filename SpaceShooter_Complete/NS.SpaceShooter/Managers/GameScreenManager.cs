using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NS.SpaceShooter.Contracts.Managers;
using NS.SpaceShooter.Contracts.Services;
using NS.SpaceShooter.Models.Constants;
using NS.SpaceShooter.Models.Enums;
using NS.SpaceShooter.Models.Game;
using NS.SpaceShooter.Services;

namespace NS.SpaceShooter.Managers
{
    public class GameScreenManager : IGameScreenManager
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly ContentManager _contentManager;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly List<IGameScreen> _gameScreens = new List<IGameScreen>();
        private readonly Texture2D _whitePixel;
        private readonly SpriteFont _defaultFont;
        private readonly Rectangle _transitionRectangle;
        private Action _onGameExit;
        private float _transitionOpacity;
        private TransitionState _transitionState;
        private IGameScreen _screenToDisplay;
        private IStorageService _storageService;

        private bool IsScreenListEmpty { get { return !_gameScreens.Any(); } }
        public float SoundVolume { get; private set; }
        public float MusicVolume { get; private set; }
        public GraphicsDevice GraphicsDevice { get { return _graphicsDevice; } }
        public KeyboardState PreviousKeyBoardState { get; set; }
        public MouseState PreviousMouseState { get; set; }
        public Player Player { get; set; }
        public int GameScreenWidth { get; set; }
        public int GameScreenHeight { get; set; }

        float timer = 10;

        public GameScreenManager(SpriteBatch spriteBatch, ContentManager contentManager, GraphicsDevice graphicsDevice, Texture2D whitePixel, SpriteFont defaultFont)
        {
            _spriteBatch = spriteBatch;
            _contentManager = contentManager;
            _graphicsDevice = graphicsDevice;
            _whitePixel = whitePixel;
            _defaultFont = defaultFont;
            _transitionRectangle = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            _transitionOpacity = 0f;
            _storageService = new StorageService();
        }

        public void Initialize()
        {
            var player = _storageService.Load();
            if (player != null)
                Player = player;
            else
            {
                Player = new Player { Id = Guid.NewGuid(), Settings = new GameSettings { SoundActive = GameConstants.GameDefaultSoundEnabled, MusicActive = GameConstants.GameDefaultMusicEnabled } };
                _storageService.Save(Player);
            }

            SetVolume();
        }

        public void HandleInput(GameTime gameTime)
        {
            if (!IsScreenListEmpty)
            {
                var currentScreen = GetCurrentSreen();

                if (!currentScreen.IsPaused)
                    currentScreen.HandleInput(gameTime);
            }
        }

        public void Update(GameTime gameTime)
        {
            PreviousKeyBoardState = Keyboard.GetState();
            PreviousMouseState = Mouse.GetState();
            SetVolume();

            StartChangeScreenTransition();
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timer -= elapsed;

            if (!IsScreenListEmpty)
            {
                var currentScreen = GetCurrentSreen();

                if (!currentScreen.IsPaused)
                    currentScreen.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsScreenListEmpty)
            {
                var currentScreen = GetCurrentSreen();

                if (!currentScreen.IsPaused)
                {
                    spriteBatch.Begin();
                    currentScreen.Draw(gameTime, spriteBatch);
                    DrawTransition(spriteBatch);
                    spriteBatch.End();
                }
            }

            spriteBatch.Begin();
            DrawTransition(spriteBatch);
            spriteBatch.End();
        }


        public void ChangeScreen(IGameScreen screen)
        {
            if (_screenToDisplay == null)
                _transitionState = TransitionState.FadeIn;
            _screenToDisplay = screen;
            if (_transitionState == TransitionState.Hidden)
            {
                RemoveAllScreens();
                _gameScreens.Add(screen);
                screen.Init(_contentManager);
                _transitionState = TransitionState.FadeOut;
                _screenToDisplay = null;
            }
        }

        public void PushScreen(IGameScreen screen)
        {
            if (!IsScreenListEmpty)
            {
                var currentScreen = GetCurrentSreen();
                currentScreen.Pause();
            }

            _gameScreens.Add(screen);
            screen.Init(_contentManager);
        }

        public void PopScreen()
        {
            if (!IsScreenListEmpty)
                RemoveCurrentScreen();

            if (!IsScreenListEmpty)
            {
                var currentScreen = GetCurrentSreen();
                currentScreen.Resume();
            }
        }

        public void ChangeBetweenScreens()
        {
            if (!IsScreenListEmpty)
            {
                var currentScreen = GetCurrentSreen();

                if (!currentScreen.IsPaused)
                    currentScreen.ChangeBetweenScreens();
            }
        }

        public void Dispose()
        {
            RemoveAllScreens();
        }

        public void Exit()
        {
            _onGameExit?.Invoke();
        }

        public event Action OnGameExit
        {
            add { _onGameExit += value; }
            remove { _onGameExit -= value; }
        }
        #region Private Methods

        private IGameScreen GetCurrentSreen()
        {
            return _gameScreens.Last();
        }

        private void RemoveCurrentScreen()
        {
            var currentScreen = GetCurrentSreen();
            currentScreen.Dispose();
            _gameScreens.Remove(currentScreen);
        }

        private void RemoveAllScreens()
        {
            while (!IsScreenListEmpty)
            {
                RemoveCurrentScreen();
            }
        }

        private void StartChangeScreenTransition()
        {
            switch (_transitionState)
            {
                case TransitionState.Active:
                    _transitionOpacity = 0;
                    break;
                case TransitionState.Hidden:
                    _transitionOpacity = 1;
                    ChangeScreen(_screenToDisplay);
                    break;
                case TransitionState.FadeIn:
                    if (_transitionOpacity < 1)
                        _transitionOpacity += GameConstants.TransitionTime;
                    else
                        _transitionState = TransitionState.Hidden;
                    break;
                case TransitionState.FadeOut:
                    if (_transitionOpacity > 0)
                        _transitionOpacity -= GameConstants.TransitionTime;
                    else
                        _transitionState = TransitionState.Active;
                    break;
            }
        }

        private void DrawTransition(SpriteBatch spriteBatch)
        {
            if (_transitionState != TransitionState.Active)
                spriteBatch.Draw(_whitePixel, _transitionRectangle, Color.Black * _transitionOpacity);
        }

        private void SetVolume()
        {
            SoundVolume = Player.Settings.SoundActive ? GameConstants.SoundVolume : 0f;
            MusicVolume = Player.Settings.MusicActive ? GameConstants.MusicVolume : 0f;
        }

        #endregion
    }
}
