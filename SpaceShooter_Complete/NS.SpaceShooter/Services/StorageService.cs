namespace NS.SpaceShooter.Services
{
    using Newtonsoft.Json;
    using NS.SpaceShooter.Contracts.Services;
    using NS.SpaceShooter.Models.Constants;
    using NS.SpaceShooter.Models.Game;
    using System;
    using System.IO;

    public class StorageService : IStorageService
    {
        public void Save(Player player)
        {
            var str = Serialize(player);
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\" + GameConstants.SettingsFileName, str);
        }

        public Player Load()
        {
            try
            {
                var str = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\" + GameConstants.SettingsFileName);
                var player = Deserialize<Player>(str);
                return player;
            }
            catch
            {
                return null;
            }
        }

        private string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        private T Deserialize<T>(string str)
        {
            return JsonConvert.DeserializeObject<T>(str);
        }
    }
}
