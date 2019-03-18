using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NS.SpaceShooter.Contracts.Managers;
using NS.SpaceShooter.Managers;
using NS.SpaceShooter.Screens;

namespace NS.SpaceShooter
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        IGameScreenManager gameScreenManager;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private bool _isFullScreen;
        private Point _windowPosition;
        private Texture2D whitePixel;
        private SpriteFont defaultFont;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            _isFullScreen = false;
            _windowPosition = new Point(50, 50);
            ToWindow();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            whitePixel = Content.Load<Texture2D>("Images/WhitePixel");
            defaultFont = Content.Load<SpriteFont>("Fonts/DefaultFont");

            gameScreenManager = new GameScreenManager(spriteBatch, Content, GraphicsDevice, whitePixel, defaultFont);
            gameScreenManager.Initialize();
            gameScreenManager.GameScreenWidth = graphics.PreferredBackBufferWidth;
            gameScreenManager.GameScreenHeight = graphics.PreferredBackBufferHeight;
            gameScreenManager.OnGameExit += Exit;
            gameScreenManager.ChangeScreen(new MenuScreen(gameScreenManager, Content));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            if (gameScreenManager != null)
            {
                gameScreenManager.Dispose();
                gameScreenManager = null;
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();

            //if (Keyboard.GetState().IsKeyDown(Keys.F11))
            //    SwitchScreenResolution(!_isFullScreen);

            gameScreenManager.ChangeBetweenScreens();
            gameScreenManager.HandleInput(gameTime);
            gameScreenManager.Update(gameTime);


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            gameScreenManager.Draw(gameTime, spriteBatch);

            base.Draw(gameTime);
        }

        private void SwitchScreenResolution(bool isFullScreen)
        {
            if(isFullScreen)
            {
                ToFullScreen();
            }
            else
            {
                ToWindow();
            }

            gameScreenManager.GameScreenWidth = graphics.PreferredBackBufferWidth;
            gameScreenManager.GameScreenHeight = graphics.PreferredBackBufferHeight;
            _isFullScreen = isFullScreen;
        }

        private void ToFullScreen()
        {
            _windowPosition = this.Window.Position;
            graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
        }

        private void ToWindow()
        {
            Window.Position = _windowPosition;
            graphics.PreferredBackBufferWidth = 1280;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = 720;   // set this value to the desired height of your window
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
        }
    }
}
