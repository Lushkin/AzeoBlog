namespace NS.SpaceShooter.Contracts.Services
{
    using Microsoft.WindowsAzure.Storage.Table;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IAzureStorageService
    {
        Task InsertAsync<T>(string tableName, IEnumerable<T> entities) where T : ITableEntity;
        Task InsertUniqueAsync<T>(string tableName, T entity) where T : ITableEntity;
        Task UpdateUniqueAsync<T>(string tableName, T entity) where T : ITableEntity;
        Task InsertOrUpdateUniqueAsync<T>(string tableName, T entity) where T : ITableEntity;
        Task<IEnumerable<T>> GetAsync<T>(string tableName, TableQuery<T> query) where T : ITableEntity, new();
        Task<T> GetUniqueAsync<T>(string tableName, string partitionKey, string rowKey) where T : ITableEntity, new();
        Task DeleteAsync<T>(string tableName, IEnumerable<T> items) where T : ITableEntity, new();
        Task DeleteAsync<T>(string tableName, TableQuery<T> query) where T : ITableEntity, new();
        Task DeleteUniqueAsync<T>(string tableName, string partitionKey, string rowKey) where T : ITableEntity, new();
    }
}
