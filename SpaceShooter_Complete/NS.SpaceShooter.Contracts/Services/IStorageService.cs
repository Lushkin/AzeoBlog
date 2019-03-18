namespace NS.SpaceShooter.Contracts.Services
{
    using NS.SpaceShooter.Models.Game;

    public interface IStorageService
    {
        void Save(Player player);
        Player Load();
    }
}
