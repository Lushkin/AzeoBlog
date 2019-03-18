namespace NS.SpaceShooter.Contracts.Managers
{
    using Microsoft.Xna.Framework.Input;
    using NS.SpaceShooter.Models.Enums;

    public interface IInputManager
    {
        bool IsTapped(Keys key);

        bool IsTapped(MouseButton button);

        bool IsPressed(Keys key);

        bool IsPressed(MouseButton button);

        bool IsReleased(Keys key);

        bool IsReleased(MouseButton button);

        bool IsFree();
    }
}
