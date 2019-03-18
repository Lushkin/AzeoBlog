namespace NS.SpaceShooter.Models.Enums
{
    public enum TransitionState
    {
        FadeIn, // Start opacity incrementation (from 0 to 1)
        FadeOut, //Start opacity decrementation (from 1 to 0)
        Active, //Display screen (opacity 0)
        Hidden, //Hide screen (opacity 1)
    }
}
