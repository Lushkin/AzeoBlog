namespace NS.SpaceShooter.Services
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using NS.SpaceShooter.Contracts.Services;
    using NS.SpaceShooter.Models.Constants;
    using NS.SpaceShooter.Services.Extensions;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class AzureStorageService : IAzureStorageService
    {
        private const int BatchMaxElements = 100;
        private readonly CloudStorageAccount _account;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableManager" /> class.
        /// </summary>
        /// <param name="account">The account.</param>
        public AzureStorageService()
        {
            _account = GetAccountFromAppSettings();
        }

        /// <summary>
        /// Writes the specified entities into a table with the given name.
        /// </summary>
        /// <typeparam name="T">Type of the entity.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="entities">The entities.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task InsertAsync<T>(string tableName, IEnumerable<T> entities)
            where T : ITableEntity
        {
            CloudTableClient client = _account.CreateCloudTableClient();
            CloudTable table = client.GetTableReference(tableName);
            await table.CreateIfNotExistsAsync().ConfigureAwait(false);

            foreach (var partition in entities.Split(BatchMaxElements))
            {
                var batchOperation = new TableBatchOperation();

                foreach (var entity in partition)
                {
                    batchOperation.Insert(entity);
                }

                await table.ExecuteBatchAsync(batchOperation).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Writes the specified entity into a table with the given name.
        /// </summary>
        /// <typeparam name="T">Type of the entity.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task InsertUniqueAsync<T>(string tableName, T entity)
            where T : ITableEntity
        {
            CloudTableClient client = _account.CreateCloudTableClient();
            CloudTable table = client.GetTableReference(tableName);
            await table.CreateIfNotExistsAsync().ConfigureAwait(false);

            TableOperation insertOperation = TableOperation.Insert(entity);

            await table.ExecuteAsync(insertOperation).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the specified entity into a table with the given name.
        /// </summary>
        /// <typeparam name="T">Type of the entity.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task UpdateUniqueAsync<T>(string tableName, T entity)
            where T : ITableEntity
        {
            CloudTableClient client = _account.CreateCloudTableClient();
            CloudTable table = client.GetTableReference(tableName);

            if (!await table.ExistsAsync().ConfigureAwait(false))
            {
                return;
            }

            TableOperation replaceOperation = TableOperation.Replace(entity);

            await table.ExecuteAsync(replaceOperation).ConfigureAwait(false);
        }

        /// <summary>
        /// Inserts or updates the specified entity into a table with the given name.
        /// </summary>
        /// <typeparam name="T">Type of the entity.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task InsertOrUpdateUniqueAsync<T>(string tableName, T entity)
            where T : ITableEntity
        {
            CloudTableClient client = _account.CreateCloudTableClient();
            CloudTable table = client.GetTableReference(tableName);
            await table.CreateIfNotExistsAsync().ConfigureAwait(false);

            TableOperation operation = TableOperation.InsertOrReplace(entity);

            await table.ExecuteAsync(operation).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the entities matching the specified query.
        /// </summary>
        /// <typeparam name="T">Type of the entity.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="query">The query.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<IEnumerable<T>> GetAsync<T>(string tableName, TableQuery<T> query)
            where T : ITableEntity, new()
        {
            CloudTableClient client = _account.CreateCloudTableClient();
            CloudTable table = client.GetTableReference(tableName);

            if (!await table.ExistsAsync().ConfigureAwait(false))
            {
                return Enumerable.Empty<T>();
            }

            return table.ExecuteQuery(query);
        }

        /// <summary>
        /// Gets the entities matching the specified query.
        /// </summary>
        /// <typeparam name="T">Type of the entity.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="rowKey">The row key.</param>
        /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
        public async Task<T> GetUniqueAsync<T>(string tableName, string partitionKey, string rowKey)
            where T : ITableEntity, new()
        {
            CloudTableClient client = _account.CreateCloudTableClient();
            CloudTable table = client.GetTableReference(tableName);

            if (!await table.ExistsAsync().ConfigureAwait(false))
            {
                return default(T);
            }

            TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            TableResult retrievedResult = await table.ExecuteAsync(retrieveOperation).ConfigureAwait(false);

            return retrievedResult.Result == null ? default(T) : (T)retrievedResult.Result;
        }

        /// <summary>
        /// Deletes the specified items.
        /// </summary>
        /// <typeparam name="T">Type of the entity.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="items">The items.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task DeleteAsync<T>(string tableName, IEnumerable<T> items)
            where T : ITableEntity, new()
        {
            CloudTableClient client = _account.CreateCloudTableClient();
            CloudTable table = client.GetTableReference(tableName);

            if (!await table.ExistsAsync().ConfigureAwait(false))
            {
                return;
            }

            foreach (var partition in items.GroupBy(x => x.PartitionKey))
            {
                foreach (var block in partition.Split(BatchMaxElements))
                {
                    var batchOperation = new TableBatchOperation();

                    foreach (var entity in block)
                    {
                        batchOperation.Delete(entity);
                    }

                    await table.ExecuteBatchAsync(batchOperation).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Deletes the entities matching the specified query.
        /// </summary>
        /// <typeparam name="T">Type of the entity.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="query">The query.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task DeleteAsync<T>(string tableName, TableQuery<T> query)
            where T : ITableEntity, new()
        {
            CloudTableClient client = _account.CreateCloudTableClient();
            CloudTable table = client.GetTableReference(tableName);

            if (!await table.ExistsAsync().ConfigureAwait(false))
            {
                return;
            }

            IEnumerable<T> entities = table.ExecuteQuery(query);

            foreach (var partition in entities.Split(BatchMaxElements))
            {
                var batchOperation = new TableBatchOperation();

                foreach (var entity in partition)
                {
                    batchOperation.Delete(entity);
                }

                await table.ExecuteBatchAsync(batchOperation).ConfigureAwait(false);
            }
        }

        public async Task DeleteUniqueAsync<T>(string tableName, string partitionKey, string rowKey)
            where T : ITableEntity, new()
        {
            CloudTableClient client = _account.CreateCloudTableClient();
            CloudTable table = client.GetTableReference(tableName);

            if (!await table.ExistsAsync().ConfigureAwait(false))
            {
                return;
            }

            var batchOperation = new TableBatchOperation();

            TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            TableResult retrievedResult = await table.ExecuteAsync(retrieveOperation).ConfigureAwait(false);

            var entity = retrievedResult.Result == null ? default(T) : (T)retrievedResult.Result;

            if(entity != null)
                batchOperation.Delete(entity);
        }

        private CloudStorageAccount GetAccountFromAppSettings()
        {
            string connectionString = GameConstants.AzureStorageConnexionString;
            return CloudStorageAccount.Parse(connectionString);
        }

    }
}
