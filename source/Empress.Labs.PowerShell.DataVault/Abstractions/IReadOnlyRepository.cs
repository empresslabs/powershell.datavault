// Copyright (c) Bruno Sales <me@baliestri.dev>. Licensed under the MIT License.
// See the LICENSE file in the repository root for full license text.

using System.Linq.Expressions;

namespace Empress.Labs.PowerShell.DataVault.Abstractions;

/// <summary>
///   A read-only repository that provides basic operations to entities.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public interface IReadOnlyRepository<TEntity> : IDisposable where TEntity : Entity, new() {
  /// <summary>
  ///   Counts all entities in the repository.
  /// </summary>
  /// <returns>The number of entities.</returns>
  int Count();

  /// <summary>
  ///   Counts all entities in the repository by a predicate.
  /// </summary>
  /// <param name="predicate">The predicate to use.</param>
  /// <returns>The number of entities.</returns>
  /// <exception cref="ArgumentNullException">If the <paramref name="predicate" /> is <c>null</c>.</exception>
  int Count(Expression<Func<TEntity, bool>> predicate);

  /// <summary>
  ///   Checks if an entity exists in the repository by its identifier.
  /// </summary>
  /// <param name="id">The identifier of the entity to find.</param>
  /// <returns><c>true</c> if the entity exists, <c>false</c> otherwise.</returns>
  bool Exists(Guid id);

  /// <summary>
  ///   Checks if an entity exists in the repository by a predicate.
  /// </summary>
  /// <param name="predicate">The predicate to use.</param>
  /// <returns><c>true</c> if the entity exists, <c>false</c> otherwise.</returns>
  /// <exception cref="ArgumentNullException">If the <paramref name="predicate" /> is <c>null</c>.</exception>
  bool Exists(Expression<Func<TEntity, bool>> predicate);

  /// <summary>
  ///   Gets an entity in the repository by its identifier.
  /// </summary>
  /// <param name="id">The identifier of the entity to find.</param>
  /// <returns>The entity if found, null otherwise.</returns>
  TEntity? Get(Guid id);

  /// <summary>
  ///   Gets an entity in the repository by a predicate.
  /// </summary>
  /// <param name="predicate">The predicate to use.</param>
  /// <returns>The entity if found, null otherwise.</returns>
  /// <exception cref="ArgumentNullException">If the <paramref name="predicate" /> is <c>null</c>.</exception>
  TEntity? Get(Expression<Func<TEntity, bool>> predicate);

  /// <summary>
  ///   Finds all entities in the repository.
  /// </summary>
  /// <returns>The found entities or an empty collection.</returns>
  IReadOnlyCollection<TEntity> Find();

  /// <summary>
  ///   Finds all entities in the repository by a predicate.
  /// </summary>
  /// <param name="predicate">The predicate to use.</param>
  /// <returns>The entities found by the predicate or an empty collection.</returns>
  /// <exception cref="ArgumentNullException">If the <paramref name="predicate" /> is <c>null</c>.</exception>
  IReadOnlyCollection<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
}
