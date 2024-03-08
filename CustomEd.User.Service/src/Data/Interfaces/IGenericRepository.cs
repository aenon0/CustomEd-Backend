//Purpose : Represents a generic repository interface for CRUD operations on entities.

using System.Linq.Expressions;
using CustomEd.User.Service.Model;

namespace CustomEd.User.Service.Data.Interfaces;

/// <summary>
/// Represents a generic repository interface for CRUD operations on entities.
/// </summary>
/// <typeparam name="T">The type of entity.</typeparam>
public interface IGenericRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Retrieves an entity by its ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the entity.</param>
    /// <returns>The entity with the specified ID.</returns>
    Task<T> GetAsync(Guid id);

    /// <summary>
    /// Retrieves all entities asynchronously.
    /// </summary>
    /// <returns>A collection of all entities.</returns>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Retrieves entities that satisfy the specified filter asynchronously.
    /// </summary>
    /// <param name="filter">The filter expression.</param>
    /// <returns>A collection of entities that satisfy the filter.</returns>
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter);

    /// <summary>
    /// Retrieves an entity that satisfies the specified filter asynchronously.
    /// </summary>
    /// <param name="filter">The filter expression.</param>
    /// <returns>The entity that satisfies the filter.</returns>
    Task<T> GetAsync(Expression<Func<T, bool>> filter);

    /// <summary>
    /// Creates a new entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    Task CreateAsync(T entity);

    /// <summary>
    /// Updates an existing entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Removes an entity by its ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the entity to remove.</param>
    Task RemoveAsync(Guid id);

    /// <summary>
    /// Removes an entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    Task RemoveAsync(T entity);
}