// Copyright (c) Bruno Sales <me@baliestri.dev>. Licensed under the MIT License.
// See the LICENSE file in the repository root for full license text.

using System.Linq.Expressions;

namespace Empress.Labs.PowerShell.DataVault.Abstractions;

/// <summary>
///   A repository that provides basic operations to entities.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public interface IRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : Entity, new() {
  /// <summary>
  ///   Converts the repository to a read-only repository.
  /// </summary>
  /// <returns>A read-only repository.</returns>
  IReadOnlyRepository<TEntity> AsReadOnly();

  /// <summary>
  ///   Adds an entity to the repository.
  /// </summary>
  /// <param name="entity">The entity to add.</param>
  /// <exception cref="ArgumentNullException">If the <paramref name="entity" /> is <c>null</c>.</exception>
  void Add(TEntity entity);

  /// <summary>
  ///   Adds a range of entities to the repository.
  /// </summary>
  /// <param name="entities">The entities to add.</param>
  /// <exception cref="ArgumentNullException">If the <paramref name="entities" /> is <c>null</c>.</exception>
  void Add(IEnumerable<TEntity> entities);

  /// <summary>
  ///   Updates an entity in the repository.
  /// </summary>
  /// <param name="id">The identifier of the entity to update.</param>
  /// <param name="action">The action to update the entity.</param>
  /// <exception cref="ArgumentNullException">If the <paramref name="id" /> or <paramref name="action" /> is <c>null</c>.</exception>
  void Update(Guid id, Action<TEntity> action);

  /// <summary>
  ///   Updates an entity in the repository.
  /// </summary>
  /// <param name="entity">The entity to update.</param>
  /// <exception cref="ArgumentNullException">If the <paramref name="entity" /> is <c>null</c>.</exception>
  void Update(TEntity entity);

  /// <summary>
  ///   Updates a range of entities in the repository.
  /// </summary>
  /// <param name="entities">The entities to update.</param>
  /// <exception cref="ArgumentNullException">If the <paramref name="entities" /> is <c>null</c>.</exception>
  void Update(IEnumerable<TEntity> entities);

  /// <summary>
  ///   Updates a range of entities in the repository.
  /// </summary>
  /// <param name="predicate">The predicate to filter the entities to update.</param>
  /// <param name="action">The action to update the entities.</param>
  /// <exception cref="ArgumentNullException">If the <paramref name="predicate" /> or <paramref name="action" /> is <c>null</c>.</exception>
  void Update(Expression<Func<TEntity, bool>> predicate, Action<TEntity> action);

  /// <summary>
  ///   Deletes an entity from the repository by its identifier.
  /// </summary>
  /// <param name="id">The identifier of the entity to delete.</param>
  /// <exception cref="ArgumentNullException">If the <paramref name="id" /> is <c>null</c>.</exception>
  void Delete(Guid id);

  /// <summary>
  ///   Deletes an entity from the repository.
  /// </summary>
  /// <param name="entity">The entity to delete.</param>
  /// <exception cref="ArgumentNullException">If the <paramref name="entity" /> is <c>null</c>.</exception>
  void Delete(TEntity entity);

  /// <summary>
  ///   Deletes a range of entities from the repository.
  /// </summary>
  /// <param name="entities">The entities to delete.</param>
  /// <exception cref="ArgumentNullException">If the <paramref name="entities" /> is <c>null</c>.</exception>
  void Delete(IEnumerable<TEntity> entities);

  /// <summary>
  ///   Deletes a range of entities from the repository.
  /// </summary>
  /// <param name="predicate">The predicate to filter the entities to delete.</param>
  /// <exception cref="ArgumentNullException">If the <paramref name="predicate" /> is <c>null</c>.</exception>
  void Delete(Expression<Func<TEntity, bool>> predicate);

  /// <summary>
  ///   Deletes a range of entities from the repository by their identifiers.
  /// </summary>
  /// <param name="identifiers">The identifiers of the entities to delete.</param>
  /// <exception cref="ArgumentNullException">If the <paramref name="identifiers" /> is <c>null</c>.</exception>
  void Delete(IEnumerable<Guid> identifiers);

  /// <summary>
  ///   Deletes all entities from the repository.
  /// </summary>
  void Truncate();
}
