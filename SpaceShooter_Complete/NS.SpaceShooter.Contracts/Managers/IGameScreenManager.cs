using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NS.SpaceShooter.Models.Game;
using System;

namespace NS.SpaceShooter.Contracts.Managers
{
    public interface IGameScreenManager : IDisposable
    {
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        void Initialize();

        float SoundVolume { get; }
        float MusicVolume { get; }
        GraphicsDevice GraphicsDevice { get; }
        KeyboardState PreviousKeyBoardState { get; set; }
        MouseState PreviousMouseState { get; set; }
        Player Player { get; set; }

        int GameScreenWidth { get; set; }
        int GameScreenHeight { get; set; }

        /// <summary>
        /// Occurs when [on game exit].
        /// </summary>
        event Action OnGameExit;

        /// <summary>
        /// Changes the between screens.
        /// </summary>
        void ChangeBetweenScreens();

        /// <summary>
        /// Remove all screens and display a new one.
        /// </summary>
        /// <param name="screen">The screen.</param>
        void ChangeScreen(IGameScreen screen);

        /// <summary>
        /// Add the screen in the existant screens stack. Previous screens are conserved, bun paused.
        /// </summary>
        /// <param name="screen">The screen.</param>
        void PushScreen(IGameScreen screen);

        /// <summary>
        /// Removes the las screen of the existant screens stack. The next screen in the stack will be resumed
        /// </summary>
        void PopScreen();

        /// <summary>
        /// Handles the input from keyboard
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        void HandleInput(GameTime gameTime);

        /// <summary>
        /// Exits this instance.
        /// </summary>
        void Exit();

        /// <summary>
        /// Updates the specified game time loop.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        void Update(GameTime gameTime);

        /// <summary>
        /// Draws the specified game time loop.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <param name="spriteBatch">The sprite batch.</param>
        void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
