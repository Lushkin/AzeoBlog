namespace NS.SpaceShooter.Models.Constants
{
    using Microsoft.Xna.Framework;

    public static class GameConstants
    {
        public const float TransitionTime = 0.025f;
        public const float SoundVolume = 0.5f;
        public const float MusicVolume = 0.3f;
        public const float GameMusicVolume = 0.4f;
        public const float GameWrapEngineVolume = 0.7f;
        public const float GameBulletShotVolume = 0.5f;
        public const float GameImpactVolume = 0.2f;
        public const float GameExplosionVolume = 0.3f;

        public static Color MenuButtonColor = new Color(122, 122, 122, 20);
        public static Color MenuSelectedButtonColor = Color.White;
        public static Color TextColor = Color.White;
        public static Color TextNegativeColor = Color.Black;
        public static Color TextErrorColor = Color.Red;

        public const bool GameDefaultSoundEnabled = true;
        public const bool GameDefaultMusicEnabled = true;

        public const string SettingsFileName = "NS.SpaceShooter.Save.json";
        public const string AzureStorageConnexionString = "Put your Azure Storage connexion string HERE";
        public const string AzureStorageScoreTable = "GlobalScored";
    }
}
