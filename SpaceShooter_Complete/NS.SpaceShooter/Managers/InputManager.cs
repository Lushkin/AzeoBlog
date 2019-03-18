namespace NS.SpaceShooter.Managers
{
    using Microsoft.Xna.Framework.Input;
    using NS.SpaceShooter.Contracts.Managers;
    using NS.SpaceShooter.Models.Enums;

    public class InputManager : IInputManager
    {
        private readonly IGameScreenManager _gameScreenManager;

        public InputManager(IGameScreenManager gameScreenManager)
        {
            _gameScreenManager = gameScreenManager;
        }

        #region IsFree
        public bool IsFree()
        {
            return Keyboard.GetState().Equals(new KeyboardState());
        }
        #endregion

        #region IsTapped
        public bool IsTapped(Keys key)
        {
            return (Keyboard.GetState().IsKeyDown(key) && _gameScreenManager.PreviousKeyBoardState.IsKeyUp(key));
        }

        public bool IsTapped(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.LeftButton:
                    return (Mouse.GetState().LeftButton == ButtonState.Pressed && _gameScreenManager.PreviousMouseState.LeftButton == ButtonState.Released);
                case MouseButton.MiddleButton:
                    return (Mouse.GetState().MiddleButton == ButtonState.Pressed && _gameScreenManager.PreviousMouseState.MiddleButton == ButtonState.Released);
                case MouseButton.RightButton:
                    return (Mouse.GetState().RightButton == ButtonState.Pressed && _gameScreenManager.PreviousMouseState.RightButton == ButtonState.Released);
                case MouseButton.XButton1:
                    return (Mouse.GetState().XButton1 == ButtonState.Pressed && _gameScreenManager.PreviousMouseState.XButton1 == ButtonState.Released);
                case MouseButton.XButton2:
                    return (Mouse.GetState().XButton2 == ButtonState.Pressed && _gameScreenManager.PreviousMouseState.XButton2 == ButtonState.Released);
                default:
                    return false;
            }
        }
        #endregion

        #region IsPressed
        public bool IsPressed(Keys key)
        {
            return (Keyboard.GetState().IsKeyDown(key) && _gameScreenManager.PreviousKeyBoardState.IsKeyDown(key));
        }

        public bool IsPressed(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.LeftButton:
                    return (Mouse.GetState().LeftButton == ButtonState.Pressed && _gameScreenManager.PreviousMouseState.LeftButton == ButtonState.Pressed);
                case MouseButton.MiddleButton:
                    return (Mouse.GetState().MiddleButton == ButtonState.Pressed && _gameScreenManager.PreviousMouseState.MiddleButton == ButtonState.Pressed);
                case MouseButton.RightButton:
                    return (Mouse.GetState().RightButton == ButtonState.Pressed && _gameScreenManager.PreviousMouseState.RightButton == ButtonState.Pressed);
                case MouseButton.XButton1:
                    return (Mouse.GetState().XButton1 == ButtonState.Pressed && _gameScreenManager.PreviousMouseState.XButton1 == ButtonState.Pressed);
                case MouseButton.XButton2:
                    return (Mouse.GetState().XButton2 == ButtonState.Pressed && _gameScreenManager.PreviousMouseState.XButton2 == ButtonState.Pressed);
                default:
                    return false;
            }
        }
        #endregion

        #region IsReleased
        public bool IsReleased(Keys key)
        {
            return (Keyboard.GetState().IsKeyUp(key) && _gameScreenManager.PreviousKeyBoardState.IsKeyDown(key));
        }

        public bool IsReleased(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.LeftButton:
                    return (Mouse.GetState().LeftButton == ButtonState.Released && _gameScreenManager.PreviousMouseState.LeftButton == ButtonState.Pressed);
                case MouseButton.MiddleButton:
                    return (Mouse.GetState().MiddleButton == ButtonState.Released && _gameScreenManager.PreviousMouseState.MiddleButton == ButtonState.Pressed);
                case MouseButton.RightButton:
                    return (Mouse.GetState().RightButton == ButtonState.Released && _gameScreenManager.PreviousMouseState.RightButton == ButtonState.Pressed);
                case MouseButton.XButton1:
                    return (Mouse.GetState().XButton1 == ButtonState.Released && _gameScreenManager.PreviousMouseState.XButton1 == ButtonState.Pressed);
                case MouseButton.XButton2:
                    return (Mouse.GetState().XButton2 == ButtonState.Released && _gameScreenManager.PreviousMouseState.XButton2 == ButtonState.Pressed);
                default:
                    return false;
            }
        }
        #endregion
    }
}
