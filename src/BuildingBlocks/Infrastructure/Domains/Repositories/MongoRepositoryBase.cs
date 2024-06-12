using System.Linq.Expressions;
using Contracts.Domains;
using Contracts.Domains.Repositories;
using Infrastructure.Attributes;
using MongoDB.Driver;
using Shared.Settings;

namespace Infrastructure.Domains.Repositories;

public class MongoRepositoryBase<T>(IMongoClient client, MongoDbSettings settings) : IMongoRepositoryBase<T>
    where T : MongoEntity
{
    private IMongoDatabase Database { get; } = client.GetDatabase(settings.DatabaseName).WithWriteConcern(WriteConcern.Acknowledged);

    protected virtual IMongoCollection<T> Collection => Database.GetCollection<T>(GetCollectionName());

    public IMongoCollection<T> FindAll(ReadPreference? readPreference = null)
    {
        return Database
            .WithReadPreference(readPreference ?? ReadPreference.Primary)
            .GetCollection<T>(GetCollectionName());
    }
    
    public async Task<T> FindByIdAsync(string id, ReadPreference? readPreference = null)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        return await Database.WithReadPreference(readPreference ?? ReadPreference.Primary)
            .GetCollection<T>(GetCollectionName()).Find(filter).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(T entity) => await Collection.InsertOneAsync(entity);

    public async Task UpdateAsync(T entity)
    {
        Expression<Func<T, string>> func = f => f.Id;

        var propertyName = func.Body.ToString().Split(".")[1];
        var propertyInfo = entity.GetType().GetProperty(propertyName);

        if (propertyInfo == null)
        {
            throw new Exception($"Cannot found '{propertyName}' in '{entity.GetType().Name}'.");
        }

        var valueObj = propertyInfo.GetValue(entity, null);

        if (valueObj is not string value)
        {
            throw new InvalidCastException($"Cannot cast '{propertyName}' to string.");
        }

        var filter = Builders<T>.Filter.Eq(func, value);
        await Collection.ReplaceOneAsync(filter, entity);
    }

    public async Task DeleteAsync(string id) => await Collection.DeleteOneAsync(x => x.Id.Equals(id));
    
    private static string GetCollectionName()
    {
        if (typeof(T).GetCustomAttributes(typeof(BsonCollectionAttribute), true).FirstOrDefault() is BsonCollectionAttribute collectionAttribute)
        {
            return collectionAttribute.CollectionName;
        }

        return "DefaultCollectionName";
    }
}