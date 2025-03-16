using Ecommerce.Services.DAO.Interfaces.IRepository;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;

namespace Ecommerce.Services.DAO.Connexion
{
    public class MongoConnection : IMongoDbConnection, IDisposable
    {
        private readonly string _connectionString;
        private readonly string _databaseName;
        private IMongoClient _mongoClient;
        private IMongoDatabase _mongoDatabase;
        private static readonly object _lock = new object();
        private bool _disposed = false;
        private readonly string mongoConnection = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");
        private readonly string mongoDbName = Environment.GetEnvironmentVariable("MONGO_CONNECTION_DB_NAME");

        private static MongoConnection _instance;

        public IMongoClient Client => _mongoClient;

         MongoConnection(IOptions<MongoDbSettings> mongoDbSettings)
        {
            _connectionString = mongoConnection;
            _databaseName = mongoDbName;

            _mongoClient = new MongoClient(_connectionString);
            _mongoDatabase = _mongoClient.GetDatabase(_databaseName);
        }

        public IMongoDatabase GetDatabase()
        {
            return _mongoDatabase;
        }

        public static MongoConnection Instance(IOptions<MongoDbSettings> mongoDbSettings)
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new MongoConnection(mongoDbSettings);
                    }
                }
            }
            return _instance;
        }


        // Libération des ressources
        public void Dispose()
        {
            if (!_disposed)
            {
                _mongoClient = null;
                _mongoDatabase = null;
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}
