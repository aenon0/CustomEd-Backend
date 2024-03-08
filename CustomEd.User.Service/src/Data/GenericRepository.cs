using System.Linq.Expressions;
using CustomEd.User.Service.Data.Interfaces;
using CustomEd.User.Service.Model;
using MongoDB.Driver;

namespace CustomEd.User.Service.Data;

/// <summary>
/// Represents a generic repository implementation for MongoDB.
/// </summary>
/// <typeparam name="T">The type of entity.</typeparam>
public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    private readonly IMongoCollection<T> _collection;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericRepository{T}"/> class.
    /// </summary>
    /// <param name="database">The MongoDB database.</param>
    /// <param name="collection">The MongoDB collection.</param>
    public GenericRepository(IMongoDatabase? database, IMongoCollection<T> collection)
    {
        _collection = collection;
    }

    /// <summary>
    /// Retrieves an entity by its ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the entity.</param>
    /// <returns>The entity with the specified ID, or null if not found.</returns>
    public async Task<T> GetAsync(Guid id)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Retrieves all entities asynchronously.
    /// </summary>
    /// <returns>A collection of all entities.</returns>
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    /// <summary>
    /// Retrieves entities that match the specified filter asynchronously.
    /// </summary>
    /// <param name="filter">The filter expression.</param>
    /// <returns>A collection of entities that match the filter.</returns>
    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter)
    {
        return await _collection.Find(filter).ToListAsync();
    }

    /// <summary>
    /// Retrieves an entity that matches the specified filter asynchronously.
    /// </summary>
    /// <param name="filter">The filter expression.</param>
    /// <returns>The entity that matches the filter, or null if not found.</returns>
    public async Task<T> GetAsync(Expression<Func<T, bool>> filter)
    {
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Creates a new entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    public async Task CreateAsync(T entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        await _collection.InsertOneAsync(entity);
    }

    /// <summary>
    /// Updates an existing entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    public async Task UpdateAsync(T entity)
    {
        var filter = Builders<T>.Filter.Eq(x => x.Id, entity.Id);
        entity.UpdatedAt = DateTime.UtcNow;
        await _collection.ReplaceOneAsync(filter, entity);
    }

    /// <summary>
    /// Removes an entity by its ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the entity to remove.</param>
    public async Task RemoveAsync(Guid id)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        await _collection.DeleteOneAsync(filter);
    }

    /// <summary>
    /// Removes an entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    public async Task RemoveAsync(T entity)
    {
        var filter = Builders<T>.Filter.Eq("Id", entity.Id);
        await _collection.DeleteOneAsync(filter);
    }
}