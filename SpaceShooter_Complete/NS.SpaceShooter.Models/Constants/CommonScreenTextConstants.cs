namespace NS.SpaceShooter.Models.Constants
{
    public static class CommonScreenTextConstants
    {
        public const string BackNavigationText = "Press Esc to go back";
        public const string Yes = "Yes";
        public const string No = "No";
        public const string On = "On";
        public const string Off = "Off";
        public const string Separator = " / ";

        public static class Menu
        {
            public const string Title = "Space Shooter";
            public const string SubTitle = "Press SPACE key to continue.";
            public const string Version = "Version 1.0.0";

            public const string Play = "Play";
            public const string Credits = "Credits";
            public const string Options = "Options";
            public const string Scores = "Scores";
            public const string Quit = "Quit";
        }

        public static class Credits
        {
            public const string PostCreditMessage = "It's OK ! You can leave now. There is nothing more to see here :)";
        }

        public static class Pause
        {
            public const string Title = "Pause";
            public const string Resume = "Resume";
            public const string Options = "Options";
            public const string Quit = "Quit";
            public const string QuitConfirmMessage = "Are you shure you want to quit ? \nAll progression will be lost !";
        }

        public static class Options
        {
            public const string Title = "Options";
            public const string Sound = "Sound";
            public const string Music = "Music";
            public const string Controls = "Controls";
            public const string Save = "Save";
        }

        public static class Score
        {
            public const string PlayerScores = "Player scores";
            public const string GlobalScores = "Global scores";
        }

        public static class PlayerSettings
        {
            public const string PlayerName = "Enter your name and submit with \"Enter\" key";
            public const string EmptyNameErrorMessage = "Please enter your name before submitting";
            public const string ConfirmationMessage = "Are you sure you want to keep this name ?\nYou won't be able to change it later !";
        }
    }
}
