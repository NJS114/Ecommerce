using MongoDB.Driver;

namespace Ecommerce.Services.DAO.Interfaces.IRepository
{
    public interface IMongoDbConnection
    {
        IMongoDatabase GetDatabase();
    }
}
